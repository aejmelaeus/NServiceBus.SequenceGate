using System;
using System.Linq;

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

            var trackedObjects = _parser.Parse(message);

            _persistence.Register(trackedObjects);

            var objectsIdsToDismiss = _persistence.ListObjectIdsToDismiss(trackedObjects);

            if (objectsIdsToDismiss.Any())
            {
                message = _mutator.Mutate(message, objectsIdsToDismiss);
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
            var sequenceGateType = _configuration.SingleOrDefault(c => c.Messages.Any(m => m.Type == messageType));

            return sequenceGateType != default(SequenceGateMember) ? sequenceGateType.Id : string.Empty;
        }
    }
}
