using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    public static class SequenceGateBusConfiguration
    {
        public static BusConfiguration SequenceGate(this BusConfiguration busConfiguration, SequenceGateConfiguration sequenceGateConfiguration)
        {
            return busConfiguration;
        }
    }
}
