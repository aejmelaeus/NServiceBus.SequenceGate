using System.Security.Cryptography.X509Certificates;
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
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "ScopeId"
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
            var expectedResult = "Metadata for SimpleMessage is invalid. ObjectIdProperty: WrongObjectIdProperty is missing";
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
            var expectedResult = "Metadata for SimpleMessage is invalid. TimeStampProperty: WrongTimeStampProperty is missing or not of type System.DateTime";
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_AllPropertiesAreNull_ReturnsErrorsInsteadOfException()
        {
            // Arrange
            var nullMessage = new SequenceGateMessageMetadata();

            // Act
            nullMessage.Validate();

            Assert.Pass("Should not crash with uninitialized properties");
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
            var expectedResult = "Metadata for TimeStampInWrongFormat is invalid. TimeStampProperty: TimeStamp is missing or not of type System.DateTime";
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_MessageTypeIsNull_CorrectErrorMessageReturned()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata();

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result.Contains("MessageType missing."));
        }

        [Test]
        public void Validate_WhenTimeStampIsInComplexType_ValidatedCorrectly()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (ComplexMetaDataMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "MetaData.TimeStamp",
                ScopeIdPropertyName = "ScopeId"
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
                MessageType = typeof (ComplexMessage),
                ObjectIdPropertyName = "User.Id",
                TimeStampPropertyName = "MetaData.TimeStamp",
                ScopeIdPropertyName = "Scope.Id"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_MessageWithCorrectScope_ValidatesOK()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (ComplexMessage),
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
        public void Validate_GivenScopePropertyDoesNotExist_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "WrongScope"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result.Contains("Metadata for SimpleMessage is invalid. ScopeIdProperty: WrongScope is missing"));
        }

        [Test]
        public void Validate_ScopeIdIsNullOrEmpty_Valid()
        {
            // Arrange
            var messageMetadata = new SequenceGateMessageMetadata
            {
                MessageType = typeof (SimpleMessage),
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
