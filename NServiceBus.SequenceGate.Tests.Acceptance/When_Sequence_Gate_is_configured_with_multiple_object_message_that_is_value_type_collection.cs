using System;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_multiple_object_message_that_is_value_type_collection
    {
        [Test]
        public void Then_messages_are_parsed_correctly()
        {
            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var benkkeUserId = Guid.NewGuid();
            var bosseUserId = Guid.NewGuid();

            var benkke = new User
            {
                Id = benkkeUserId
            };

            var bosse = new User
            {
                Id = bosseUserId
            };

            var message = new UsersDeactivated
            {
                TimeStamp = today,
                UserIds = new List<Guid> { bosseUserId }
            };

            var retriedMessage = new UsersActivated
            {
                TimeStamp = yesterday,
                Users = new List<User> { benkke, bosse }
            };

            var context = Scenario.Define(() => new Context())
                .WithEndpoint<Endpoint>(endpoint => endpoint
                    .Given((b, c) =>
                    {
                        b.SendLocal(message);
                        b.SendLocal(retriedMessage);
                    }))
                .Run();

            Assert.That(context.ActiveUsers.Count, Is.EqualTo(1));
            Assert.That(context.ActiveUsers.Contains(benkkeUserId));
        }

        public class Context : ScenarioContext
        {
            public List<Guid> ActiveUsers { get; set; } = new List<Guid>();
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(busConfiguration =>
                {
                    busConfiguration.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
                    {
                        member.Id = "UserActivationActions";
                        member.HasMultipleObjectsMessage<UsersActivated>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(User.Id);
                            metadata.CollectionPropertyName = nameof(UsersActivated.Users);
                            metadata.TimeStampPropertyName = nameof(UsersActivated.TimeStamp);
                        });
                        member.HasMultipleObjectsMessage<UsersDeactivated>(metadata =>
                        {
                            metadata.CollectionPropertyName = nameof(UsersDeactivated.UserIds);
                            metadata.TimeStampPropertyName = nameof(UsersDeactivated.TimeStamp);
                        });
                    });

                    busConfiguration.SequenceGate(configuration);
                });
            }

            public class Handlers : IHandleMessages<UsersActivated>, IHandleMessages<UsersDeactivated>
            {
                public Context Context { get; set; }

                public void Handle(UsersActivated message)
                {
                    foreach (var user in message.Users)
                    {
                        Context.ActiveUsers.Add(user.Id);
                    }
                }

                public void Handle(UsersDeactivated message)
                {
                    foreach (var userId in message.UserIds)
                    {
                        Context.ActiveUsers.Remove(userId);
                    }
                }
            }
        }

        public class UsersActivated : IMessage
        {
            public List<User> Users { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public class UsersDeactivated : IMessage
        {
            public List<Guid> UserIds { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public class User
        {
            public Guid Id { get; set; }
        }
    }
}
