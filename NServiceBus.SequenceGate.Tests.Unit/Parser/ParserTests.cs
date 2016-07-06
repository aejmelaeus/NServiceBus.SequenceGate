using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.Parser
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_WithSimpleObject_CorrectValues()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "ObjectId";
                    metadata.TimeStampPropertyName = "TimeStamp";
                    metadata.ScopeIdPropertyName = "ScopeId";
                });
            });
            
            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            Guid objectId = Guid.NewGuid();
            DateTime timeStamp = DateTime.UtcNow;
            string scopeId = "AScopeId";

            var message = new SimpleMessage
            {
                ObjectId = objectId,
                TimeStamp = timeStamp,
                ScopeId = scopeId
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ObjectIds.Count, Is.EqualTo(1));

            var expectedSequenceAnchor = timeStamp.Ticks;

            string resultObjectId = result.ObjectIds.First();

            Assert.That(resultObjectId, Is.EqualTo(objectId.ToString()));
            Assert.That(result.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(result.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
            Assert.That(result.ScopeId, Is.EqualTo(scopeId));
        }

        [Test]
        public void Parse_WithComplexObject_CorrectValues()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<ComplexMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "User.Id";
                    metadata.TimeStampPropertyName = "MetaData.TimeStamp";
                    metadata.ScopeIdPropertyName = "Scope.Id";
                });
            });
            
            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            Guid objectId = Guid.NewGuid();
            DateTime timeStamp = DateTime.UtcNow;
            int scopeId = 123;

            var message = new ComplexMessage
            {
                User = new User { Id = objectId },
                MetaData = new MetaData { TimeStamp = timeStamp },
                Scope = new Scope { Id = scopeId }
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ObjectIds.Count, Is.EqualTo(1));

            var expectedSequenceAnchor = timeStamp.Ticks;
            string resultObjectId = result.ObjectIds.First();

            Assert.That(resultObjectId, Is.EqualTo(objectId.ToString()));
            Assert.That(result.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(result.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
            Assert.That(result.ScopeId, Is.EqualTo(scopeId.ToString()));
        }

        [Test]
        public void Parse_WithComplexCollectionObject_ParsedCorrectly()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<ComplexCollectionMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "Id";
                    metadata.TimeStampPropertyName = "MetaData.TimeStamp";
                    metadata.ScopeIdPropertyName = "Scope.Id";
                    metadata.CollectionPropertyName = "Users";
                });
            });
            
            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            DateTime timeStamp = DateTime.UtcNow;
            int scopeId = 123;

            var message = new ComplexCollectionMessage
            {
                MetaData = new MetaData { TimeStamp = timeStamp },
                Scope = new Scope { Id = scopeId },
                Users = new List<User>
                {
                    new User { Id = userId1 },
                    new User { Id = userId2 }
                }
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ObjectIds.Count, Is.EqualTo(2));
            var expectedSequenceAnchor = timeStamp.Ticks;

            var userId1ObjectId = result.ObjectIds.Single(r => r.Equals(userId1.ToString()));
            var userId2ObjectId = result.ObjectIds.Single(r => r.Equals(userId2.ToString()));

            Assert.That(userId1ObjectId, Is.EqualTo(userId1.ToString()));
            Assert.That(userId2ObjectId, Is.EqualTo(userId2.ToString()));

            Assert.That(result.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(result.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(result.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
        }

        [Test]
        public void Parse_WithSimpleCollectionObject_ParsedCorrectly()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<SimpleCollectionMessage>(metadata =>
                {
                    metadata.TimeStampPropertyName = "MetaData.TimeStamp";
                    metadata.ScopeIdPropertyName = "Scope.Id";
                    metadata.CollectionPropertyName = "UserIds";
                });
            });
            
            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            DateTime timeStamp = DateTime.UtcNow;
            int scopeId = 123;

            var message = new SimpleCollectionMessage
            {
                MetaData = new MetaData { TimeStamp = timeStamp },
                Scope = new Scope { Id = scopeId },
                UserIds = new List<Guid> { userId1, userId2 }
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ObjectIds.Count, Is.EqualTo(2));
            var expectedSequenceAnchor = timeStamp.Ticks;

            var userId1ObjectId = result.ObjectIds.Single(r => r.Equals(userId1.ToString()));
            var userId2ObjectId = result.ObjectIds.Single(r => r.Equals(userId2.ToString()));

            Assert.That(userId1ObjectId, Is.EqualTo(userId1.ToString()));
            Assert.That(userId2ObjectId, Is.EqualTo(userId2.ToString()));

            Assert.That(result.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(result.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(result.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
        }

        [Test]
        public void Parse_WhenScopeIdIsNullOrEmpty_ScopeIdSetToScopeIdNotApplicable()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration("SomeEndpointName").WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "ObjectId";
                    metadata.TimeStampPropertyName = "TimeStamp";
                });
            });
            
            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            Guid objectId = Guid.NewGuid();
            DateTime timeStamp = DateTime.UtcNow;

            var message = new SimpleMessage
            {
                ObjectId = objectId,
                TimeStamp = timeStamp
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ScopeId, Is.EqualTo(NServiceBus.SequenceGate.Parser.ScopeIdNotApplicable));
        }

        [Test]
        public void Parse_WhenCalled_EndpointNameTakenFromConfiguration()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";
            const string endpointName = "SomeNiceEndpointName";

            var configuration = new SequenceGateConfiguration(endpointName).WithMember(member =>
            {
                member.Id = sequenceGateId;
                member.WithMessage<SimpleMessage>(metadata =>
                {
                    metadata.ObjectIdPropertyName = "ObjectId";
                    metadata.TimeStampPropertyName = "TimeStamp";
                });
            });

            var parser = new NServiceBus.SequenceGate.Parser(configuration);

            Guid objectId = Guid.NewGuid();
            DateTime timeStamp = DateTime.UtcNow;

            var message = new SimpleMessage
            {
                ObjectId = objectId,
                TimeStamp = timeStamp
            };

            // Act
            var result = parser.Parse(message);

            // Assert
            Assert.That(result.ObjectIds.Count, Is.EqualTo(1));
            
            Assert.That(result.EndpointName, Is.EqualTo(endpointName));
        }
    }
}
