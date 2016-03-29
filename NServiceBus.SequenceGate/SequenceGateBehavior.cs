using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateBehavior : IBehavior<IncomingContext>
    {
        public void Invoke(IncomingContext context, Action next)
        {
            // Here the magic will happen!
        }
    }
}
