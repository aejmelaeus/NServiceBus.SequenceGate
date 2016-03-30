using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Repository
{
    public interface ISequenceGateRepository
    {
        /// <summary>
        /// Inserts a entry for each object
        /// </summary>
        /// <param name="query"></param>
        void Register(string sequenceGateId, SequenceGateQuery query);
        /// <summary>
        /// Returns the ids of the objects that already are seen
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<string> ListObjectIdsWithNewerDates(SequenceGateQuery query);
    }
}
