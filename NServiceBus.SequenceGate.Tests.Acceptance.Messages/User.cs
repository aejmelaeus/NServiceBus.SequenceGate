using System;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Messages
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}