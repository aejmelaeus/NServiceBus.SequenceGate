using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.SequenceGate
{
    [TestFixture]
    public class SequenceGateTests
    {
        private IPersistence _persistence;
        private IParser _parser;
        private IMutator _mutator;
        private SimpleMessage _simpleMessage;
        private SequenceGateConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _persistence = Substitute.For<IPersistence>();
            _parser = Substitute.For<IParser>();
            _mutator = Substitute.For<IMutator>();
            _simpleMessage = new SimpleMessage();
            _configuration = new SequenceGateConfiguration();
        }

        [Test]
        public void Pass_WhenCalled_RegisterCalledOnRepository()
        {
            // Arrange
            const string sequenceGateId = "UserEmailUpdated";
            var message = new SimpleMessage();
            var gateData = new List<TrackedObject>();

            _persistence.ListObjectIdsToDismiss(Arg.Any<List<TrackedObject>>()).Returns(new List<string>());

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
                            ObjectIdPropertyName = "UserId",
                            TimeStampPropertyName = "TimeStamp"
                        }
                    }
                }
            };

            _parser.Parse(message).Returns(gateData);

            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _persistence.Received().Register(gateData);
        }

        [Test]
        public void Pass_WhenCalled_RepositoryCalledToListMessageStatusWithParsedData()
        {
            const string sequenceGateId = "UserEmailUpdated";
            var message = new SimpleMessage();
            var gateData = new List<TrackedObject>();

            _persistence.ListObjectIdsToDismiss(Arg.Any<List<TrackedObject>>()).Returns(new List<string>());

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
                            ObjectIdPropertyName = "UserId",
                            TimeStampPropertyName = "TimeStamp"
                        }
                    }
                }
            };

            _parser.Parse(message).Returns(gateData);

            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _persistence.Received().ListObjectIdsToDismiss(gateData);
        }

        [Test]
        public void Pass_AllObjectsAreNewest_MutatorNotCalled()
        {
            const string sequenceGateId = "UserEmailUpdated";
            var message = new SimpleMessage();
            var gateData = new List<TrackedObject>();

            _persistence.ListObjectIdsToDismiss(Arg.Any<List<TrackedObject>>()).Returns(new List<string>());

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
                            ObjectIdPropertyName = "UserId",
                            TimeStampPropertyName = "TimeStamp"
                        }
                    }
                }
            };

            _parser.Parse(message).Returns(gateData);

            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(message);

            // Assert
            _mutator.DidNotReceiveWithAnyArgs().Mutate(null, null);
        }

        [Test]
        public void Pass_SomeObjectAlreadySeen_MutatedObjectReturned()
        {
            // Arrange
            var originalObject = new SimpleMessage();
            var seenObjects = new List<string> { "ASeenId" };

            var metadata = new NServiceBus.SequenceGate.MessageMetadata
            {
                Type = typeof (SimpleMessage),
                ObjectIdPropertyName = "UserId",
                TimeStampPropertyName = "TimeStamp"
            };

            _persistence.ListObjectIdsToDismiss(Arg.Any<List<TrackedObject>>()).Returns(seenObjects);

            var mutatedObject = new SimpleMessage();
            _mutator.Mutate(originalObject, seenObjects).Returns(mutatedObject);

            var configuration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "abc123",
                    Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                    {
                        metadata
                    }
                }
            };

            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(configuration, _persistence, _parser, _mutator);

            // Act
            var result = sequenceGate.Pass(originalObject);

            // Assert
            Assert.That(result, Is.SameAs(mutatedObject));
        }

        [Test]
        public void Pass_WhenCalled_ReturnsTheMessage()
        {
            // Arrange
            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            var result = sequenceGate.Pass(_simpleMessage);

            // Assert
            Assert.That(result, Is.SameAs(_simpleMessage));
        }

        [Test]
        public void Pass_WhenCalled_RepositoryNotCalled()
        {
            // Arrange
            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(_simpleMessage);

            // Assert
            _persistence.DidNotReceiveWithAnyArgs().ListObjectIdsToDismiss(null);
            _persistence.DidNotReceiveWithAnyArgs().Register(null);
        }

        [Test]
        public void Pass_WhenCalled_ParserNotCalled()
        {
            // Arrange
            var sequenceGate = new NServiceBus.SequenceGate.SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(_simpleMessage);

            // Assert
            _parser.DidNotReceiveWithAnyArgs().Parse(null);
        }
    }
}