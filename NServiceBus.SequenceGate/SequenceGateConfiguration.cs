using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateConfiguration : List<SequenceGateMember>
    {
        public string EndpointName { get; }

        public SequenceGateConfiguration(string endpointName)
        {
            EndpointName = endpointName;
        }

        internal void Validate()
        {
            foreach (var sequenceGateMember in this)
            {
                var result = sequenceGateMember.Validate();

                foreach (var item in result)
                {
                    foreach (var validationError in item.Value)
                    {
                        throw new ArgumentException($"Sequence Gate Configuration error => {item.Key}: {validationError}");
                    }
                }
            }
        }

        public MessageMetadata GetMessageMetadata(object message)
        {
            return this.SelectMany(m => m.Messages).SingleOrDefault(m => m.Type == message.GetType());
        }

        public string GetSequenceGateIdForMessage(object message)
        {
            foreach (var sequenceGateMember in this)
            {
                if (sequenceGateMember.Messages.Any(m => m.Type == message.GetType()))
                {
                    return sequenceGateMember.Id;
                }
            }
            return string.Empty;
        }
    }
}
