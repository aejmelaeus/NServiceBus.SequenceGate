using System.Collections.Generic;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    internal interface IRepository
    {
        /// <summary>
        /// Inserts a entry for each object
        /// </summary>
        /// <param name="gateData"></param>
        void Register(List<GateData> gateData);

        /// <summary>
        /// Returns the ids of the objects that already are seen
        /// </summary>
        /// <param name="gateData"></param>
        /// <returns></returns>
        List<string> ListSeenObjectIds(List<GateData> gateData);
    }
}
