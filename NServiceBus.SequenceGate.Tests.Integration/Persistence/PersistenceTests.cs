using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.EntityFramework;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Persistence
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void Register_WithNewObject_Added()
        {
            // Arrange
            var entities = new List<TrackedObjectEntity>();

            var id = Guid.NewGuid().ToString();

            var parsed = new Parsed();
            parsed.EndpointName = "TheName";
            parsed.ScopeId = "TheScopeId";
            parsed.SequenceAnchor = 123;
            parsed.ObjectIds = new List<string> { id };

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities.AsQueryable());

            // Assert
            Assert.That(result.IdsToAdd.Count, Is.EqualTo(1));
            Assert.That(result.IdsToAdd.Contains(id));
        }

        [Test]
        public void Register_WithExistingObject_NotInAdded()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            var entities = new List<TrackedObjectEntity>();
            
            entities.Add(new TrackedObjectEntity { ObjectId = id });

            var parsed = new Parsed();
            parsed.EndpointName = "TheName";
            parsed.ScopeId = "TheScopeId";
            parsed.SequenceAnchor = 123;
            parsed.ObjectIds = new List<string> { id };

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities.AsQueryable());

            // Assert
            Assert.That(result.IdsToAdd.Count, Is.EqualTo(0));
        }

        [Test]
        public void Register_WhenDatabaseContainsOlderAnchor_IdInIdsToUpdate()
        {
            // Arrange
            var olderAnchor = 123;
            var newerAnchor = 456;

            var id = Guid.NewGuid().ToString();

            var entities = new List<TrackedObjectEntity>();

            entities.Add(new TrackedObjectEntity { ObjectId = id, SequenceAnchor = olderAnchor });

            var parsed = new Parsed();
            parsed.EndpointName = "TheName";
            parsed.ScopeId = "TheScopeId";
            parsed.SequenceAnchor = newerAnchor;
            parsed.ObjectIds = new List<string> { id };

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities.AsQueryable());

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

            var id = Guid.NewGuid().ToString();

            var entities = new List<TrackedObjectEntity>();

            entities.Add(new TrackedObjectEntity { ObjectId = id, SequenceAnchor = newerAnchor });

            var parsed = new Parsed();
            parsed.EndpointName = "TheName";
            parsed.ScopeId = "TheScopeId";
            parsed.SequenceAnchor = olderAnchor;
            parsed.ObjectIds = new List<string> { id };

            var persistence = new EntityFramework.Persistence();

            // Act
            var result = persistence.GetActions(parsed, entities.AsQueryable());

            // Assert
            Assert.That(result.IdsToDismiss.Count, Is.EqualTo(1));
            Assert.That(result.IdsToDismiss.Contains(id));
        }

        [Test]
        public void GetScopedQuery_WithSpecificEndpointNameAndScopeId_ReturnsCorrectEntities()
        {
            // Arrange
            const string scopeId = "TheScope";
            const string endpointName = "TheEndpoint";
            const string sequenceGateId = "TheSequenceGateId";

            var parsed = new Parsed();
            parsed.EndpointName = endpointName;
            parsed.ScopeId = scopeId;
            parsed.SequenceGateId = sequenceGateId;

            var entities = new List<TrackedObjectEntity>
            {
                new TrackedObjectEntity
                {
                    Id = 1,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new TrackedObjectEntity
                {
                    Id = 2,
                    ScopeId = scopeId,
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new TrackedObjectEntity
                {
                    Id = 3,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = endpointName,
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new TrackedObjectEntity
                {
                    Id = 4,
                    ScopeId = scopeId,
                    EndpointName = endpointName,
                    SequenceGateId = sequenceGateId
                },
                new TrackedObjectEntity
                {
                    Id = 5,
                    ScopeId = scopeId,
                    EndpointName = endpointName,
                    SequenceGateId = "SomeRandomSequenceGateId"
                }
            };

            var persistence = new EntityFramework.Persistence();

            // Act
            var query = persistence.GetQuery(parsed, entities.AsQueryable());

            // Assert
            Assert.That(query.Count(), Is.EqualTo(1));
            Assert.That(query.Any(e => e.Id.Equals(4)));
        }
    }
}
