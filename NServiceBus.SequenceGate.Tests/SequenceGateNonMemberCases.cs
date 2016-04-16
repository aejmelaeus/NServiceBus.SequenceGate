using NServiceBus.SequenceGate.Tests.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests
{
    [TestFixture]
    public class SequenceGateNonMemberCases
    {
        private IPersistence _persistence;
        private IParser _parser;
        private IMutator _mutator;
        private UserEmailUpdated _message;
        private SequenceGateConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _persistence = Substitute.For<IPersistence>();
            _parser = Substitute.For<IParser>();
            _mutator = Substitute.For<IMutator>();
            _message = new UserEmailUpdated();
            _configuration = new SequenceGateConfiguration();
        }

        [Test]
        public void Pass_WhenCalled_ReturnsTheMessage()
        {
            // Arrange
            var sequenceGate = new SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            var result = sequenceGate.Pass(_message);

            // Assert
            Assert.That(result, Is.SameAs(_message));
        }

        [Test]
        public void Pass_WhenCalled_RepositoryNotCalled()
        {
            // Arrange
            var sequenceGate = new SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(_message);

            // Assert
            _persistence.DidNotReceiveWithAnyArgs().ListAlreadySeenTrackedObjectIds(null);
            _persistence.DidNotReceiveWithAnyArgs().Register(null);
        }

        [Test]
        public void Pass_WhenCalled_ParserNotCalled()
        {
            // Arrange
            var sequenceGate = new SequenceGate(_configuration, _persistence, _parser, _mutator);

            // Act
            sequenceGate.Pass(_message);

            // Assert
            _parser.DidNotReceiveWithAnyArgs().Parse(null, null);
        }
    }
}