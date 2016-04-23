using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public void Register(List<TrackedObject> trackedObjects)
        {
            throw new NotImplementedException();
        }

        public List<string> ListObjectIdsToDismiss(List<TrackedObject> trackedObjects)
        {
            throw new NotImplementedException();
        }
    }
}
