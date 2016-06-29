using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var entities = new List<SequenceObject>();

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

            var entities = new List<SequenceObject>();
            
            entities.Add(new SequenceObject { ObjectId = id });

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

            var entities = new List<SequenceObject>();

            entities.Add(new SequenceObject { ObjectId = id, SequenceAnchor = olderAnchor });

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

            var entities = new List<SequenceObject>();

            entities.Add(new SequenceObject { ObjectId = id, SequenceAnchor = newerAnchor });

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

            var entities = new List<SequenceObject>
            {
                new SequenceObject
                {
                    Id = 1,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new SequenceObject
                {
                    Id = 2,
                    ScopeId = scopeId,
                    EndpointName = "SomeRandomEndpointName",
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new SequenceObject
                {
                    Id = 3,
                    ScopeId = "SomeRandomScopeId",
                    EndpointName = endpointName,
                    SequenceGateId = "SomeRandomSequenceGateId"
                },
                new SequenceObject
                {
                    Id = 4,
                    ScopeId = scopeId,
                    EndpointName = endpointName,
                    SequenceGateId = sequenceGateId
                },
                new SequenceObject
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
            
            var parsed = new Parsed();
            parsed.EndpointName = endpointName;
            parsed.ScopeId = scopeId;
            parsed.SequenceAnchor = sequenceAnchor;
            parsed.SequenceGateId = sequenceGateId;

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
