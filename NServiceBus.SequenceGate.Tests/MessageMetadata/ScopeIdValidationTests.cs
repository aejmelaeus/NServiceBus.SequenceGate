using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.MessageMetadata
{
    [TestFixture]
    public class ScopeIdValidationTests
    {
        [Test]
        public void Validate_CorrectScope_ValidatesOK()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(ComplexMessage),
                ObjectIdPropertyName = "User.Id",
                ScopeIdPropertyName = "Scope.Id",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_GivenPropertyDoesNotExist_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "WrongScope"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.ScopeIdPropertyMissing;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_IsNullOrEmpty_Valid()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = string.Empty
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}