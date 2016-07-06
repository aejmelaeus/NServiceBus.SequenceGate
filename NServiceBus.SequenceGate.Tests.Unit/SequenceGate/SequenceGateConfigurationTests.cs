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
            var configuration = new SequenceGateConfiguration("SomeEndpointName");

            configuration.WithMember(member =>
            {
                member.Id = "SomeSequenceGateId";
                member.HasMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = nameof(SimpleMessage.ObjectId);
                    metadata.TimeStampPropertyName = nameof(SimpleMessage.TimeStamp);
                });
            });

            // Act
            configuration.Validate();

            // Assert
            Assert.Pass("SequenceGateConfiguration.Validate did not throw any exceptions! Pass!");
        }
        
        [Test]
        public void Validate_WithInvalidConfiguration_ThrowsException()
        {
            // Arrange
            var configuration = new SequenceGateConfiguration("SomeEndpointName");

            configuration.WithMember(member =>
            {
                member.Id = "SomeSequenceGateId";
                member.HasMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "NonExistingPropertyName";
                    metadata.TimeStampPropertyName = nameof(SimpleMessage.TimeStamp);
                });
            });

            // Act & Assert
            Assert.Throws<ArgumentException>(() => configuration.Validate());
        }
    }
}
