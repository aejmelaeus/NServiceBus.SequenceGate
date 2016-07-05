using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Parsed
    {
        private readonly List<string> _objectIds = new List<string>();

        internal Parsed(string endpointName, string sequenceGateId, string scopeId, long sequenceAnchor)
        {
            EndpointName = endpointName;
            SequenceGateId = sequenceGateId;
            ScopeId = scopeId;
            SequenceAnchor = sequenceAnchor;
        }

        internal void AddObjectId(string objectId)
        {
            _objectIds.Add(objectId);
        }

        internal string EndpointName { get; }

        internal string SequenceGateId { get; }

        internal string ScopeId { get; }

        internal long SequenceAnchor { get; }

        internal IEnumerable<string> ObjectIds => _objectIds;
    }
}