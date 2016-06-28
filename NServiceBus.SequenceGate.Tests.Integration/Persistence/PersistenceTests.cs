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
            var result = persistence.Process(parsed, entities.AsQueryable());

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
            var result = persistence.Process(parsed, entities.AsQueryable());

            // Assert
            Assert.That(result.IdsToAdd.Count, Is.EqualTo(0));
        }
    }
}
