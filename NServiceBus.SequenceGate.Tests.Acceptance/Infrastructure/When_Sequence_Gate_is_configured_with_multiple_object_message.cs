using System;
using System.Collections.Generic;
using NServiceBus.AcceptanceTesting;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_multiple_object_message
    {
        [Test]
        public void Then_each_object_in_the_message_is_expected_to_be_sequences_separetely()
        {
            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);
            var tomorrow = today.AddDays(1);

            const string groupName = "AReallyRandomGroup";

            var bosseUserId = Guid.NewGuid();
            var lasseUserId = Guid.NewGuid();

            var bosse = new User
            {
                Firstname = "Bosse",
                Lastname = "Larsson",
                Id = bosseUserId
            };

            var lasse = new User
            {
                Firstname = "Lasse",
                Lastname = "Bengtsson",
                Id = lasseUserId
            };

            var retriedMessage = new UsersAddedToGroup
            {
                Users = new List<User> { lasse, bosse },
                Group = groupName,
                Metadata = new Metadata { TimeStamp = yesterday }
            };

            var message = new UsersRemovedFromGroup
            {
                Users = new List<User> { bosse },
                Group = groupName,
                Metadata = new Metadata { TimeStamp = today }
            };

            var context = Scenario.Define(() => new Context())
                .WithEndpoint<Endpoint>(endpoint => endpoint
                    .When(c => c.EndpointsStarted, b =>
                    {
                        b.SendLocal(message);
                        b.SendLocal(retriedMessage);
                        b.SendLocal(new DoneMessage());
                    }))
                .Done(c => c.Done)
                .Run();

            var count = context.UsersInGroup.Count;
            var users = context.UsersInGroup;

            Assert.That(count, Is.EqualTo(1));
            Assert.That(users.ContainsKey(lasseUserId));
        }

        public class Context : ScenarioContext
        {
            public Dictionary<Guid, string> UsersInGroup { get; set; } = new Dictionary<Guid, string>();
            public bool Done { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(config =>
                {
                    config.PurgeOnStartup(true);
                    config.EnableInstallers();

                    var configuration = new SequenceGateConfiguration("EndpointName").WithMember(member =>
                    {
                        member.Id = "UserGroupActions";
                        member.HasMultipleObjectsMessage<UsersAddedToGroup>(metadata =>
                        {
                            metadata.CollectionPropertyName = nameof(UsersAddedToGroup.Users);
                            metadata.ObjectIdPropertyName = nameof(User.Id);
                            metadata.TimeStampPropertyName = "Metadata.TimeStamp";
                        });
                        member.HasMultipleObjectsMessage<UsersRemovedFromGroup>(metadata =>
                        {
                            metadata.CollectionPropertyName = nameof(UsersRemovedFromGroup.Users);
                            metadata.ObjectIdPropertyName = nameof(User.Id);
                            metadata.TimeStampPropertyName = "Metadata.TimeStamp";
                        });
                    });

                    config.SequenceGate(configuration);
                });
            }

            public class Handlers : IHandleMessages<UsersAddedToGroup>, 
                IHandleMessages<UsersRemovedFromGroup>,
                IHandleMessages<DoneMessage>
            {
                public Context Context { get; set; }

                public void Handle(UsersAddedToGroup message)
                {
                    foreach (var user in message.Users)
                    {
                        Context.UsersInGroup.Add(user.Id, message.Group);
                    }
                }

                public void Handle(UsersRemovedFromGroup message)
                {
                    foreach (var user in message.Users)
                    {
                        Context.UsersInGroup.Remove(user.Id);
                    }
                }

                public void Handle(DoneMessage message)
                {
                    Context.Done = true;
                }
            }
        }

        public class DoneMessage : IMessage
        {
            // Nothing here...
        }

        public class UsersAddedToGroup : IMessage
        {
            public string Group { get; set; }
            public List<User> Users { get; set; }
            public Metadata Metadata { get; set; }
        }

        public class UsersRemovedFromGroup : IMessage
        {
            public string Group { get; set; }
            public List<User> Users { get; set; }
            public Metadata Metadata { get; set; }
        }

        public class User
        {
            public Guid Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
        }

        public class Metadata
        {
            public DateTime TimeStamp { get; set; }
            public Guid LoggedInUserId { get; set; }
        }
    }
}
