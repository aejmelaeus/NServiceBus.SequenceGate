using System;
using NUnit.Framework;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_single_object_message
    {
        [Test]
        public void Then_an_older_message_for_the_same_object_is_discarded()
        {
            var userId = Guid.NewGuid();

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var newerMessage = new UserEmailUpdated { UserId = userId, EmailAdress = "new@email.com", TimeStamp = today };
            var olderMessage = new UserEmailUpdated { UserId = userId, EmailAdress = "old@email.com", TimeStamp = yesterday };

            var context = Scenario.Define(() => new Context { })
                .WithEndpoint<Endpoint>(producer => producer
                    .Given((b, c) =>
                    {
                        b.SendLocal(newerMessage);
                        b.SendLocal(olderMessage);
                    }))
                .Run();

            Assert.That(context.LastValue, Is.EqualTo(newerMessage.EmailAdress));
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

            public class Handler : IHandleMessages<UserEmailUpdated>
            {
                public Context Context { get; set; }

                public void Handle(UserEmailUpdated message)
                {
                    Context.LastValue = message.EmailAdress;
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
