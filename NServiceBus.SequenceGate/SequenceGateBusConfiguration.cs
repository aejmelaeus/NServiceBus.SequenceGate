using System.Collections.Generic;

namespace NServiceBus.SequenceGate.Repository
{
    public static class SequenceGateBusConfiguration
    {
        public static BusConfiguration SequenceGate(this BusConfiguration busConfiguration, List<SequenceGateConfiguration> sequenceGateConfiguration)
        {
            return busConfiguration;
        }
    }
}
