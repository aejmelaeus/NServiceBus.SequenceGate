using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.MessageMetadata
{
    [TestFixture]
    public class ObjectIdValidationTests
    {
        [Test]
        public void Validate_WithInvalidPropertyName_ReturnsError()
        {
            // Arrange
            var invalidMessageMetatdata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(SimpleMessage),
                ObjectIdPropertyName = "WrongObjectIdProperty",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetatdata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.ObjectIdPropertyMissing;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_InComplexType_ValidateCorreclty()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(ComplexMessage),
                ObjectIdPropertyName = "User.Id",
                TimeStampPropertyName = "MetaData.TimeStamp",
                ScopeIdPropertyName = "Scope.Id"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}