using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.MessageMetadata
{
    [TestFixture]
    public class TimeStampValidationTests
    {
        [Test]
        public void Validate_WithInvalidPropertyName_ReturnsError()
        {
            // Arrange
            var invalidMessageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof(SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "WrongTimeStampProperty"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert                                                    
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidType_ReturnsError()
        {
            // Arrange
            var invalidMessageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof(TimeStampInWrongFormat),
                ObjectIdPropertyName = "Id",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_PropertyInComplexType_PassesValidatio()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof(ComplexMetaDataMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "MetaData.TimeStamp",
                ScopeIdPropertyName = "ScopeId"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}