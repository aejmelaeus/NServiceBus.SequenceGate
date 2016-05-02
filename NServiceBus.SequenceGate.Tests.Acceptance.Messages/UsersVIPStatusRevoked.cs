using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Messages
{
    public class VIPStatusRevoked : ICommand
    {
        public List<Guid> UserIds { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}