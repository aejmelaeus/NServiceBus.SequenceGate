# NServiceBus.SequenceGate

NServiceBus is about messages. The Sequence Gate is about the objects in the messages.

## Why?

We want to be sure that no state is messed up when we are retrying messages from the error queue.

We fund ourselves to implement a custom Saga for message correlation in different Endpoints. This is a way to standardize handling messages that arrive in the wrong order.

Messaging is unreliable by nature. Messages can arrive in the wrong order, or not arrive at all. The Sequence Gate can help when messages arrive in wrong order.

The Sequence Gate could be used in situations where only the latest state of an object or resource is needed.

## Background

Some events in a system are imutable. For example `SendEmail` or `WriteToAuditLog`. But other messages are mutable by nature and changes some objects state, for example `UserEmailUpdated`. There could also be messages that take out each other, for example `PermissionGrantedForUser` and `PermissionDeniedForUser`. Often it is important to get these types of messages in the correct order in downstream systems, or at least to only capture the latest state.

## Limitations

The Sequence Gate has performance impacts. When performance is key and you working in a high frequence environment you should try to design your events to be immutable or your Endpoint's business logic should use an Event Store that by nature mitigates timing issues.

However, when the your Endpoint is working with some legacy component of your system or a third party system it can be a simple and efficient way to solve sequencing issues. 

The Sequence Gate will only work in a single Endpoint single thread environment. This means that there can only be a single Endpoint with a single thread processing messages at a time. Otherwise consistency can not be guaranteed. This implies that the domain is not a fast data domain, where the Endpoints must be scaled out.

## Consider to use the Sequence gate when:

- It is important to store or process the latest state of a message.
- Your are working in a "low frequency environment"

## This will not be usefull to you when:

- In a fast data environment
- Your need to scale out the number of Endpoints or Endpoint threads

## Sequence

The messages are sequnce anchored using the `long` datatype. The DateTime class can be used in the publishing endpoint when the time is synchronized between the publishing nodes.

## Persistence

The current implementation uses SQL Server as persistence. The Sequence Gate expects a connection string called `NServiceBus/SequenceGate`. The database used in the connections string is expected to have a table called `dbo.TrackedObjects`. The table can be created with the following [Script].

# Examples

- Single
- ValueCollection
- ComplexCollection
- Scope