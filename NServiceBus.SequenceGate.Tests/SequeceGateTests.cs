using System.Collections.Generic;
using NServiceBus.SequenceGate.Repository;
using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequeceGateTests
    {
        [Test]
        public void EntranceGranted_WithMessageNotParticipatingInGate_ReturnsTrue()
        {
            // Arrange
            var message = new UserEmailUpdated();
            var configuration = new SequenceGateConfiguration();
            var sequenceGate = new SequenceGate(configuration);

            // Act
            var result = sequenceGate.EntranceGranted(message);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EntranceGranted_WithNewestMessage_ReturnsTrue()
        {
            // Arrange
            var message = new UserEmailUpdated();

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateType
                {
                    Id = "UserEmailUpdated",
                    Members = new List<SequenceGateMember>
                    {
                        new SequenceGateMember
                        {
                            MessageType = typeof (UserEmailUpdated),
                            ObjectIdPropertyName = "UserId",
                            TimeStampPropertyName = "TimeStamp"
                        }
                    }
                }
            };

            var sequenceGate = new SequenceGate(configuration);

            // Act
            var result = sequenceGate.EntranceGranted(message);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}