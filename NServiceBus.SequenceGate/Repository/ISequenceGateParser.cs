namespace NServiceBus.SequenceGate.Repository
{
    public interface ISequenceGateParser
    {
        SequenceGateQuery Parse(object message, SequenceGateMember member);
    }
}