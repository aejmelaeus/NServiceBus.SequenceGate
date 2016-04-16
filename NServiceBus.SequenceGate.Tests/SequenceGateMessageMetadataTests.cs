using System;
using System.Runtime.CompilerServices;
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
                MessageType = typeof (UserEmailUpdated),
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
                MessageType = typeof (UserEmailUpdated),
                ObjectIdPropertyName = "WrongObjectIdProperty"
            };

            // Act
            var result = invalidMessageMetatdata.Validate();

            // Assert
            var expectedResult = "Metadata for UserEmailUpdated is invalid. ObjectIdPropertyName WrongObjectIdProperty is missing";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidTimeStampPropertyName_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (UserEmailUpdated),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "WrongTimeStampProperty"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = "Metadata for UserEmailUpdated is invalid. TimeStampPropertyName WrongTimeStampProperty is missing";
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private class TimeStampInWrongFormat
        {
            public string Id { get; set; }
            public string TimeStamp { get; set; }
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
            Assert.That(result, Is.EqualTo(expectedResult));
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
