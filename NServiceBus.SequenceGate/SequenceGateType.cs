using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Repository
{
    /// <summary>
    /// TODO: Figure out what to call this...
    /// </summary>
    public class SequenceGateType
    {
        public string Id { get; set; }
        public List<SequenceGateMember> Members { get; set; }
    }
}