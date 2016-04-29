using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateBehavior : IBehavior<IncomingContext>
    {
        private readonly SequenceGate _sequenceGate;

        public SequenceGateBehavior(SequenceGate sequenceGate)
        {
            _sequenceGate = sequenceGate;
        }

        public void Invoke(IncomingContext context, Action next)
        {
            object message = context.IncomingLogicalMessage.Instance;
            var result = _sequenceGate.Pass(message);

            if (result != default(object))
            {
                next();
            }
        }
    }
}
