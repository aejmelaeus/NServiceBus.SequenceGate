using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.MessageMetadata
{
    [TestFixture]
    public class ObjectIdValidationTests
    {
        [Test]
        public void Validate_WithInvalidPropertyName_ReturnsError()
        {
            // Arrange
            var invalidMessageMetatdata = new SingleObjectMessageMetadata
            {
                Type = typeof(SimpleMessage),
                ObjectIdPropertyName = "WrongObjectIdProperty",
                TimeStampPropertyName = "TimeStamp"
            };

            // Act
            var result = invalidMessageMetatdata.Validate();

            // Assert
            var expectedResult = ValidationError.ObjectIdPropertyMissing;
            Assert.That(result.Contains(expectedResult));
        }

        [Test]
        public void Validate_InComplexType_ValidateCorreclty()
        {
            // Arrange
            var messageMetadata = new SingleObjectMessageMetadata
            {
                Type = typeof(ComplexMessage),
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