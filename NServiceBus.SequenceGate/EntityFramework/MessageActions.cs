using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class MessageActions
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
        /// The ObjectIds to update, it the Id, not the ObjectId!
        /// </summary>
        public List<string> ObjectIdsToUpdate { get; set; } = new List<string>();
    }
}