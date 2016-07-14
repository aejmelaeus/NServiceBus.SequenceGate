# NServiceBus.SequenceGate

NServiceBus is about messages. The Sequence Gate is about the objects in the messages.

## Why?

We want to be sure that no state is messed up when we are retrying messages from the error queue.

We found ourselves to implementing custom Sagas for message correlation in different Endpoints. This is a way to standardize handling messages that arrive in the wrong order.

Messaging is unreliable by nature. Messages can arrive in the wrong order, or not arrive at all. The Sequence Gate can help when messages arrive in wrong order.

The Sequence Gate could be used in situations where only the latest state of an object or resource is needed.

## Background

Some events in a system are imutable. For example `SendEmail` or `WriteToAuditLog`. But other messages are mutable by nature and changes some objects state, for example `UserEmailUpdated`. There could also be messages that take out each other, for example `PermissionGrantedForUser` and `PermissionDeniedForUser`. Often it is important to get these types of messages in the correct order in downstream systems, or at least to only capture the latest state.

## Limitations

The Sequence Gate has performance impacts. When performance is key and you working in a high frequence environment you should try to design your events to be immutable or your Endpoint's business logic should use an Event Store that by nature mitigates timing issues.

However, when the your Endpoint is working with some legacy component of your system or a third party system it can be a simple and efficient way to solve sequencing issues. 

The Sequence Gate will only work relyably in a single Endpoint - single thread environment. This means that there can only be a single Endpoint with a single thread processing messages at a time. Otherwise consistency can not be guaranteed. This implies that the domain is not a fast data domain, where the Endpoints must be scaled out.

## Consider to use the Sequence gate when:

- It is important to store or process the latest state of a object.
- Your are working in a "low frequency environment"

## This will not be usefull to you when:

- In a fast data environment
- Your need to scale out the number of Endpoints or Endpoint threads

## Sequence

The messages are sequence anchored using the `long` datatype. The DateTime class can be used in the publishing Endpoint. The time is expected to be reasonably synchronized between the publishing nodes.

## Persistence

The current implementation uses SQL Server as persistence. The Sequence Gate expects a connection string called `NServiceBus/SequenceGate`. The database used in the connections string is expected to have a table called `dbo.TrackedObjects`. The table can be created with the following [Script].

# Examples

The Sequence Gate is implemented as a NServiceBus Pipeline component. The SequenceGate is registered with the bus and all messages that the Endpoint processes are passed through the gate.

The basic flow is:
- Is the message a Sequence Gate member? Yes - keep on processing it. No - pass it through the pipeline.
- Parse the objects from the message
- Find out if some objects needs to be discarded
- Mark the newer or unseen ones ones with the timestamp
- Mutate the message if needed

### Message mutation

Messages are mutated in the Sequence Gate.

If it is a single object message the whole message is dropped by stopping pipeline execution.

If it is a message containing multiple objects the objects that has newer seen versions are removed from the message before passed on in the pipeline.

## Single object messages

Often a message contains a single object to keep track of, for example ´UserEmailUpdated`: [Acceptance test OK]

``` csharp

public class UserEmailUpdated : IMessage
{
    public Guid UserId { get; set; }
	public string EmailAdress { get; set; }
	public DateTime TimeStamp { get; set; }
}

```

The ´UserEmailUpdated´ message becomes a member of the SequenceGate by adding it to the SequenceGateConfiguration:

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserEmailUpdated";
	member.HasSingleObjectMessage<UserEmailUpdated>(metadata =>
	{
		metadata.ObjectIdPropertyName = nameof(UserEmailUpdated.UserId);
		metadata.TimeStampPropertyName = nameof(UserEmailUpdated.TimeStamp);
	});
});

busConfiguration.SequenceGate(configuration);

```

There can also be several messages modifying the same object: [Acceptance test OK]

``` csharp

public class UserAddedToRole : IMessage
{
	public Guid UserId { get; set; }
	public string Role { get; set; }
	public DateTime TimeStamp { get; set; }
}

public class UserRemovedFromRole : IMessage
{
	public Guid UserId { get; set; }
	public string Role { get; set; }
	public DateTime TimeStamp { get; set; }
}

```

Then the SequenceGate member is configured with two messages:

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserRoleActions";
	member.HasSingleObjectMessage<UserAddedToRole>(metadata =>
	{
		metadata.ObjectIdPropertyName = nameof(UserAddedToRole.UserId);
		metadata.TimeStampPropertyName = nameof(UserAddedToRole.TimeStamp);
	});
	member.HasSingleObjectMessage<UserRemovedFromRole>(metadata =>
	{
		metadata.ObjectIdPropertyName = nameof(UserRemovedFromRole.UserId);
		metadata.TimeStampPropertyName = nameof(UserRemovedFromRole.TimeStamp);
	});
});

