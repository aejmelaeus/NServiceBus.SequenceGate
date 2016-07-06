using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.Mutator
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

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = "Test";
                member.WithMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = nameof(SimpleMessage.ObjectId);
                    metadata.ScopeIdPropertyName = nameof(SimpleMessage.ScopeId);
                    metadata.TimeStampPropertyName = nameof(SimpleMessage.TimeStamp);
                });
            });
            
            var mutator = new NServiceBus.SequenceGate.Mutator(configuration);

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

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = "Test";
                member.WithMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = nameof(SimpleMessage.ObjectId);
                    metadata.ScopeIdPropertyName = nameof(SimpleMessage.ScopeId);
                    metadata.TimeStampPropertyName = nameof(SimpleMessage.TimeStamp);
                });
            });

            var mutator = new NServiceBus.SequenceGate.Mutator(configuration);

            // Act
            var result = mutator.Mutate(simpleMessage, objectIdsToDismiss);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Mutate_WithComplexCollectionWithOneAlreadySeenObject_CorrectObjectDismissed()
        {
            // Arrange
            var alreadySeenUserId = Guid.NewGuid();
            var newestUserId = Guid.NewGuid();

            var objectIdsToDismiss = new List<string> { alreadySeenUserId.ToString() };

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = "Test";
                member.WithMessage<ComplexCollectionMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "Id";
                    metadata.ScopeIdPropertyName = "Scope.Id";
                    metadata.TimeStampPropertyName = "MetaData.TimeStamp";
                    metadata.CollectionPropertyName = "Users";
                });
            });
            
            var message = new ComplexCollectionMessage
            {
                Users = new List<User>
                {
                    new User { Id = alreadySeenUserId },
                    new User { Id = newestUserId }
                }
            };

            var mutator = new NServiceBus.SequenceGate.Mutator(configuration);

            // Act
            var result = mutator.Mutate(message, objectIdsToDismiss) as ComplexCollectionMessage;

            // Assert
            Assert.That(result, Is.SameAs(message));
            Assert.That(result.Users.Count, Is.EqualTo(1));
            Assert.That(result.Users.First().Id, Is.EqualTo(newestUserId));
        }

        [Test]
        public void Mutate_WithSimpleCollectionWithOneAlreadySeenObject_CorrectObjectDismissed()
        {
            // Arrange
            var alreadySeenUserId = Guid.NewGuid();
            var newestUserId = Guid.NewGuid();

            var objectIdsToDismiss = new List<string> { alreadySeenUserId.ToString() };

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = "Test";
                member.WithMessage<SimpleCollectionMessage>(metadata =>
                {
                    metadata.ScopeIdPropertyName = "Scope.Id";
                    metadata.TimeStampPropertyName = "MetaData.TimeStamp";
                    metadata.CollectionPropertyName = "UserIds";
                });
            });
            
            var message = new SimpleCollectionMessage
            {
                UserIds = new List<Guid>
                {
                    alreadySeenUserId,
                    newestUserId
                }
            };

            var mutator = new NServiceBus.SequenceGate.Mutator(configuration);

            // Act
            var result = mutator.Mutate(message, objectIdsToDismiss) as SimpleCollectionMessage;

            // Assert
            Assert.That(result, Is.SameAs(message));
            Assert.That(result.UserIds.Count, Is.EqualTo(1));
            Assert.That(result.UserIds.First(), Is.EqualTo(newestUserId));
        }
    }
}
