using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequenceGateConfigurationTests
    {
        [Test]
        public void Validate_WithValidConfiguration_ReturnsEmptyString()
        {
            // Arrange
            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "UserEmailUpdated",
                    Messages = new List<SequenceGateMessageMetadata>
                    {
                        new SequenceGateMessageMetadata
                        {
                            MessageType = typeof (UserEmailUpdated),
                            ObjectIdPropertyName = "UserId",
                            TimeStampPropertyName = "TimeStamp"
                        }
                    }
                }
            };

            // Act
            var result = configuration.Validate();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
