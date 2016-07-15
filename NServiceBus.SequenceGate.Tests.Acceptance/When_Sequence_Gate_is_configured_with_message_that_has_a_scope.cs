using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_Sequence_Gate_is_configured_with_message_that_has_a_scope
    {
        [Test]
        public void Then_scope_is_taken_into_account_when_passing_messages_through_gate()
        {
            Guid userId = Guid.NewGuid();
            const string firstClientId = "abc123";
            const string otherClientId = "def456";

            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);

            var firstClientMessage = new UserRoleUpdatedOnClient
            {
                ClientId = firstClientId,
                Role = "AMagicRole",
                TimeStamp = today,
                UserId = userId
            };

            var otherClientMessage = new UserRoleUpdatedOnClient
            {
                ClientId = otherClientId,
                Role = "SomeOrdinaryRole",
                TimeStamp = yesterday,
                UserId = userId
            };

            var context = Scenario.Define(() => new Context())
                .WithEndpoint<Endpoint>(endpoint => endpoint
                    .Given((b, c) =>
                    {
                        b.SendLocal(firstClientMessage);
                        b.SendLocal(otherClientMessage);
                    }))
                .Run();

            Assert.That(context.ClientIdsWithUpdatedUserRoles.Count, Is.EqualTo(2));
            Assert.That(context.ClientIdsWithUpdatedUserRoles.Contains(firstClientId));
            Assert.That(context.ClientIdsWithUpdatedUserRoles.Contains(otherClientId));
        }

        public class Context : ScenarioContext
        {
            public List<string> ClientIdsWithUpdatedUserRoles { get; set; } = new List<string>();
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
                        member.Id = "UserRoleUpdatedOnClient";
                        member.HasSingleObjectMessage<UserRoleUpdatedOnClient>(metadata =>
                        {
                            metadata.ObjectIdPropertyName = nameof(UserRoleUpdatedOnClient.UserId);
                            metadata.ScopeIdPropertyName = nameof(UserRoleUpdatedOnClient.ClientId);
                            metadata.TimeStampPropertyName = nameof(UserRoleUpdatedOnClient.TimeStamp);
                        });
                    });

                    busConfiguration.SequenceGate(configuration);
                });
            }

            public class Handler : IHandleMessages<UserRoleUpdatedOnClient>
            {
                public Context Context { get; set; }

                public void Handle(UserRoleUpdatedOnClient message)
                {
                    Context.ClientIdsWithUpdatedUserRoles.Add(message.ClientId);
                }
            } 
        }

        public class UserRoleUpdatedOnClient : IMessage
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }
            public string ClientId { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
