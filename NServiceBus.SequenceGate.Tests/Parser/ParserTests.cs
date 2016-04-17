using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Messages;

namespace NServiceBus.SequenceGate.Tests.Parser
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parse_WithSimpleObject_CorrectValues()
        {
            // Arrange
            const string sequenceGateId = "AFancyId";

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        new NServiceBus.SequenceGate.MessageMetadata
                        {
                            MessageType = typeof (SimpleMessage),
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

            Assert.That(parsed.SequenceGateId, Is.EqualTo(sequenceGateId));
            Assert.That(parsed.ObjectId, Is.EqualTo(objectId.ToString()));
            Assert.That(parsed.TimeStampUTC, Is.EqualTo(timeStamp));
            Assert.That(parsed.ScopeId, Is.EqualTo(scopeId));
        }
    }
}
