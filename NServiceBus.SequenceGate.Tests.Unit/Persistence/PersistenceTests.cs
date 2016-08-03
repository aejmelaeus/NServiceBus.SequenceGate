using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.EntityFramework;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.Persistence
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void Register_WithNewObject_Added()
        {
            // Arrange
            var objects = new List<SequenceObject>();

            var objectId = Guid.NewGuid().ToString();

            var parsed = new ParsedMessage("SomeEndpoint", "SequenceGateId", "TheScopeId", DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, objects);

            // Assert
            Assert.That(result.ObjectIdsToAdd.Count, Is.EqualTo(1));
            Assert.That(result.ObjectIdsToAdd.Contains(objectId));
        }

        [Test]
        public void Register_WithExistingObject_NotInAdded()
        {
            // Arrange
            var objectId = Guid.NewGuid().ToString();

            var objects = new List<SequenceObject>();
           
            objects.Add(new SequenceObject { Id = objectId });

            var parsed = new ParsedMessage("EndpointName", "SequenceGateId", "ScopeId", DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, objects);

            // Assert
            Assert.That(result.ObjectIdsToAdd.Count, Is.EqualTo(0));
        }

        [Test]
        public void Register_WhenDatabaseContainsOlderAnchor_IdInIdsToUpdate()
        {
            // Arrange
            var olderAnchor = 123;
            var newerAnchor = 456;

            var objectId = Guid.NewGuid().ToString();

            var objects = new List<SequenceObject>();
            objects.Add(new SequenceObject { Id = objectId, SequenceAnchor = olderAnchor });

            var parsed = new ParsedMessage("EndpointName", "SequenceGateId", "ScopeId", newerAnchor);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, objects);

            // Assert
            Assert.That(result.ObjectIdsToUpdate.Count, Is.EqualTo(1));
            Assert.That(result.ObjectIdsToUpdate.Contains(objectId));
        }
        
        [Test]
        public void Register_WhenParsedMessageHasOlderAnchor_IdInIdsToDismiss()
        {
            // Arrange
            var olderAnchor = 123;
            var newerAnchor = 456;

            var objectId = Guid.NewGuid().ToString();

            var objects = new List<SequenceObject>();
            objects.Add(new SequenceObject { Id = objectId, SequenceAnchor = newerAnchor });

            var parsed = new ParsedMessage("EndpointName", "SequenceGateId", "ScopeId", olderAnchor);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, objects);

            // Assert
            Assert.That(result.ObjectIdsToDismiss.Count, Is.EqualTo(1));
            Assert.That(result.ObjectIdsToDismiss.Contains(objectId));
        }
        
        [Test]
        public void GetEntitiesToAdd_WhenInvoked_ParsedCorrectly()
        {
            // Arrange
            const string scopeId = "TheScope";
            const string endpointName = "TheEndpoint";
            const string sequenceGateId = "TheSequenceGateId";
            const string firstId = "123";
            const string secondId = "456";
            const string thirdId = "789";
            const long sequenceAnchor = 123;
            
            var parsed = new ParsedMessage(endpointName, sequenceGateId, scopeId, sequenceAnchor);
            
            var idsToAdd = new List<string> { firstId, secondId, thirdId };

            var persistence = new EntityFramework.Persistence();

            // Act
            var objects = persistence.GetObjectsToAdd(parsed, idsToAdd).ToList();

            // Assert
            Assert.That(objects.Count, Is.EqualTo(3));
            Assert.That(objects.All(o => o.SequenceAnchor.Equals(sequenceAnchor)));
            Assert.That(objects.Any(e => e.Id.Equals(firstId)));
            Assert.That(objects.Any(e => e.Id.Equals(secondId)));
            Assert.That(objects.Any(e => e.Id.Equals(thirdId)));
        }
    }
}
