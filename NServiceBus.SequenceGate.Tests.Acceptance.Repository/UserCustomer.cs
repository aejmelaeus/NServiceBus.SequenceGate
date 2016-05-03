using System;
using System.ComponentModel.DataAnnotations;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository
{
    public class UserCustomer
    {
        [Key]
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}