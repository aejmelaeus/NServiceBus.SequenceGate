using System;
using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.SequenceGate
{
    [TestFixture]
    public class SequenceGateConfigurationTests
    {
        [Test]
        public void Validate_WhenInvokedWithCorrectConfiguration_ExceptionNotThrown()
        {
            // Arrange
            var configuration = new SequenceGateConfiguration("SomeEndpointName")
            {
                new SequenceGateMember
                {
                    Id = "SomeSequenceGateId",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleMessage),
                            ObjectIdPropertyName = nameof(SimpleMessage.ObjectId),
                            TimeStampPropertyName = nameof(SimpleMessage.TimeStamp)
                        }
                    }
                }
            };

            // Act
            configuration.Validate();

            // Assert
            Assert.Pass("SequenceGateConfiguration.Validate did not throw any exceptions! Pass!");
        }

        [Test]
        public void Validate_WithInvalidConfiguration_ThrowsException()
        {
            // Arrange
            var configuration = new SequenceGateConfiguration("SomeEndpointName")
            {
                new SequenceGateMember
                {
                    Id = "SomeSequenceGateId",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleMessage),
                            ObjectIdPropertyName = "NonExistingPropertyName",
                            TimeStampPropertyName = nameof(SimpleMessage.TimeStamp)
                        }
                    }
                }
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => configuration.Validate());
        }
    }
}
