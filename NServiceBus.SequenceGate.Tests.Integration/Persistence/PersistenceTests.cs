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
        public void Register_WhenInvoked_ItemPersistedWithCorrectValues()
        {
            // Arrange
            string objectId = Guid.NewGuid().ToString();
            const string sequenceGateId = "SomethingSequential";
            const string scopeId = "AScopeId";
            DateTime timestampUtc = DateTime.UtcNow;

            var trackedObject = new TrackedObject
            {
                ObjectId = objectId,
                ScopeId = scopeId,
                SequenceGateId = sequenceGateId,
                SequenceAnchor = timestampUtc.Ticks
            };

            var trackedObjects = new List<TrackedObject> { trackedObject };

            var persistence = new EntityFramework.Persistence();

            // Act
            persistence.Register(trackedObjects);

            // Assert
            using (var context = new TrackedObjectsContext())
            {
                var trackedObjectEntity = context.TrackedObjectEntities.SingleOrDefault(to => to.ObjectId.Equals(objectId));

                long actual = trackedObjectEntity.SequenceAnchor;
                long expected = timestampUtc.Ticks;

                Assert.That(trackedObjectEntity.ObjectId, Is.EqualTo(objectId));
                Assert.That(trackedObjectEntity.ScopeId, Is.EqualTo(scopeId));
                Assert.That(trackedObjectEntity.SequenceGateId, Is.EqualTo(sequenceGateId));
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
