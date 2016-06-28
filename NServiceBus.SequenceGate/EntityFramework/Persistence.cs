using System.Collections.Generic;
using System.Data.Entity;
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
            using (var context = new TrackedObjectsContext())
            {
                var query = GetQuery(parsed, context.TrackedObjectEntities);
                var actions = GetActions(parsed, query);

                var entitiesToAdd = GetEntitiesToAdd(parsed, actions.IdsToAdd);
                context.TrackedObjectEntities.AddRange(entitiesToAdd);

                UpdateEntities(context, parsed.SequenceAnchor, actions.IdsToUpdate);

                context.SaveChanges();

                return actions.IdsToDismiss;
            }
        }

        private void UpdateEntities(TrackedObjectsContext context, long sequenceAnchor, List<string> objedctIdsToUpdate)
        {
            var entitiesToUpdate = context.TrackedObjectEntities.Where(e => objedctIdsToUpdate.Contains(e.ObjectId)).ToList();
            entitiesToUpdate.ForEach(e => e.SequenceAnchor = sequenceAnchor);
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

        public IEnumerable<TrackedObjectEntity> GetEntitiesToAdd(Parsed parsed, List<string> idsToAdd)
        {
            return idsToAdd.Select(i => new TrackedObjectEntity
            {
                EndpointName = parsed.EndpointName,
                ObjectId = i,
                ScopeId = parsed.ScopeId,
                SequenceAnchor = parsed.SequenceAnchor,
                SequenceGateId =  parsed.SequenceGateId
            });
        }
    }
}
