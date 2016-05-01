using System;
using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.MessageMetadata
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
                Type = typeof(CollectionMessage),
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
                Type = typeof(CollectionMessage),
                CollectionPropertyName = "WrongCollection",
                ObjectIdPropertyName = "Id",
                ScopeIdPropertyName = "Scope.Id",
                TimeStampPropertyName = "MetaData.TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_WithWrongCollectionType_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof(WrongCollectionTypeMessage),
                CollectionPropertyName = "CollectionThatIsAString",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.CollectionPropertyMissingOrNotICollection;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_ObjectIdDoesNotExistOnCollectionObject_CorrectErrorMessage()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof(CollectionMessage),
                CollectionPropertyName = "Users",
                ObjectIdPropertyName = "WrongObjectId",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.ObjectIdPropertyMissingOnObjectInCollection;

            Assert.That(result.Contains(expectedResult));
        }
        
        [Test]
        public void Validate_ObjectIdIsEmptyAndCollectionGiven_ValidTypesPassed()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof (GuidCollectionMessage),
                CollectionPropertyName = "Items",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Validate_ObjectIdIsEmptyAndCollectionIsListOfDateTimes_ValidationFails()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                CollectionPropertyName = nameof(DateTimeCollectionMessage.Items),
                Type = typeof (DateTimeCollectionMessage),
                TimeStampPropertyName = nameof(DateTimeCollectionMessage.TimeStamp)
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.CollectionObjectTypeNotInAllowedBasicCollectionTypes;

            Assert.That(result.Contains(expectedResult));
        }
    }

    internal class GuidCollectionMessage
    {
        public DateTime TimeStamp { get; set; }
        public List<Guid> Items { get; set; }
    }

    internal class DateTimeCollectionMessage
    {
        public DateTime TimeStamp { get; set; }
        public List<DateTime> Items { get; set; }
    }
}