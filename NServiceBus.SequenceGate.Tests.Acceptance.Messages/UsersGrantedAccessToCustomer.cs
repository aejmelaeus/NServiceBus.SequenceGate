using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Messages
{
    public class UsersGrantedAccessToCustomer : ICommand
    {
        public Customer Customer { get; set; }
        public List<User> Users { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}