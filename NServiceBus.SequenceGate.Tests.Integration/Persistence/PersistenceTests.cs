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

        [Test]
        public void ListObjectIdsToDismiss_SomeObjectIdIsOlder_CorrectIdDismissed()
        {
            // Arrange
            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow.Ticks;

            var user1PersistedAnchor = now;
            var user2PersistedAnchor = now;

            var scopeId = Guid.NewGuid().ToString();
            var sequenceGateId = Guid.NewGuid().ToString();

            var trackedObject1 = new TrackedObject
            {
                ObjectId = userId1,
                ScopeId = scopeId,
                SequenceAnchor = user1PersistedAnchor,
                SequenceGateId = sequenceGateId
            };

            var trackedObject2 = new TrackedObject
            {
                ObjectId = userId2,
                ScopeId = scopeId,
                SequenceAnchor = user2PersistedAnchor,
                SequenceGateId = sequenceGateId
            };

            var persistedTrackedObjects = new List<TrackedObject>
            {
                trackedObject1,
                trackedObject2
            };

            var persistence = new EntityFramework.Persistence();

            persistence.Register(persistedTrackedObjects);

            var olderTrackedUserObject = new TrackedObject
            {
                ObjectId = userId1,
                SequenceGateId = sequenceGateId,
                ScopeId = scopeId,
                SequenceAnchor = now - 1000
            };

            var newerTrackedUserObject = new TrackedObject
            {
                ObjectId = userId2,
                SequenceGateId = sequenceGateId,
                ScopeId = scopeId,
                SequenceAnchor = now + 1000
            };

            var trackedObjects = new List<TrackedObject>
            {
                olderTrackedUserObject,
                newerTrackedUserObject
            };

            // Act
            var objectIdsToDismiss = persistence.ListObjectIdsToDismiss(trackedObjects);

            // Assert
            Assert.That(objectIdsToDismiss.Contains(userId1));
        }

        [Test]
        public void ListObjectIdsToDismiss_WithSameObjectInOtherScopeThatIsNewer_ScopeTakenIntoAccount()
        {
            // Arrange
            var sequenceGateId = Guid.NewGuid().ToString();
            var objectId = Guid.NewGuid().ToString();

            var scopeId = Guid.NewGuid().ToString();
            var otherScopeId = Guid.NewGuid().ToString();

            var sequenceAnchor = DateTime.UtcNow.Ticks;
            var otherSequenceAnchor = DateTime.UtcNow.Ticks + 1000;

            var trackedObjectInOtherScope = new TrackedObject
            {
                ObjectId = objectId,
                ScopeId = otherScopeId,
                SequenceGateId = sequenceGateId,
                SequenceAnchor = otherSequenceAnchor
            };

            var trackedObject = new TrackedObject
            {
                ObjectId = objectId,
                ScopeId = scopeId,
                SequenceGateId = sequenceGateId,
                SequenceAnchor = sequenceAnchor
            };

            var persistedTrackedObjects = new List<TrackedObject>
            {
                trackedObject,
                trackedObjectInOtherScope
            };

            var persistence = new EntityFramework.Persistence();
            persistence.Register(persistedTrackedObjects);

            var trackedObjects = new List<TrackedObject>
            {
                trackedObject
            };

            // Act
            var result = persistence.ListObjectIdsToDismiss(trackedObjects);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
