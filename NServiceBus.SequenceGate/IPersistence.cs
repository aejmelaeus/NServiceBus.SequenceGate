using System.Collections.Generic;

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
        /// Returns the ids of the objects that has a been handled with a more recent timestamp.
        /// </summary>
        /// <param name="trackedObjects"></param>
        /// <returns>A list of object ids that has been handled more recently</returns>
        List<string> ListObjectIdsToDismiss(List<TrackedObject> trackedObjects);
    }
}
