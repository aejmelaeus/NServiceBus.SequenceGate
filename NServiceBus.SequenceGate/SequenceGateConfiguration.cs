using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateConfiguration : List<SequenceGateMember>
    {
        internal string Validate()
        {
            return string.Empty;
        }

        public MessageMetadata GetMessageMetadata(object message)
        {
            return this.SelectMany(m => m.Messages).SingleOrDefault(m => m.MessageType == message.GetType());
        }

        public string GetSequenceGateIdForMessage(object message)
        {
            foreach (var sequenceGateMember in this)
            {
                if (sequenceGateMember.Messages.Any(m => m.MessageType == message.GetType()))
                {
                    return sequenceGateMember.Id;
                }
            }
            return string.Empty;
        }
    }
}
