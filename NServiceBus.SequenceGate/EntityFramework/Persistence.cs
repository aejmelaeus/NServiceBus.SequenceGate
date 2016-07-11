using System.Linq;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public List<string> Register(Parsed parsed)
        {
            using (var context = new SequenceGateContext())
            {
                var query = GetObjectsFromParsedMessage(parsed, context.TrackedObjects);
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

        /// <summary>
        /// Returns the actions.
        /// </summary>
        /// <param name="parsed">The parsed data</param>
        /// <param name="entitiesInMessage">The entities that matches EndpointName and ScopeId and ObjectIds</param>
        /// <returns></returns>
        internal Actions GetActions(Parsed parsed, List<TrackedObject> entitiesInMessage)
        {
            return new Actions
            {
                ObjectIdsToAdd = parsed.ObjectIds.Except(entitiesInMessage.Select(e => e.ObjectId))
                    .ToList(),
                IdsToUpdate = entitiesInMessage.Where(e => e.SequenceAnchor < parsed.SequenceAnchor)
                    .Select(e => e.Id)
                    .ToList(),
                ObjectIdsToDismiss = entitiesInMessage.Where(e => e.SequenceAnchor > parsed.SequenceAnchor)
                    .Select(e => e.ObjectId)
                    .ToList()
            };
        }

        internal List<TrackedObject> GetObjectsFromParsedMessage(Parsed parsed, IQueryable<TrackedObject> entities)
        {
            return entities.Where(e => 
                e.EndpointName.Equals(parsed.EndpointName) && 
                e.ScopeId.Equals(parsed.ScopeId) &&
                e.SequenceGateId.Equals(parsed.SequenceGateId) &&
                parsed.ObjectIds.Contains(e.ObjectId)
            ).ToList();
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
