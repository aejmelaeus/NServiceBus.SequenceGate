using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.SequenceGate.Repository
{
    public class SequenceGateBehaviour : IBehavior<IncomingContext>
    {
        public void Invoke(IncomingContext context, Action next)
        {
            throw new NotImplementedException();
        }
    }
}
