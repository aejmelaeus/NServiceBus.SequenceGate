using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    public interface IMutator
    {
        object Mutate(object message, IEnumerable<string> objectIdsToDismiss);
    }
}