```

The id for the member is given a descriptive id, in this case `UserRoleActions`.

## Endpoint name

The Sequence Gate Configuration takes in the Endpoint name:

`var configuration = new SequenceGateConfiguration("EndpointName")`

In this way several Endpoints may share the same database. [Acceptance test OK]

## Multiple object messages

Some messages contains several objects that the Sequence Gate needs to take into consideration: [Acceptance test OK]

``` csharp

public class UsersAddedToGroup : IMessage
{
	public string Group { get; set; }
	public List<User> Users { get; set; }
	public DateTime TimeStamp { get; set; }
}

public class UsersRemovedFromGroup : IMessage
{
	public string Group { get; set; }
	public List<User> Users { get; set; }
	public DateTime TimeStamp { get; set; }
}

public class User 
{	
	public Guid Id { get; set; }
	public string Firstname { get; set; }
	public string Lastname { get; set; }
}

```

Then the Sequence Gate is configured with a `CollectionPropertyName` and the `ObjectIdPropertyName` is expeced to be on the object in the collection.

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserGroupActions";
	member.HasMultipleObjectsMessage<UsersAddedToGroup>(metadata =>
	{
		metadata.CollectionPropertyName = nameof(UsersAddedToGroup.Users);
		metadata.ObjectIdPropertyName = nameof(User.Id);
		metadata.TimeStampPropertyName = nameof(TimeStamp);
	});
	member.HasMultipleObjectsMessage<UsersRemovedFromGroup>(metadata =>
	{
		metadata.CollectionPropertyName = nameof(UsersRemovedFromGroup.Users);
		metadata.ObjectIdPropertyName = nameof(User.Id);
		metadata.TimeStampPropertyName = nameof(TimeStamp);
	});
});

```

The property names in the configuration can be set with the ´nameof´ operator when the property is directly on the root-object.

But some messages might have the properties that are used in the gate as complex datastructures:

``` csharp

public class UserEmailUpdated : IMessage
{
    public Guid UserId { get; set; }
	public string EmailAdress { get; set; }
	public Metadata Metadata { get; set; }
}

public class Metadata
{
	public DateTime TimeStamp { get; set; }
	public Guid LoggedInUserId { get; set; }
}

```

In the example the time stamp is a complex object on the message. Configuration is done using a string as separator of the properties:

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserEmailUpdated";
	member.HasSingleObjectMessage<UserEmailUpdated>(metadata =>
	{
		metadata.ObjectIdPropertyName = nameof(UserEmailUpdated.UserId);
		metadata.TimeStampPropertyName = "Metadata.TimeStamp";
	});
});

```

### Value collections

In some cases the message contains a collection of value type that are object ids: [Acceptance test]

´´´ csharp

public class DeactivatedUsers
{
	public List<Guid> UserIds { get; set; }
	public DateTime TimeStamp { get; set; }
}

```

In this case the `ObjectIdPropertyName` is omitted:

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserActivationActions";
	member.HasMultipleObjectsMessage<DeactivatedUsers>(metadata =>
	{
		metadata.CollectionPropertyName = nameof(DeactivatedUsers.UserIds);
		metadata.TimeStampPropertyName = "TimeStamp";
	});
});

```

A value typed collection can contain following types as Object Id:

- ´string´
- ´int´
- ´long´
- ´Guid´

## Scope

Sometimes a scope is needed when using the Sequence Gate. For example a system can be modelled in such a way that a user may have different roles on different clients in the system. The client becomes the scope: [Acceptance test]

``` csharp

public class UserRoleUpdatedOnClient
{
	public Guid UserId { get; set; }
	public string Role { get; set; }
	public string ClientId { get; set; }
	public DateTime TimeStamp { get; set; }
}

```

The `ScopeIdPropertyName` is configured on the message metadata:

``` csharp

var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
{
	member.Id = "UserRoleUpdatedOnClient";
	member.HasSingleObjectMessage<UserRoleUpdatedOnClient>(metadata =>
	{
		metadata.ObjectIdPropertyName = nameof(UserRoleUpdatedOnClient.UserId);
		metadata.ScopeIdPropertyName = nameof(UserRoleUpdatedOnClient.ClientId);
		metadata.TimeStampPropertyName = nameof(UserRoleUpdatedOnClient.TimeStamp);		
	});
});

```

## Validation

The ´SequenceGateConfiguration´ is validated before the Endpoint is started. If the validation fails the Endpoint will fail to start [Acceptance test]. The validation verifies that all configured properties actually exists on the message types and that the types are correct.

# TODO: Filter

Implement a filter that can be configured.