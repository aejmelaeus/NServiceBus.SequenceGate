using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public void Register(List<NServiceBus.SequenceGate.TrackedObject> trackedObjects)
        {
            using (var context = new SequenceGateContext())
            {
                
            }
        }

        public List<string> Register(Parsed parsed)
        {
            using (var context = new SequenceGateContext())
            {
                var query = GetQuery(parsed, context.TrackedObjects);
                var actions = GetActions(parsed, query);

                var entitiesToAdd = GetEntitiesToAdd(parsed, actions.ObjectIdsToAdd);
                context.TrackedObjects.AddRange(entitiesToAdd);

                UpdateEntities(context, parsed.SequenceAnchor, actions.IdsToUpdate);

                context.SaveChanges();

                return actions.ObjectIdsToDismiss;
            }
        }

        private void UpdateEntities(SequenceGateContext context, long sequenceAnchor, List<long> objedctIdsToUpdate)
        {
            var entitiesToUpdate = context.TrackedObjects.Where(e => objedctIdsToUpdate.Contains(e.Id)).ToList();
            entitiesToUpdate.ForEach(e => e.SequenceAnchor = sequenceAnchor);
        }

        public List<string> ListObjectIdsToDismiss(List<NServiceBus.SequenceGate.TrackedObject> trackedObjects)
        {
            var result = new List<string>();

            var sequenceGateId = trackedObjects.First().SequenceGateId;
            var scopeId = trackedObjects.First().ScopeId;
            var sequenceAnchor = trackedObjects.First().SequenceAnchor;
            var objectIds = trackedObjects.Select(to => to.ObjectId);

            using (var context = new SequenceGateContext())
            {
                var newest = context.TrackedObjects
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
        internal Actions GetActions(Parsed parsed, IQueryable<TrackedObject> entities)
        {
            var result = new Actions();

            var idsToAdd = parsed.ObjectIds.Except(entities.Select(e => e.ObjectId));
            var idsToUpdate = entities.Where(e => e.SequenceAnchor < parsed.SequenceAnchor).Select(e => e.Id);
            var idsToDismiss = entities.Where(e => e.SequenceAnchor > parsed.SequenceAnchor).Select(e => e.ObjectId);

            result.ObjectIdsToAdd = idsToAdd.ToList();
            result.IdsToUpdate = idsToUpdate.ToList();
            result.ObjectIdsToDismiss = idsToDismiss.ToList();

            return result;
        }

        internal IQueryable<TrackedObject> GetQuery(Parsed parsed, IQueryable<TrackedObject> entities)
        {
            return entities.Where(e => 
                e.EndpointName.Equals(parsed.EndpointName) && 
                e.ScopeId.Equals(parsed.ScopeId) &&
                e.SequenceGateId.Equals(parsed.SequenceGateId)
            );
        }

        public IEnumerable<TrackedObject> GetEntitiesToAdd(Parsed parsed, List<string> idsToAdd)
        {
            return idsToAdd.Select(i => new TrackedObject
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
