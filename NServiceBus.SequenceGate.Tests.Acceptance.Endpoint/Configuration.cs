using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Endpoint
{
    public class Configuration : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            var sequenceGateConfiguration = new SequenceGateConfiguration
            {
                new SequenceGateMember
                {
                    Id = "UserEmailUpdated",
                    Messages = new List<MessageMetadata>
                    {
                        new MessageMetadata
                        {
                            ObjectIdPropertyName = nameof(UserEmailUpdated.UserId),
                            TimeStampPropertyName = nameof(UserEmailUpdated.TimeStampUtc)
                        }
                    }
                }
            };

            configuration.SequenceGate(sequenceGateConfiguration);
        }
    }
}
