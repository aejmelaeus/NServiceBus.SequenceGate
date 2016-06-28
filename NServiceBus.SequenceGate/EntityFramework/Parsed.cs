using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Parsed
    {
        public string EndpointName { get; set; }
        public string SequenceGateId { get; set; }
        public string ScopeId { get; set; }
        public List<string> ObjectIds { get; set; }
        public long SequenceAnchor { get; set; }
    }
}