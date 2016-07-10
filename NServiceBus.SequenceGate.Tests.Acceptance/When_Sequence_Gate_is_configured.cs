using System;
using NUnit.Framework;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured
    {
        [Test]
        public void Then_an_older_message_for_the_same_object_is_discarded()
        {
            var id = Guid.NewGuid();

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var newerMessage = new Message { Id = id, Value = "Newer", TimeStamp = today };
            var olderMessage = new Message { Id = id, Value = "Older", TimeStamp = yesterday };

            var context = Scenario.Define(() => new Context { })
                .WithEndpoint<Endpoint>(producer => producer
                    .Given((b, c) =>
                    {
                        b.SendLocal(newerMessage);
                        b.SendLocal(olderMessage);
                    }))
                .Run();

            Assert.That(context.LastValue, Is.EqualTo(newerMessage.Value));
        }

        public class Context : ScenarioContext
        {
            public string LastValue { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("Endpoint").WithMember(member =>
                    {
                        member.Id = "Message";
                        member.HasSingleObjectMessage<Message>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(Message.Id);
                            metadata.TimeStampPropertyName = nameof(Message.TimeStamp);
                        });
                    });
                    
                    config.SequenceGate(configuration);
                });
            }

            public class Handler : IHandleMessages<Message>
            {
                public Context Context { get; set; }

                public void Handle(Message message)
                {
                    Context.LastValue = message.Value;
                }
            }
        }

        public class Message : IMessage
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
