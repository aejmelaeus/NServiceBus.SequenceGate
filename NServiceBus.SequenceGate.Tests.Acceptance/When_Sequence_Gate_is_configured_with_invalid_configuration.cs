using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_invalid_configuration
    {
        [Test]
        public void Then_bus_is_not_started()
        {
            var context = Scenario.Define(() => new Context())
                .WithEndpoint<Endpoint>()
                    .AllowExceptions()
                .Run();

            Assert.That(context.EndpointsStarted, Is.False);
        }

        [Test]
        public void Then_validation_error_is_logged()
        {
            
        }

        public class Context : ScenarioContext
        {
            // Nothing here...
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(busConfiguration =>
                {
                    busConfiguration.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("Endpoint").WithMember(member =>
                    {
                        member.Id = "Funky.Config";
                        member.HasSingleObjectMessage<Message>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = "This.Is.Just.Plain.Wrong";

                        });
                    });

                    busConfiguration.SequenceGate(configuration);
                });
            }
        }

        public class Message : IMessage
        {
            // Nothing here...
        }
    }
}
