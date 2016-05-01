namespace NServiceBus.SequenceGate
{
    internal enum ValidationError
    {
        MessageTypeMissing,
        ObjectIdPropertyMissing,
        TimeStampPropertyMissingOrNotDateTime,
        ScopeIdPropertyMissing,
        CollectionPropertyMissingOrNotICollection,
        ObjectIdPropertyMissingOnObjectInCollection,
        CollectionObjectTypeNotInAllowedBasicCollectionTypes,
        AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent,
        IdMissingOnSequenceGateMember,
        SequenceMetadataIsMissingOnMember
    }
}