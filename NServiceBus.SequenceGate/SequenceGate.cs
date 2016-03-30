using System;
using System.Linq;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public class SequenceGate
    {
        private readonly SequenceGateConfiguration _configuration;
        private readonly ISequenceGateRepository _repository;
        private readonly ISequenceGateParser _parser;

        public SequenceGate(SequenceGateConfiguration configuration, ISequenceGateRepository repository, ISequenceGateParser parser)
        {
            _configuration = configuration;
            _repository = repository;
            _parser = parser;
        }

        public object Pass(object message)
        {
            var messageType = message.GetType();

            var sequenceGateId = GetSequenceGateId(messageType);

            if (string.IsNullOrEmpty(sequenceGateId))
            {
                return message;
            }

            var member = GetSequenceGateMember(messageType);
            var query = _parser.Parse(message, member);
            _repository.Register(sequenceGateId, query);

            return message;
        }

        private string GetSequenceGateId(Type messageType)
        {
            var sequenceGateType = _configuration.SingleOrDefault(c => c.Members.Any(m => m.MessageType == messageType));

            return sequenceGateType != default(SequenceGateType) ? sequenceGateType.Id : string.Empty;
        }

        private SequenceGateMember GetSequenceGateMember(Type messageType)
        {
            var sequenceGateType = _configuration.SingleOrDefault(c => c.Members.Any(m => m.MessageType == messageType));

            if (sequenceGateType != default(SequenceGateType))
            {
                var sequenceGateMember = sequenceGateType.Members.SingleOrDefault(m => m.MessageType == messageType);
                return sequenceGateMember;
            }
            return null;
        }
    }
}
