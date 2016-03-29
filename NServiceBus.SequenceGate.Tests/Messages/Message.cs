using System;

namespace NServiceBus.SequenceGate.Tests.Messages
{
    public class UserEmailUpdated
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}