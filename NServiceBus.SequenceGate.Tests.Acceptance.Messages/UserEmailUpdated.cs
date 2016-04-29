using System;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Messages
{
    public class UserEmailUpdated : ICommand
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime TimeStampUtc { get; set; }
    }
}
