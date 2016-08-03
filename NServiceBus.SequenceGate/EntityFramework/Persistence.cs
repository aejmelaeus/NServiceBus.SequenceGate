using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Persistence : IPersistence
    {
        public Persistence()
        {
            Database.SetInitializer(new NullDatabaseInitializer<SequenceGateContext>());
        }

        public List<string> Register(ParsedMessage parsedMessage)
        {
            using (var context = new SequenceGateContext())
            {
                var objects = GetObjectsFromParsedMessage(parsedMessage, context);
                var actions = GetActions(parsedMessage, objects); 
                
                AddEntities(context, parsedMessage, actions.ObjectIdsToAdd);
                UpdateEntities(objects, parsedMessage.SequenceAnchor, actions.ObjectIdsToUpdate);

                context.SaveChanges();

                return actions.ObjectIdsToDismiss;
            }
        }

        internal void AddEntities(SequenceGateContext context, ParsedMessage parsedMessage, List<string> objectIdsToAdd)
        {
            var sequenceMember = GetSequenceMember(context, parsedMessage);

            foreach (var objectId in objectIdsToAdd)
            {
                var sequenceObject = new SequenceObject
                {
                    Id = objectId,
                    SequenceMember = sequenceMember,
                    SequenceAnchor = parsedMessage.SequenceAnchor
                };

                context.SequenceObjects.Add(sequenceObject);
            }
        }

        private SequenceMember GetSequenceMember(SequenceGateContext context, ParsedMessage parsedMessage)
        {
            var sequenceMember = context.SequenceMembers.SingleOrDefault(s =>
                s.EndpointName.Equals(parsedMessage.EndpointName) &&
                s.ScopeId.Equals(parsedMessage.ScopeId) &&
                s.SequenceGateId.Equals(parsedMessage.SequenceGateId)
            );

            if (sequenceMember == default(SequenceMember))
            {
                sequenceMember = new SequenceMember
                {
                    EndpointName = parsedMessage.EndpointName,
                    ScopeId = parsedMessage.ScopeId,
                    SequenceGateId = parsedMessage.SequenceGateId
                };
            }

            return sequenceMember;
        }

        private void UpdateEntities(List<SequenceObject> objects, long sequenceAnchor, List<string> objectIdsToUpdate)
        {
            var entitiesToUpdate = objects.Where(o => objectIdsToUpdate.Contains(o.Id)).ToList();
            entitiesToUpdate.ForEach(e => e.SequenceAnchor = sequenceAnchor);
        }

        internal MessageActions GetActions(ParsedMessage parsedMessage, List<SequenceObject> objects)
        {
            return new MessageActions
            {
                ObjectIdsToAdd = parsedMessage.ObjectIds.Except(objects.Select(e => e.Id))
                    .ToList(),
                ObjectIdsToUpdate = objects.Where(e => e.SequenceAnchor < parsedMessage.SequenceAnchor)
                    .Select(e => e.Id)
                    .ToList(),
                ObjectIdsToDismiss = objects.Where(e => e.SequenceAnchor > parsedMessage.SequenceAnchor)
                    .Select(e => e.Id)
                    .ToList()
            };
        }

        internal List<SequenceObject> GetObjectsFromParsedMessage(ParsedMessage parsedMessage, SequenceGateContext context)
        {
            return context.SequenceObjects.Where(
                o => parsedMessage.ObjectIds.Contains(o.Id) &&
                o.SequenceMember.EndpointName.Equals(parsedMessage.EndpointName) &&
                o.SequenceMember.ScopeId.Equals(parsedMessage.ScopeId) &&
                o.SequenceMember.SequenceGateId.Equals(parsedMessage.SequenceGateId)
            ).ToList();
        }

        public IEnumerable<SequenceObject> GetObjectsToAdd(ParsedMessage parsedMessage, List<string> idsToAdd)
        {
            return idsToAdd.Select(i => new SequenceObject
            {
                Id = i,
                SequenceAnchor = parsedMessage.SequenceAnchor,
            });
        }
    }
}
