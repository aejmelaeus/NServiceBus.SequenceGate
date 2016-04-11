using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequenceGateMemberCases
    {
        private IRepository _repository;
        private IParser _parser;
        private IMutator _mutator;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IRepository>();
            _parser = Substitute.For<IParser>();
            _mutator = Substitute.For<IMutator>();
        }

        [Test]
        public void Pass_WhenCalled_RegisterCalledOnRepository()
        {
            // Arrange
            const string sequenceGateId = "UserEmailUpdated";
            var message = new UserEmailUpdated();
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

            _parser.Parse(message, configuration[0].Messages[0]).Returns(gateData);

            var sequenceGate = new SequenceGate(configuration, _repository, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _repository.Received().Register(gateData);
        }

        [Test]
        public void Pass_WhenCalled_RepositoryCalledToListMessageStatusWithParsedData()
        {
            const string sequenceGateId = "UserEmailUpdated";
            var message = new UserEmailUpdated();
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

            _parser.Parse(message, configuration[0].Messages[0]).Returns(gateData);

            var sequenceGate = new SequenceGate(configuration, _repository, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _repository.Received().ListSeenObjectIds(gateData);
        }

        [Test]
        public void Pass_AllObjectsAreNewest_MutatorNotCalled()
        {
            const string sequenceGateId = "UserEmailUpdated";
            var message = new UserEmailUpdated();
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

            _parser.Parse(message, configuration[0].Messages[0]).Returns(gateData);

            var sequenceGate = new SequenceGate(configuration, _repository, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _mutator.DidNotReceiveWithAnyArgs().Mutate(null, null, null);
        }

        [Test]
        public void Pass_SomeObjectAlreadySeen_MutatedObjectReturned()
        {
            // Arrange
            -

            // Act

            // Assert
        }
    }
}