using System;

namespace NServiceBus.SequenceGate.Tests.Messages
{
    internal class SimpleMessage
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    internal class TimeStampInWrongFormat
    {
        public string Id { get; set; }
        public string TimeStamp { get; set; }
    }

    internal class MetaData
    {
        public DateTime TimeStamp { get; set; }
    }

    internal class User
    {
        public Guid Id { get; set; }  
    }

    internal class ComplexMetaDataMessage
    {
        public MetaData MetaData { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }

    internal class ComplexUserAndMetaDataMessage
    {
        public MetaData MetaData { get; set; }
        public User User { get; set; }
    }
}