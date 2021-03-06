using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.MessageMetadata
{
    [TestFixture]
    public class TimeStampValidationTests
    {
        [Test]
        public void Validate_WithInvalidPropertyName_ReturnsError()
        {
            // Arrange
            var invalidMessageMetadata = new SingleObjectMessageMetadata
            {
                Type = typeof(SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "WrongTimeStampProperty"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert                                                    
            var expectedResult = ValidationError.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidType_ReturnsError()
        {
            // Arrange
            var invalidMessageMetadata = new SingleObjectMessageMetadata
            {
                Type = typeof(TimeStampInWrongFormat),
                ObjectIdPropertyName = "Id",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_PropertyInComplexType_PassesValidatio()
        {
            // Arrange
            var messageMetadata = new SingleObjectMessageMetadata
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