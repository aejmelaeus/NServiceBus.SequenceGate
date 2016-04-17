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
            var messageMetadata = new MessageMetadata
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
            var invalidMessageMetatdata = new MessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "WrongObjectIdProperty",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetatdata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.ObjectIdPropertyMissing;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithInvalidTimeStampPropertyName_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetadata = new MessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "WrongTimeStampProperty"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert                                                    
            var expectedResult = MessageMetadata.ValidationErrors.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_AllPropertiesAreNull_ReturnsErrorsInsteadOfException()
        {
            // Arrange
            var nullMessage = new MessageMetadata();

            // Act
            nullMessage.Validate();

            Assert.Pass("Should not crash with uninitialized properties");
        }

        [Test]
        public void Validate_WithInvalidaTypeForTimestamp_ReturnsErrorString()
        {
            // Arrange
            var invalidMessageMetadata = new MessageMetadata
            {
                MessageType = typeof (TimeStampInWrongFormat),
                ObjectIdPropertyName = "Id",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.TimeStampPropertyMissingOrNotDateTime;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_MessageTypeIsNull_CorrectErrorMessageReturned()
        {
            // Arrange
            var messageMetadata = new MessageMetadata();

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.MessageTypeMissing;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WhenTimeStampIsInComplexType_ValidatedCorrectly()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
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
            var messageMetadata = new MessageMetadata
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
            var messageMetadata = new MessageMetadata
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
            var messageMetadata = new MessageMetadata
            {
                MessageType = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "WrongScope"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.ScopeIdPropertyMissing;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_ScopeIdIsNullOrEmpty_Valid()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
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

        [Test]
        public void Validate_WhenCorrectCollectionPropertyIsPresent_ValidationPasses()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
            {
                MessageType = typeof (CollectionMessage),
                CollectionPropertyName = "Users",
                ObjectIdPropertyName = "Id",
                ScopeIdPropertyName = "Scope.Id",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_WithWrongCollectionName_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
            {
                MessageType = typeof(CollectionMessage),
                CollectionPropertyName = "WrongCollection",
                ObjectIdPropertyName = "Id",
                ScopeIdPropertyName = "Scope.Id",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithWrongCollectionType_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
            {
                MessageType = typeof(WrongCollectionTypeMessage),
                CollectionPropertyName = "CollectionThatIsAString",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_ObjectIdDoesNotExistOnCollectionObject_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new MessageMetadata
            {
                MessageType = typeof (CollectionMessage),
                CollectionPropertyName = "Users",
                ObjectIdPropertyName = "WrongObjectId",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = MessageMetadata.ValidationErrors.ObjectIdPropertyMissingOnObjectInCollection;

            Assert.That(result.Contains(expectedResult));
        }
    }
}
