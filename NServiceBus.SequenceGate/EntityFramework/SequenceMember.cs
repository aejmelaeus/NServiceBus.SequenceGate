using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NServiceBus.SequenceGate.EntityFramework
{
    public class SequenceMember
    {
        [Key]
        public long Id { get; set; }

        [Index("IX_SequenceGateId_ScopeId_EndpointName", 1, IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string SequenceGateId { get; set; }

        [Index("IX_SequenceGateId_ScopeId_EndpointName", 2, IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string ScopeId { get; set; }

        [Index("IX_SequenceGateId_ScopeId_EndpointName", 3, IsUnique = true)]
        [Required]
        [MaxLength(128)]
        public string EndpointName { get; set; }

        public virtual List<SequenceObject> Objects { get; set; }
    }
}
