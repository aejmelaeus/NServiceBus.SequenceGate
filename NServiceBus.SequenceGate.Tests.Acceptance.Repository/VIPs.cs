using System;
using System.ComponentModel.DataAnnotations;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository
{
    public class VIPs
    {
        [Key]
        public Guid UserId { get; set; }
    }
}
