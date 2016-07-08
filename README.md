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

The Sequence Gate has performance impacts. When performance is key and you working in a high frequence environment you should design your Endpoint's business logic with an Event Store that by nature mitigates timing issues or try to design your events to be immutable.

However, when the your Endpoint is working with some legacy component of your system or a third party system it can be a simple and efficient way to solve sequencing issues. 

The Sequence Gate will only work in a single thread environment. This means that there can only be a single Endpoint with a single thread processing messages at a time. Otherwise consistency can not be guaranteed. This implies that the domain is not a fast data domain, where the Endpoints must be scaled out.

## Consider to use the Sequence gate when:

- It is important to store or process the latest state of a message. //, for example when a users email address is updated and has to be correct.
- Your are working in a "low frequency environment"

## This will not be usefull to you when:

- In a fast data environment
. Your need to scale out the number of Endpoints or Endpoint threads

## Sequence

The messages are anchored in time using a time stamp in the publishing endpoint. Time is expected to be handled in UTC-format in all endpoints. The computers publishing messages are expected to have synchronized time.

## Persistence

The current implementation uses SQL Server as persistence. The Sequence Gate expects a connection string called `NServiceBus/SequenceGate`. The scema in the database is expected to be named [Endpointname].SequenceGate, for example an Order Endpoint named Order.Endpoint would expect a table called `Order.Endpoint.SequenceGate`.

### Endpoints

Each Endpoint has it's own schema named as the Enpoint in the database pointed out by the connection string.

For example the Enpoint with the id Acme.Users.Endpoint expects to have a scema called `Acme.Users.Endpoint`.

### Scripting

The schemas are created 

## Data

We keep track of already seen messages to be able to discard older version. The following data is stored in the database:

* Id - The given Sequence id, for example UserActions
* SequenceId - The id of the object that is sequenced, for example a user id

### Object Id

TODO

### Collection

TODO

### Scope Id

TODO