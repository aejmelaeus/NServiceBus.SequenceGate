using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public class SequenceGate
    {
        private readonly SequenceGateConfiguration _configuration;

        public SequenceGate(SequenceGateConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool EntranceGranted(object message)
        {
            return true;
        }
    }
}
