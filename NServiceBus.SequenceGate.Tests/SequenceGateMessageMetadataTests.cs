using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequenceGateMessageMetadataTests
    {
        [Test]
        public void Validate_WithValidMetadata_ReturnsEmptyString()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_WithInvalidObjectIdPropertyName_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetatdata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "WrongObjectIdProperty",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetatdata.Validate();

            // Assert
            var expectedResult = "Metadata for SimpleMessage is invalid. ObjectIdPropertyName WrongObjectIdProperty is missing";
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidTimeStampPropertyName_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "WrongTimeStampProperty"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = "Metadata for SimpleMessage is invalid. TimeStampPropertyName WrongTimeStampProperty is missing";
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidaTypeForTimestamp_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (TimeStampInWrongFormat),
                ObjectIdPropertyName = "Id",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = "Metadata for TimeStampInWrongFormat is invalid. Property for TimeStampPropertyName is not of type System.DateTime";
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WhenTimeStampIsInComplexType_ValidatedCorrectly()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (ComplexMetaDataMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_ObjectIdInComplexType_ValidateCorreclty()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (ComplexUserAndMetaDataMessage),
                ObjectIdPropertyName = "User.Id",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }

    public class ComplexType
    {
        public TheInnerType TheInnerType { get; set; }
    }

    public class TheInnerType
    {
        public string Id { get; set; }
    }
}
