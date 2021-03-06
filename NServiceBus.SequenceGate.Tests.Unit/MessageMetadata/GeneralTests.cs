﻿using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.MessageMetadata
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public void Validate_WithValidMetadata_ReturnsEmptyValidationResult()
        {
            // Arrange
            var messageMetadata = new SingleObjectMessageMetadata
            {
                Type = typeof (SimpleMessage),
                ObjectIdPropertyName = "ObjectId",
                TimeStampPropertyName = "TimeStamp",
                ScopeIdPropertyName = "ScopeId"
            };

            // Act
            var result = messageMetadata.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        } 

        [Test]
        public void Validate_AllPropertiesAreNull_ReturnsErrorsInsteadOfException()
        {
            // Arrange
            var nullMessage = new NServiceBus.SequenceGate.SingleObjectMessageMetadata();

            // Act
            nullMessage.Validate();

            // Assert
            Assert.Pass("Should not crash with uninitialized properties");
        }

        [Test]
        public void Validate_MessageTypeIsNull_CorrectErrorMessageReturned()
        {
            // Arrange
            var messageMetadata = new NServiceBus.SequenceGate.MultipleObjectMessageMetadata();

            // Act
            var result = messageMetadata.Validate();

            // Assert
            var expectedResult = ValidationError.MessageTypeMissing;

            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void MessageType_WhenMetadataIsSingleType_SingleIsReturned()
        {
            // Arrange
            var messageMetadata = new SingleObjectMessageMetadata();

            // Assert
            Assert.That(messageMetadata.MessageType, Is.EqualTo(NServiceBus.SequenceGate.MessageMetadata.MessageTypes.Single));
        }

        [Test]
        public void MessageType_WhenCollectionAndObjectIdPropertySet_ComplexCollectionIsReturned()
        {
            // Arrange
            var messageMetadata = new MultipleObjectMessageMetadata();
            messageMetadata.CollectionPropertyName = "Collectional";
            messageMetadata.ObjectIdPropertyName = "Objective";

            // Assert
            Assert.That(messageMetadata.MessageType, Is.EqualTo(NServiceBus.SequenceGate.MessageMetadata.MessageTypes.ComplexCollection));
        }

        [Test]
        public void MessageType_WhenCollectionIsSetAndObjectIdIsNull_PrimitiveCollectionIsReturned()
        {
            // Arrange
            var messageMetadata = new MultipleObjectMessageMetadata();
            messageMetadata.CollectionPropertyName = "Collectional";
            messageMetadata.ObjectIdPropertyName = null;

            // Assert
            Assert.That(messageMetadata.MessageType, Is.EqualTo(NServiceBus.SequenceGate.MessageMetadata.MessageTypes.PrimitiveCollection));
        }
    }
}
