namespace NServiceBus.SequenceGate
{
    public class SequenceGate
    {
        public bool EntranceGranted(object message)
        {
            return true;
        }
    }
}
