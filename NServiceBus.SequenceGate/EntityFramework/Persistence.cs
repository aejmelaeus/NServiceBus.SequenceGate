using System;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public void Register(List<TrackedObject> trackedObjects)
        {
            using (var context = new TrackedObjectsContext())
            {
                foreach (var trackedObject in trackedObjects)
                {
                    var trackedObjectEntity = new TrackedObjectEntity();
                    trackedObjectEntity.ObjectId = trackedObject.ObjectId;
                    trackedObjectEntity.ScopeId = trackedObject.ScopeId;
                    trackedObjectEntity.SequenceGateId = trackedObject.SequenceGateId;
                    trackedObjectEntity.SequenceAnchor = trackedObject.SequenceAnchor;

                    context.TrackedObjectEntities.Add(trackedObjectEntity);
                }

                context.SaveChanges();
            }
        }

        public List<string> ListObjectIdsToDismiss(List<TrackedObject> trackedObjects)
        {
            throw new NotImplementedException();
        }
    }
}
