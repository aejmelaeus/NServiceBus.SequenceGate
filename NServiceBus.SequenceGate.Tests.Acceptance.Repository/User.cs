using System;
using System.ComponentModel.DataAnnotations;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}