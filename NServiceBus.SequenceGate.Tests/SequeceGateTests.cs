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
            var message = new Message();
            var sequenceGate = new SequenceGate();

            // Act
            var result = sequenceGate.EntranceGranted(message);

            // Assert
            Assert.That(result, Is.True);
        }
    }

    public class Message
    {
        // Nothing here...
    }
}
