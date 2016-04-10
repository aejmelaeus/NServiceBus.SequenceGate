using System.Collections.Generic;
using NServiceBus.SequenceGate.Repository;
using NServiceBus.SequenceGate.Tests.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequeceGateTests
    {
        [Test]
        public void Pass_WithMessageNotParticipatingInGate_ReturnsTheMessage()
        {
            // Arrange
            var message = new UserEmailUpdated();
            var repository = Substitute.For<IRepository>();
            var parser = Substitute.For<IParser>();
            var configuration = new SequenceGateConfiguration();
            var sequenceGate = new SequenceGate(configuration, repository, parser);

            // Act
            var result = sequenceGate.Pass(message);

            // Assert
            Assert.That(result, Is.SameAs(message));
        }

        [Test]
        public void Pass_WithMessageNotParticipatingInGate_RepositoryNotCalled()
        {
            // Arrange
            var message = new UserEmailUpdated();
            var repository = Substitute.For<IRepository>();
            var parser = Substitute.For<IParser>();
            var configuration = new SequenceGateConfiguration();
            var sequenceGate = new SequenceGate(configuration, repository, parser);

            // Act
            sequenceGate.Pass(message);

            // Assert
            repository.DidNotReceiveWithAnyArgs().ListObjectIdsWithNewerDates(null);
            repository.DidNotReceiveWithAnyArgs().Register(null);
        }

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
                    Messages = new List<SequenceGateMessage>
                    {
                        new SequenceGateMessage
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
    }
}