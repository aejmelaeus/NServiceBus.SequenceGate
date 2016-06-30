using System.Collections.Generic;
using NServiceBus.SequenceGate.EntityFramework;

namespace NServiceBus.SequenceGate
{
    internal interface IPersistence
    {
        /// <summary>
        /// Inserts a entry for each object
        /// </summary>
        /// <param name="trackedObjects"></param>
        void Register(List<TrackedObject> trackedObjects);

        /// <summary>
        /// Registers the new objects
        /// Updates existing objects that are newer
        /// Returns the object ids to dismiss, the ones that already has newer ones seen in the Db.
        /// </summary>
        /// <param name="parsed">The parsing result</param>
        /// <returns>The object ids to dismiss</returns>
        List<string> Register(Parsed parsed);

            /// <summary>
        /// Returns the ids of the objects that has a been handled with a more recent timestamp.
        /// </summary>
        /// <param name="trackedObjects"></param>
        /// <returns>A list of object ids that has been handled more recently</returns>
        List<string> ListObjectIdsToDismiss(List<TrackedObject> trackedObjects);
    }
}
