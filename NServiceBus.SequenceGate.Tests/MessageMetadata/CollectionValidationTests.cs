using System;
using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.MessageMetadata
{
    [TestFixture]
    public class CollectionValidationTests
    {
        [Test]
        public void Validate_WhenCorrectCollectionPropertyIsPresent_ValidationPasses()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(CollectionMessage),
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
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
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
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithWrongCollectionType_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(WrongCollectionTypeMessage),
                CollectionPropertyName = "CollectionThatIsAString",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_ObjectIdDoesNotExistOnCollectionObject_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                MessageType = typeof(CollectionMessage),
                CollectionPropertyName = "Users",
                ObjectIdPropertyName = "WrongObjectId",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = NServiceBus.SequenceGate.MessageMetadata.ValidationErrors.ObjectIdPropertyMissingOnObjectInCollection;

            Assert.That(result.Contains(expectedResult));
        }
        
        [TestCase(typeof(string), true)]
        [TestCase(typeof(Guid), true)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(long), true)]
        [TestCase(typeof(DateTime), false)]
        [TestCase(typeof(User), false)]
        public void Validate_ObjectIdIsEmptyAndCollectionGiven_ValidTypesPassed(Type type, bool valid)
        {
            // Arrange

            // Act

            // Assert
        }
    }
}