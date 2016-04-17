using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.MessageMetadata
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public void Validate_WithValidMetadata_ReturnsEmptyValidationResult()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "ScopeId"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        } 

        [Test]
        public void Validate_AllPropertiesAreNull_ReturnsErrorsInsteadOfException()
        {
            // Arrange
            var nullMessage = new NServiceBus.SequenceGate.MessageMetadata();

            // Act
            nullMessage.Validate();

            // Assert
            Assert.Pass("Should not crash with uninitialized properties");
        }

        [Test]
        public void Validate_MessageTypeIsNull_CorrectErrorMessageReturned()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata();

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.MessageTypeMissing;

            Assert.That(result.Contains(expectedResult));
        }
    }
}
