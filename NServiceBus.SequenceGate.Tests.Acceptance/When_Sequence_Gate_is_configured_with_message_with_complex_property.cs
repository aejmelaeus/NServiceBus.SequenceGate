using System;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_message_with_complex_property
    {
        [Test]
        public void Then_property_is_parsed_correctly()
        {
            var userId = Guid.NewGuid();

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var newerMessage = new UserEmailUpdated
            {
                UserId = userId,
                EmailAdress = "new@email.com",
                Metadata = new Metadata
                {
                    TimeStamp = today
                }
            };

            var failedRetiredMessage = new UserEmailUpdated
            {
                UserId = userId,
                EmailAdress = "old@email.com",
                Metadata = new Metadata
                {
                    TimeStamp = yesterday
                } 
            };

            var context = Scenario.Define(() => new Context { })
                .WithEndpoint<Endpoint>(producer => producer
                    .Given((b, c) =>
                    {
                        b.SendLocal(newerMessage);
                        b.SendLocal(failedRetiredMessage);
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
                            metadata.TimeStampPropertyName = "Metadata.TimeStamp";
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
            public Metadata Metadata { get; set; }
        }

        public class Metadata
        {
            public DateTime TimeStamp { get; set; }
            public Guid LoggedInUserId { get; set; }
        }
    }

}
