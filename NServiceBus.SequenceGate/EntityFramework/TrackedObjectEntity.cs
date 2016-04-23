using System;
using System.ComponentModel.DataAnnotations;

namespace NServiceBus.SequenceGate.EntityFramework
{
    public class TrackedObjectEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string SequenceGateId { get; set; }
        [Required]
        public string ObjectId { get; set; }
        [Required]
        public DateTime TimeStampUTC { get; set; }
        [Required]
        public string ScopeId { get; set; }
    }
}
