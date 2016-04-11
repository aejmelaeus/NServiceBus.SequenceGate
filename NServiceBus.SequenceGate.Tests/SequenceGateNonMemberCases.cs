using NServiceBus.SequenceGate.Tests.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequenceGateNonMemberCases
    {
        [Test]
        public void Pass_WhenCalled_ReturnsTheMessage()
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
        public void Pass_WhenCalled_RepositoryNotCalled()
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
            repository.DidNotReceiveWithAnyArgs().ListSeenObjectIds(null);
            repository.DidNotReceiveWithAnyArgs().Register(null);
        }

        [Test]
        public void Pass_WhenCalled_ParserNotCalled()
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
            parser.DidNotReceiveWithAnyArgs().Parse(null, null);
        }
    }
}