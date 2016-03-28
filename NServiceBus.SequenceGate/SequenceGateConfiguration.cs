using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Repository
{
    public class SequenceGateConfiguration : List<SequenceGate>
    {
        // Nothing here...
    }
    public class SequenceGate
    {
        public string Id { get; set; }
        public string ScopeId { get; set; }
        public List<SequenceGateMember> Members { get; set; }
    }
}
