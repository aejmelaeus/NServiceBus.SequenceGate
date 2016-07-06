using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.SequenceGate
{
    public class SequenceGateConfiguration
    {
        private readonly List<SequenceGateMember> _sequenceGateMembers = new List<SequenceGateMember>(); 

        public string EndpointName { get; }

        public IEnumerable<SequenceGateMember> SequenceGateMembers => _sequenceGateMembers.AsEnumerable();

        public SequenceGateConfiguration(string endpointName)
        {
            EndpointName = endpointName;
        }

        internal void Validate()
        {
            foreach (var sequenceGateMember in _sequenceGateMembers)
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
            return _sequenceGateMembers.SelectMany(m => m.Messages)
                                       .SingleOrDefault(m => m.Type == message.GetType());
        }

        public string GetSequenceGateIdForMessage(object message)
        {
            foreach (var sequenceGateMember in _sequenceGateMembers)
            {
                if (sequenceGateMember.Messages.Any(m => m.Type == message.GetType()))
                {
                    return sequenceGateMember.Id;
                }
            }
            return string.Empty;
        }

        public void AddMember(SequenceGateMember sequenceGateMember)
        {
            _sequenceGateMembers.Add(sequenceGateMember);
        }
    }

    public static class SequenceGateConfigurationExtensions
    {
        public static SequenceGateConfiguration WithMember(this SequenceGateConfiguration sequenceGateConfiguration, Action<SequenceGateMember> configureSequenceGateMember)
        {
            var sequenceGateMember = new SequenceGateMember();
            configureSequenceGateMember(sequenceGateMember);

            sequenceGateConfiguration.AddMember(sequenceGateMember);
            return sequenceGateConfiguration;
        }
    }
}
