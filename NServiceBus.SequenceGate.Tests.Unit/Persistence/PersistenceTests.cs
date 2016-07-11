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
            var entities = new List<EntityFramework.TrackedObject>();

            var objectId = Guid.NewGuid().ToString();

            var parsed = new Parsed("SomeEndpoint", "SequenceGateId", "TheScopeId", DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities);

            // Assert
            Assert.That(result.ObjectIdsToAdd.Count, Is.EqualTo(1));
            Assert.That(result.ObjectIdsToAdd.Contains(objectId));
        }

        [Test]
        public void Register_WithExistingObject_NotInAdded()
        {
            // Arrange
            var objectId = Guid.NewGuid().ToString();

            var entities = new List<EntityFramework.TrackedObject>();
            
            entities.Add(new EntityFramework.TrackedObject { ObjectId = objectId });

            var parsed = new Parsed("EndpointName", "SequenceGateId", "ScopeId", DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities);

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
            var id = 789;

            var entities = new List<EntityFramework.TrackedObject>();

            entities.Add(new EntityFramework.TrackedObject { Id = id, ObjectId = objectId, SequenceAnchor = olderAnchor });

            var parsed = new Parsed("EndpointName", "SequenceGateId", "ScopeId", newerAnchor);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities);

            // Assert
            Assert.That(result.IdsToUpdate.Count, Is.EqualTo(1));
            Assert.That(result.IdsToUpdate.Contains(id));
        }
        
        [Test]
        public void Register_WhenParsedMessageHasOlderAnchor_IdInIdsToDismiss()
        {
            // Arrange
            var olderAnchor = 123;
            var newerAnchor = 456;

            var objectId = Guid.NewGuid().ToString();

            var entities = new List<EntityFramework.TrackedObject>();

            entities.Add(new EntityFramework.TrackedObject { ObjectId = objectId, SequenceAnchor = newerAnchor });

            var parsed = new Parsed("EndpointName", "SequenceGateId", "ScopeId", olderAnchor);
            parsed.AddObjectId(objectId);

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities);

            // Assert
            Assert.That(result.ObjectIdsToDismiss.Count, Is.EqualTo(1));
            Assert.That(result.ObjectIdsToDismiss.Contains(objectId));
        }
        
        [Test]
        public void GetScopedQuery_WithSpecificEndpointNameAndScopeId_ReturnsCorrectEntities()
        {
            // Arrange
            const string scopeId = "TheScope";
            const string endpointName = "TheEndpoint";
            const string sequenceGateId = "TheSequenceGateId";
            const string objectId = "123";

            var parsed = new Parsed(endpointName, sequenceGateId, scopeId, DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var entities = new List<EntityFramework.TrackedObject>
            {
                new EntityFramework.TrackedObject
                {
                    Id = 1,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new EntityFramework.TrackedObject
                {
                    Id = 2,
                    ScopeId = scopeId,
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new EntityFramework.TrackedObject
                {
                    Id = 3,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = endpointName,
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new EntityFramework.TrackedObject
                {
                    Id = 4,
                    ScopeId = scopeId,
                    EndpointName = endpointName,
                    SequenceGateId = sequenceGateId,
                    ObjectId = objectId
                },
                new EntityFramework.TrackedObject
                {
                    Id = 5,
                    ScopeId = scopeId,
                    EndpointName = endpointName,
                    SequenceGateId = "SomeRandomSequenceGateId"
                }
            };

            var persistence = new EntityFramework.Persistence();

            // Act
            var query = persistence.GetObjectsFromParsedMessage(parsed, entities.AsQueryable());

            // Assert
            Assert.That(query.Count(), Is.EqualTo(1));
            Assert.That(query.Any(e => e.Id.Equals(4)));
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
            
            var parsed = new Parsed(endpointName, sequenceGateId, scopeId, sequenceAnchor);
            
            var idsToAdd = new List<string> { firstId, secondId, thirdId };

            var persistence = new EntityFramework.Persistence();

            // Act
            var entities = persistence.GetEntitiesToAdd(parsed, idsToAdd).ToList();

            // Assert
            Assert.That(entities.All(e => e.EndpointName.Equals(endpointName)));
            Assert.That(entities.All(e => e.ScopeId.Equals(scopeId)));
            Assert.That(entities.All(e => e.SequenceAnchor.Equals(sequenceAnchor)));
            Assert.That(entities.All(e => e.SequenceGateId.Equals(sequenceGateId)));
            
            Assert.That(entities.Count, Is.EqualTo(3));
            Assert.That(entities.Any(e => e.ObjectId.Equals(firstId)));
            Assert.That(entities.Any(e => e.ObjectId.Equals(secondId)));
            Assert.That(entities.Any(e => e.ObjectId.Equals(thirdId)));
        }
    }
}
