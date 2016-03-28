using NServiceBus.Pipeline;

namespace NServiceBus.SequenceGate.Repository
{
    internal class SequenceGateRegistration : RegisterStep
    {
        public SequenceGateRegistration()
            : base("SequenceGateBehavior", typeof (SequenceGateBehavior), "Discards older messages")
        {
            InsertBefore(WellKnownStep.LoadHandlers);
        }
    }
}
