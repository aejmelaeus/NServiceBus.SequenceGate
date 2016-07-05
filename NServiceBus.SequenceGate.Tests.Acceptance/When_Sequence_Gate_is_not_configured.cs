using System;
using NUnit.Framework;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_not_configured
    {
        [Test]
        public void Then_an_older_message_for_the_same_object_is_processed()
        {
            var id = Guid.NewGuid();

            var newerMessage = new Message { Id = id, Value = "Newer", TimeStamp = DateTime.UtcNow.AddDays(-1) };
            var olderMessage = new Message { Id = id, Value = "Older", TimeStamp = DateTime.UtcNow.AddDays(-2) };

            var context = Scenario.Define(() => new Context { })
                .WithEndpoint<Endpoint>(producer => producer
                    .Given((b, c) =>
                    {
                        b.SendLocal(newerMessage);
                        b.SendLocal(olderMessage);
                    }))
                .Run();

            Assert.That(context.LastValue, Is.EqualTo(olderMessage.Value));
        }

        public class Context : ScenarioContext
        {
            public string LastValue { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(config => config.EnableInstallers());
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