using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Messages
{
    public class VIPStatusGranted : ICommand
    {
        public List<User> Users { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}