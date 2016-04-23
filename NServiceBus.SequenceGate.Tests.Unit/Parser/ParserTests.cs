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

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleMessage),
                            ObjectIdPropertyName = "ObjectId",
                            TimeStampPropertyName = "TimeStamp",
                            ScopeIdPropertyName = "ScopeId"
                        }
                    }
                }
            };

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
            Assert.That(result.Count, Is.EqualTo(1));

            var parsed = result.First();
            var expectedSequenceAnchor = timeStamp.Ticks;

            Assert.That(parsed.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(parsed.ObjectId, Is.EqualTo(objectId.ToString()));
            Assert.That(parsed.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
            Assert.That(parsed.ScopeId, Is.EqualTo(scopeId));
        }

        [Test]
        public void Parse_WithComplexObject_CorrectValues()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (ComplexMessage),
                            ObjectIdPropertyName = "User.Id",
                            TimeStampPropertyName = "MetaData.TimeStamp",
                            ScopeIdPropertyName = "Scope.Id"
                        }
                    }
                }
            };

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
            Assert.That(result.Count, Is.EqualTo(1));

            var parsed = result.First();
            var expectedSequenceAnchor = timeStamp.Ticks;

            Assert.That(parsed.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(parsed.ObjectId, Is.EqualTo(objectId.ToString()));
            Assert.That(parsed.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
            Assert.That(parsed.ScopeId, Is.EqualTo(scopeId.ToString()));
        }

        [Test]
        public void Parse_WithComplexCollectionObject_ParsedCorrectly()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (ComplexCollectionMessage),
                            ObjectIdPropertyName = "Id",
                            TimeStampPropertyName = "MetaData.TimeStamp",
                            ScopeIdPropertyName = "Scope.Id",
                            CollectionPropertyName = "Users"
                        }
                    }
                }
            };

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
            Assert.That(result.Count, Is.EqualTo(2));
            var expectedSequenceAnchor = timeStamp.Ticks;

            var userId1Object = result.Single(r => r.ObjectId.Equals(userId1.ToString()));
            var userId2Object = result.Single(r => r.ObjectId.Equals(userId2.ToString()));

            Assert.That(userId1Object.ObjectId, Is.EqualTo(userId1.ToString()));
            Assert.That(userId1Object.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(userId1Object.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(userId1Object.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));

            Assert.That(userId2Object.ObjectId, Is.EqualTo(userId2.ToString()));
            Assert.That(userId2Object.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(userId2Object.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(userId2Object.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
        }

        [Test]
        public void Parse_WithSimpleCollectionObject_ParsedCorrectly()
        {
            // Arrange
            const string sequenceGateId = "SimpleMessage";

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            Type = typeof (SimpleCollectionMessage),
                            TimeStampPropertyName = "MetaData.TimeStamp",
                            ScopeIdPropertyName = "Scope.Id",
                            CollectionPropertyName = "UserIds"
                        }
                    }
                }
            };

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
            Assert.That(result.Count, Is.EqualTo(2));
            var expectedSequenceAnchor = timeStamp.Ticks;

            var userId1Object = result.Single(r => r.ObjectId.Equals(userId1.ToString()));
            var userId2Object = result.Single(r => r.ObjectId.Equals(userId2.ToString()));

            Assert.That(userId1Object.ObjectId, Is.EqualTo(userId1.ToString()));
            Assert.That(userId1Object.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(userId1Object.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(userId1Object.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));

            Assert.That(userId2Object.ObjectId, Is.EqualTo(userId2.ToString()));
            Assert.That(userId2Object.ScopeId, Is.EqualTo(scopeId.ToString()));
            Assert.That(userId2Object.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(userId2Object.SequenceAnchor, Is.EqualTo(expectedSequenceAnchor));
        }
    }
}
