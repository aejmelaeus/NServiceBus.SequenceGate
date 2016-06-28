using System.ComponentModel.DataAnnotations;

namespace NServiceBus.SequenceGate.EntityFramework
{
    // TODO: This is a booring name...
    public class TrackedObjectEntity
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string SequenceGateId { get; set; }
        [Required]
        public string ObjectId { get; set; }
        [Required]
        public long SequenceAnchor { get; set; }
        [Required]
        public string ScopeId { get; set; }
        [Required]
        public string EndpointName { get; set; }
    }
}
