using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateConfiguration : List<SequenceGateMember>
    {
        internal string Validate()
        {
            return string.Empty;
        }
    }
}
