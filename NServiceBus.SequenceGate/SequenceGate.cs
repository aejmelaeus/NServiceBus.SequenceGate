using System;
using System.Linq;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public class SequenceGate
    {
        private readonly SequenceGateConfiguration _configuration;
        private readonly IRepository _repository;
        private readonly IParser _parser;
        private readonly IMutator _mutator;

        internal SequenceGate(SequenceGateConfiguration configuration, IRepository repository, IParser parser, IMutator mutator)
        {
            _configuration = configuration;
            _repository = repository;
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
            var gateData = _parser.Parse(message, messageMetadata);
            _repository.Register(gateData);

            var seenObjectIds = _repository.ListSeenObjectIds(gateData);

            if (seenObjectIds.Any())
            {
                message = _mutator.Mutate(message, seenObjectIds, messageMetadata);
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

        private SequenceGateMessageMetadata GetSequenceGateMember(object message)
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
