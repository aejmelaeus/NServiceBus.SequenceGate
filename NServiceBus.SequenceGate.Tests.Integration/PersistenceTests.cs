using System;
using System.Linq;
using NServiceBus.SequenceGate.EntityFramework;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Integration
{
    [TestFixture]
    public class PersistenceTests
    {
        [Test]
        public void Register_WithUnseenObjects_PeristedCorrectly()
        {
            // Arrange
            var objectId = Guid.NewGuid().ToString();
            var sequenceGateId = Guid.NewGuid().ToString();

            var parsed = new Parsed("EndpointName", sequenceGateId, "NotApplicable", DateTime.UtcNow.Ticks);
            parsed.AddObjectId(objectId);

            var persistence = new Persistence();

            // Act
            persistence.Register(parsed);

            // Assert
            using (var context = new SequenceGateContext())
            {
                var trackedObject = context.TrackedObjects.Single(t =>
                    t.ObjectId.Equals(objectId) && t.SequenceGateId.Equals(sequenceGateId)
                );  

                Assert.That(trackedObject, Is.Not.Null);
            }
        }
    }
}
