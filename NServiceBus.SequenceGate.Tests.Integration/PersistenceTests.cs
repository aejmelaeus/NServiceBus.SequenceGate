using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.EntityFramework;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Integration
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void AddEntities_WithUnseenMember_MemberAdded()
        {
            // Arrange
            string endpointName = Guid.NewGuid().ToString();
            string sequenceGateId = Guid.NewGuid().ToString();
            string scopeId = Guid.NewGuid().ToString();
            string objectId1 = Guid.NewGuid().ToString();
            string objectId2 = Guid.NewGuid().ToString();

            long sequenceAnchor = 123;

            var parsedMessage = new ParsedMessage(endpointName, sequenceGateId, scopeId, sequenceAnchor);

            var persistence = new Persistence();
            
            // Assert
            using (var context = new SequenceGateContext())
            {
                persistence.AddEntities(context, parsedMessage, new List<string> { objectId1, objectId2 });
                context.SaveChanges();
            }

            // Act
            using (var context = new SequenceGateContext())
            {
                var sequenceObject = context.SequenceObjects.SingleOrDefault(so => so.Id.Equals(objectId1));

                var memberSequenceGateId = sequenceObject.SequenceMember.SequenceGateId;

                Assert.That(memberSequenceGateId, Is.EqualTo(sequenceGateId));
            }
        }

        [Test]
        public void AddEntities_WithAlreadySeenMember_MemberAttachedFromDb()
        {
            // Arrange
            string endpointName = Guid.NewGuid().ToString();
            string sequenceGateId = Guid.NewGuid().ToString();
            string scopeId = Guid.NewGuid().ToString();

            long sequenceAnchor = 123;

            var seqenceMember = new SequenceMember
            {
                EndpointName = endpointName,
                ScopeId = scopeId,
                SequenceGateId = sequenceGateId
            };

            using (var context = new SequenceGateContext())
            {
                context.SequenceMembers.Add(seqenceMember);
                context.SaveChanges();
            }

            string objectId = Guid.NewGuid().ToString();
            var parsedMessage = new ParsedMessage(endpointName, sequenceGateId, scopeId, sequenceAnchor);
            var persistence = new Persistence();

            // Act
            using (var context = new SequenceGateContext())
            {
                persistence.AddEntities(context, parsedMessage, new List<string> { objectId });
                context.SaveChanges();
            }

            // Assert
            using (var context = new SequenceGateContext())
            {
                var sequenceObject = context.SequenceObjects.SingleOrDefault(so => so.Id.Equals(objectId));

                var memberSequenceGateId = sequenceObject.SequenceMember.SequenceGateId;

                Assert.That(memberSequenceGateId, Is.EqualTo(sequenceGateId));
            }
        }
    }
}
