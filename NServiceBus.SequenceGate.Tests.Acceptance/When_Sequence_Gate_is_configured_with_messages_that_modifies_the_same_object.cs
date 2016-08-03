using System;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_messages_that_modifies_the_same_object
    {
        [Test]
        public void Then_both_of_the_messages_are_gating_the_same_object()
        {
            var userId = Guid.NewGuid();

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var newerMessage = new UserRemovedFromRole { UserId = userId, Role = "Administrator", TimeStamp = today };
            var olderFailedMessage = new UserAddedToRole { UserId = userId, Role = "Administrator", TimeStamp = yesterday };

            var context = Scenario.Define(() => new Context())
                .WithEndpoint<Endpoint>(producer => producer
                    .Given((b, c) =>
                    {
                        b.SendLocal(newerMessage);
                        b.SendLocal(olderFailedMessage);
                    }))
                .Run();

            Assert.That(context.RemovedRoles.Count, Is.EqualTo(1));
            Assert.That(context.AddedRoles.Count, Is.EqualTo(0));
        }

        public class Context : ScenarioContext
        {
            public List<string> RemovedRoles { get; set; } = new List<string>();
            public List<string> AddedRoles { get; set; } = new List<string>(); 
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
                        member.Id = "UserRoleActions";
                        member.HasSingleObjectMessage<UserAddedToRole>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(UserAddedToRole.UserId);
                            metadata.TimeStampPropertyName = nameof(UserAddedToRole.TimeStamp);
                        });
                        member.HasSingleObjectMessage<UserRemovedFromRole>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(UserRemovedFromRole.UserId);
                            metadata.TimeStampPropertyName = nameof(UserRemovedFromRole.TimeStamp);
                        });
                    });

                    busConfiguration.SequenceGate(configuration);
                });
            }

            public class Handler : IHandleMessages<UserAddedToRole>, IHandleMessages<UserRemovedFromRole>
            {
                public Context Context { get; set; }

                public void Handle(UserAddedToRole message)
                {
                    Context.AddedRoles.Add(message.Role);
                }

                public void Handle(UserRemovedFromRole message)
                {
                    Context.RemovedRoles.Add(message.Role);
                }
            }
        }

        public class UserAddedToRole : IMessage
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public class UserRemovedFromRole : IMessage
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
