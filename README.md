# NServiceBus.SequenceGate

A Sequence Gate where older revisions or status of messages containing a certain object are discarded. Used in scenarios where only the newest revision or status of an object is needed. For example setting of permissions in downstream systems.

## Background

Usually message driven systems are designed in such a way that message order does not matter.

However this might not always be possible and this Sequence Gate tries to solve that problem. One way to correlate message order in NServiceBus is to use Sagas. We could have two events "CompanyCreated" and "CompanyUpdated". We could have a Saga to handle when the "CompanyUpdated" event is arriving before the "CompanyCreated".

The scenario that the Sequence Gate is solving is when there are states that takes out each other and the last stat is the one that we are intrested in.

Examples of this might be `UserEmailUpdated` or `PermissionGrantedForUser` and `PermissionDeniedForUser`. These types of events are important to get in the right order in downstream systems.

## Sequence

The messages are anchored in time using a time stamp in the Publishing system. Time is expected to be handled in UTC-format in all endpoints. The computers publishing messages are expected to have synchronized time.

## Data

We keep track of already seen messages to be able to discard older version. The following data is stored in the database:

* Test
* Test
* Test

New row

Test.

Again.

### Hihi Hoho

Test

Long long long: https://github.com/aejmelaeus/NServiceBus.SequenceGate.git long long long long long https://github.com/aejmelaeus/NServiceBus.SequenceGate.git

