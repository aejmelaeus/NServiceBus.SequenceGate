using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Actions
    {
        /// <summary>
        /// The unseen object ids for the current filtering
        /// </summary>
        public List<string> ObjectIdsToAdd { get; set; } = new List<string>();
        /// <summary>
        /// The objects ids that has newer versions already seen
        /// </summary>
        public List<string> ObjectIdsToDismiss { get; set; } = new List<string>();
        /// <summary>
        /// The IDs to update, it the Id, not the ObjectId!
        /// </summary>
        public List<long> IdsToUpdate { set; get; } = new List<long>();
    }
}