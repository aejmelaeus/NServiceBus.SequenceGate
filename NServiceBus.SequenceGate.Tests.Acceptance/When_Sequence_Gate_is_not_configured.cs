using System;
using NUnit.Framework;
using NServiceBus.AcceptanceTesting;
using NServiceBus.Config;
using NServiceBus.Features;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_not_configured
    {
        [Test]
        public void Retried_message_is_passed_through_even_that_a_newer_message_has_arrived()
        {
            var id = Guid.NewGuid();

            var context = Scenario.Define(() => new Context { })
                .WithEndpoint<Producer>(producer => producer
                    .Given((b, c) =>
                    {
                        b.Send(new Message { Id = id, Value = "First" });
                        b.Send(new Message { Id = id, Value = "Second" });
                    }))
                .WithEndpoint<NonGated>()
                .AllowExceptions()
                .Run(TimeSpan.FromSeconds(4));

            /*
            ** TODO: Disable FLR and set SLR to one attempt 2 seconds later.
            */

            Assert.That(context.LastValue, Is.EqualTo("First"));
        }

        public class Context : ScenarioContext
        {
            public bool FirstMessageFailed { get; set; }
            public string LastValue { get; set; }
        }

        public class Producer : EndpointConfigurationBuilder
        {
            public Producer()
            {
                EndpointSetup<DefaultServer>()
                    .AddMapping<Message>(typeof(NonGated));
            }
        }

        public class NonGated : EndpointConfigurationBuilder
        {
            public NonGated()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.EnableFeature<Features.TimeoutManager>();
                    config.EnableFeature<Features.SecondLevelRetries>();
                })
                .WithConfig<TransportConfig>(c =>
                {
                    c.MaxRetries = 0;
                })
                .WithConfig<SecondLevelRetriesConfig>(c =>
                {
                    c.NumberOfRetries = 1;
                    c.TimeIncrease = TimeSpan.FromSeconds(2);
                })
                .AddMapping<Message>(typeof(NonGated));
            }

            public class Handler : IHandleMessages<Message>
            {
                public Context Context { get; set; }

                public void Handle(Message message)
                {
                    if (!Context.FirstMessageFailed)
                    {
                        Context.FirstMessageFailed = true;
                        throw new Exception("Simulated exception");
                    }
                    Context.LastValue = message.Value;
                }
            }
        }

        public class Message : IMessage
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
        }
    }
}