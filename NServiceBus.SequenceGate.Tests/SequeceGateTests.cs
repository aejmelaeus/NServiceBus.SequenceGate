using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequeceGateTests
    {
        [Test]
        public void Pass_WithMessageParticipatingInGate_RegisterCalledOnRepository()
        {
            // Arrange
            const string sequenceGateId = "UserEmailUpdated";
            var message = new UserEmailUpdated();
            var repository = Substitute.For<IRepository>();
            var parser = Substitute.For<IParser>();
            var gateData = new List<GateData>();

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
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

            parser.Parse(message, configuration[0].Messages[0]).Returns(gateData);

            var sequenceGate = new SequenceGate(configuration, repository, parser);

            // Act
            sequenceGate.Pass(message);

            // Assert
            repository.Received().Register(gateData);
        }

        [Test]
        public void Pass_MessageParticipatingInGate_RepositoryCalledToListMessageStatusWithParsedData()
        {
            const string sequenceGateId = "UserEmailUpdated";
            var message = new UserEmailUpdated();
            var repository = Substitute.For<IRepository>();
            var parser = Substitute.For<IParser>();
            var gateData = new List<GateData>();

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = sequenceGateId,
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

            parser.Parse(message, configuration[0].Messages[0]).Returns(gateData);

            var sequenceGate = new SequenceGate(configuration, repository, parser);

            // Act
            sequenceGate.Pass(message);

            // Assert
            repository.Received().ListSeenObjectIds(gateData);
        }

        [Test]
        public void Pass_AllObjectsAreNewest_MutatorNotCalled()
        {
            // 
        }
    }
}