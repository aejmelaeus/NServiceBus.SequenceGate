using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
            var result = new List<string>();

            var sequenceGateId = trackedObjects.First().SequenceGateId;
            var scopeId = trackedObjects.First().ScopeId;
            var sequenceAnchor = trackedObjects.First().SequenceAnchor;
            var objectIds = trackedObjects.Select(to => to.ObjectId);

            using (var context = new TrackedObjectsContext())
            {
                var newest = context.TrackedObjectEntities
                    .Where(entity => objectIds.Contains(entity.ObjectId) &&
                                     entity.SequenceGateId.Equals(sequenceGateId) &&
                                     entity.ScopeId.Equals(scopeId))
                    .GroupBy(entity => entity.ObjectId)
                    .ToDictionary(entity => entity.Key, entities => entities.Max(entity => entity.SequenceAnchor));
                
                foreach (var objectId in objectIds)
                {
                    if (newest.ContainsKey(objectId))
                    {
                        if (newest[objectId] > sequenceAnchor)
                        {
                            result.Add(objectId);
                        }
                    }
                }
            }

            return result;
        }
    }
}
