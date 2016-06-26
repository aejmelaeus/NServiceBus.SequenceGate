# NServiceBus.SequenceGate

A Sequence Gate where older revisions or status of messages containing a certain object are discarded. Used in scenarios where only the newest revision or status of an object is needed. For example setting of permissions in downstream systems.

## Background

Usually message driven systems are designed in such a way that message order does not matter.

If it is not possible to do that, Sagas in NServiceBus is a good way to correlate message ordering. This works when the message semantics has a distinct flow. When something for example is `Initialized` then `Activated` and last `Terminated`. Then the Saga can pick up `Activated` and store some intermidiate result until the `Initialized` event is received.

However, this is not possible in all scenarios. Examples of this might be `UserEmailUpdated` or `PermissionGrantedForUser` and `PermissionDeniedForUser`. These types of events are important to get in the right order in downstream systems.

## Limitations

The Sequence Gate will only work in a single thread environment. This means that there can only be a single Endpoint with a single thread processing messages at a time. Otherwise consistency can not be guaranteed. This implies that the domain is not a fast data domain, where the Endpoints must be scaled out.

## Consider to use the Sequence gate when:

- It is important to store or process the latest state of a message, for example when a users email address is updated and has to be correct.

## This will not be usefull to you when:

- In a fast data environment

## Sequence

The messages are anchored in time using a time stamp in the publishing endpoint. Time is expected to be handled in UTC-format in all endpoints. The computers publishing messages are expected to have synchronized time.

## Persistence

The current implementation uses SQL Server as persistence. The Sequence Gate expects a connection string called `NServiceBus/SequenceGate`.

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