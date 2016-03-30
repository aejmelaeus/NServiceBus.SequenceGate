using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateQuery
    {
        public string SequenceGateId { get; set; }
        public DateTime TimeStampUTC { get; set; }
        public string ScopeId { get; set; }
        public IEnumerable<string> ObjectIds { get; set; }
    }
}
