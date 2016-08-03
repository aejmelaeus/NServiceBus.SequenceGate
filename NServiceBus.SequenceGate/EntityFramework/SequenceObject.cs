using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NServiceBus.SequenceGate.EntityFramework
{
    public class SequenceObject
    {
        [Key]
        [Column(Order = 1)]
        [MaxLength(128)]
        public string Id { get; set; }

        [Required]
        public long SequenceAnchor { get; set; }

        [Required]
        public virtual SequenceMember SequenceMember { get; set; }

        [Key]
        [Column(Order = 2)]
        public long SequenceMemberId { get; set; }

    }
}
