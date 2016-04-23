using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.Tests.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class MutatorTests
    {
        [Test]
        public void Mutate_WithSimpleMessageThatIsNewest_MessageIsReturned()
        {
            // Arrange
            var objectId = Guid.NewGuid();

            var simpleMessage = new SimpleMessage();
            simpleMessage.ObjectId = objectId;

            var objectIdsToDismiss = new List<string>();

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "Test",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleMessage),
                            ObjectIdPropertyName = nameof(SimpleMessage.ObjectId),
                            ScopeIdPropertyName = nameof(SimpleMessage.ScopeId),
                            TimeStampPropertyName = nameof(SimpleMessage.TimeStamp)
                        }
                    }
                }
            };
            
            var mutator = new Mutator(configuration);

            // Act
            var result = mutator.Mutate(simpleMessage, objectIdsToDismiss);

            // Assert
            Assert.AreSame(simpleMessage, result);
        }

        [Test]
        public void Mutate_WithSimpleMessageThatIsNotNewest_NullIsReturned()
        {
            // Arrange
            var objectId = Guid.NewGuid();

            var simpleMessage = new SimpleMessage();
            simpleMessage.ObjectId = objectId;

            var objectIdsToDismiss = new List<string> { objectId.ToString() };

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "Test",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleMessage),
                            ObjectIdPropertyName = nameof(SimpleMessage.ObjectId),
                            ScopeIdPropertyName = nameof(SimpleMessage.ScopeId),
                            TimeStampPropertyName = nameof(SimpleMessage.TimeStamp)
                        }
                    }
                }
            };

            var mutator = new Mutator(configuration);

            // Act
            var result = mutator.Mutate(simpleMessage, objectIdsToDismiss);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Mutate_WithCollectionWithOneAlreadySeenObject_CorrectObjectDismissed()
        {
            // Arrange
            var alreadySeenUserId = Guid.NewGuid();
            var newestUserId = Guid.NewGuid();

            var objectIdsToDismiss = new List<string> { alreadySeenUserId.ToString() };

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "Test",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (ComplexCollectionMessage),
                            ObjectIdPropertyName = "Id",
                            ScopeIdPropertyName = "Scope.Id",
                            TimeStampPropertyName = "MetaData.TimeStamp",
                            CollectionPropertyName = "Users"
                        }
                    }
                }
            };

            var message = new ComplexCollectionMessage
            {
                Users = new List<User>
                {
                    new User { Id = alreadySeenUserId },
                    new User { Id = newestUserId }
                }
            };

            var mutator = new Mutator(configuration);

            // Act
            var result = mutator.Mutate(message, objectIdsToDismiss) as ComplexCollectionMessage;

            // Assert
            Assert.That(result.Users.Count, Is.EqualTo(1));
            Assert.That(result.Users.First().Id, Is.EqualTo(newestUserId));
        }
    }
}
