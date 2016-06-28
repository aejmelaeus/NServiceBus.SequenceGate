using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public void Register(List<TrackedObject> trackedObjects)
        {
            using (var context = new TrackedObjectsContext())
            {
                
            }
        }

        public List<string> Register(Parsed parsed)
        {
            // Join -> Those only in trackedObjects parameter should be added
            // Foreach: Anchor is newer? update anchor : add id to return value

            var result = new List<string>();

            return result;
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

        /// <summary>
        /// Returns the actions.
        /// </summary>
        /// <param name="parsed">The parsed data</param>
        /// <param name="entities">The entities that matches EndpointName and ScopeId</param>
        /// <returns></returns>
        internal Actions GetActions(Parsed parsed, IQueryable<TrackedObjectEntity> entities)
        {
            var result = new Actions();

            var idsToAdd = parsed.ObjectIds.Except(entities.Select(e => e.ObjectId));
            var idsToUpdate = entities.Where(e => e.SequenceAnchor < parsed.SequenceAnchor).Select(e => e.ObjectId);
            var idsToDismiss = entities.Where(e => e.SequenceAnchor > parsed.SequenceAnchor).Select(e => e.ObjectId);

            result.IdsToAdd = idsToAdd.ToList();
            result.IdsToUpdate = idsToUpdate.ToList();
            result.IdsToDismiss = idsToDismiss.ToList();

            return result;
        }

        internal IQueryable<TrackedObjectEntity> GetQuery(Parsed parsed, IQueryable<TrackedObjectEntity> entities)
        {
            return entities.Where(e => 
                e.EndpointName.Equals(parsed.EndpointName) && 
                e.ScopeId.Equals(parsed.ScopeId) &&
                e.SequenceGateId.Equals(parsed.SequenceGateId)
            );
        }
    }
}
