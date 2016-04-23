using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.SequenceGate
{
    [TestFixture]
    public class SequenceGateNonMemberCases
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