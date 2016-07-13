using System;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_used_by_two_Endpoints
    {
        [Test]
        public void Then_the_same_message_is_gated_separately_for_each_Endpoint()
        {
            /*
            ** By changing the Endpoint names to the same, the test fails.
            * - Why not have that in a test...
            */

            var userId = Guid.NewGuid();

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var firstEndpointMessage = new UserEmailUpdated
            {
                UserId = userId,
                EmailAdress = "first.endpoint@email.com",
                TimeStamp = today
            };
            var secondEndpointMessage = new UserEmailUpdated
            {
                UserId = userId,
                EmailAdress = "second.endpoint@email.com",
                TimeStamp = yesterday
            };

            var context = Scenario.Define(() => new Context())
                .WithEndpoint<FirstEndpoint>(endpoint => endpoint
                    .Given((b, c) =>
                    {
                        b.SendLocal(firstEndpointMessage);
                    }))
                .WithEndpoint<SecondEndpoint>(endpoint => endpoint
                    .When(c => c.FirstEndpointProcessedMessage, b =>
                    {
                        b.SendLocal(secondEndpointMessage);
                    }))
                .Done(c => c.SecondEndpointProcessdedMessage)
                .Run(TimeSpan.FromSeconds(5));

            Assert.That(context.FirstEndpointEmailAdress, Is.EqualTo(firstEndpointMessage.EmailAdress));
            Assert.That(context.SecondEndpointEmailAdress, Is.EqualTo(secondEndpointMessage.EmailAdress));
        }

        public class Context : ScenarioContext
        {
            public bool FirstEndpointProcessedMessage { get; set; }
            public bool SecondEndpointProcessdedMessage { get; set; }
            public string FirstEndpointEmailAdress { get; set; }
            public string SecondEndpointEmailAdress { get; set; }
            
        }

        public class FirstEndpoint : EndpointConfigurationBuilder
        {
            public FirstEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("FirstEndpoint").WithMember(member =>
                    {
                        member.Id = "UserEmailUpdated";
                        member.HasSingleObjectMessage<UserEmailUpdated>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(UserEmailUpdated.UserId);
                            metadata.TimeStampPropertyName = nameof(UserEmailUpdated.TimeStamp);
                        });
                    });

                    config.SequenceGate(configuration);
                });
            }

            public class UserEmailUpdatedHandler : IHandleMessages<UserEmailUpdated>
            {
                public Context Context { get; set; }

                public void Handle(UserEmailUpdated message)
                {
                    Context.FirstEndpointProcessedMessage = true;
                    Context.FirstEndpointEmailAdress = message.EmailAdress;
                }
            }
        }

        public class SecondEndpoint : EndpointConfigurationBuilder
        {
            public SecondEndpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("SecondEndpoint").WithMember(member =>
                    {
                        member.Id = "UserEmailUpdated";
                        member.HasSingleObjectMessage<UserEmailUpdated>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(UserEmailUpdated.UserId);
                            metadata.TimeStampPropertyName = nameof(UserEmailUpdated.TimeStamp);
                        });
                    });

                    config.SequenceGate(configuration);
                });
            }

            public class UserEmailUpdatedHandler : IHandleMessages<UserEmailUpdated>
            {
                public Context Context { get; set; }

                public void Handle(UserEmailUpdated message) 
                {
                    Context.SecondEndpointEmailAdress = message.EmailAdress;
                    Context.SecondEndpointProcessdedMessage = true;
                }
            }
        }

        public class UserEmailUpdated : IMessage
        {
            public Guid UserId { get; set; }
            public string EmailAdress { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
