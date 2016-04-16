using System;

namespace NServiceBus.SequenceGate.Tests.Messages
{
    internal class UserEmailUpdated
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    internal class MetaData
    {
        public DateTime TimeStamp { get; set; }
    }

    internal class UserEmailUpdatedContainingMetaData
    {
        public MetaData MetaData { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}