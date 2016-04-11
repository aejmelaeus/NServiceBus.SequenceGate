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

        internal SequenceGate(SequenceGateConfiguration configuration, IRepository repository, IParser parser)
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

            var messageMetadata = GetSequenceGateMember(messageType);
            var gateData = _parser.Parse(message, messageMetadata);
            _repository.Register(gateData);

            var seenObjectIds = _repository.ListSeenObjectIds(gateData);



            return message;
        }

        private string GetSequenceGateId(Type messageType)
        {
            var sequenceGateType = _configuration.SingleOrDefault(c => c.Messages.Any(m => m.MessageType == messageType));

            return sequenceGateType != default(SequenceGateMember) ? sequenceGateType.Id : string.Empty;
        }

        private SequenceGateMessageMetadata GetSequenceGateMember(Type messageType)
        {
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
