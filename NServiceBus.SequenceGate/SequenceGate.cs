using System;
using System.Linq;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public class SequenceGate
    {
        private readonly SequenceGateConfiguration _configuration;
        private readonly IPersistence _persistence;
        private readonly IParser _parser;
        private readonly IMutator _mutator;

        internal SequenceGate(SequenceGateConfiguration configuration, IPersistence persistence, IParser parser, IMutator mutator)
        {
            _configuration = configuration;
            _persistence = persistence;
            _parser = parser;
            _mutator = mutator;
        }

        public object Pass(object message)
        {
            if (!MessageIsParticipatingInGate(message))
            {
                return message;
            }

            var messageMetadata = GetSequenceGateMember(message);
            var trackedObjects = _parser.Parse(message);
            _persistence.Register(trackedObjects);

            var alreadyHandledObjectIds = _persistence.ListObjectIdsToDismiss(trackedObjects);

            if (alreadyHandledObjectIds.Any())
            {
                message = _mutator.Mutate(message, alreadyHandledObjectIds, messageMetadata);
            }

            return message;
        }

        private bool MessageIsParticipatingInGate(object message)
        {
            var messageType = message.GetType();

            var sequenceGateId = GetSequenceGateId(messageType);

            return !string.IsNullOrEmpty(sequenceGateId);
        }

        private string GetSequenceGateId(Type messageType)
        {
            var sequenceGateType = _configuration.SingleOrDefault(c => c.Messages.Any(m => m.MessageType == messageType));

            return sequenceGateType != default(SequenceGateMember) ? sequenceGateType.Id : string.Empty;
        }

        private MessageMetadata GetSequenceGateMember(object message)
        {
            var messageType = message.GetType();

            var sequenceGateType = _configuration.SingleOrDefault(c => c.Messages.Any(m => m.MessageType == messageType));

            if (sequenceGateType != default(SequenceGateMember))
            {
                var sequenceGateMember = sequenceGateType.Messages.SingleOrDefault(m => m.MessageType == messageType);
                return sequenceGateMember;
            }
            return null;
        }
    }
}
