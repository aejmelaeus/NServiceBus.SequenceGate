using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;
using NUnit.Framework;
using User = NServiceBus.SequenceGate.Tests.Acceptance.Messages.User;
using System.Linq;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class ScopeTests : TestFixtureBase
    {
        [Test]
        public void Send_WhenSentWithMessagesInDifferentScopes_ScopeTakenIntoAccount()
        {
            // Arrange

            /*
            ** We track object ids and timestamps.
            ** However messages can have a scope for
            ** a specific id. In this test a user is
            ** granted access to a customer but we 
            ** pretend that the processing fails and
            ** the message ends up in the error queue.
            ** Meantime the same user is granted access 
            ** to another customer in the system. After
            ** that the first message is replayed from
            ** the error queue. The message should be
            ** processed since we have another scope
            ** (customer id).
            */

            var bob = new User
            {
                Id = Guid.NewGuid(),
                Email = "bob@bob.com"
            };

            var hardwareSports = new Customer
            {
                Id = "abc123",
                Name = "Hardware Sports"
            };

            var softwareDish = new Customer
            {
                Id = "def456",
                Name = "Software Dish"
            };

            var bobHardwareSportsGrant = new UsersGrantedAccessToCustomer
            {
                Users = new List<User> {bob},
                Customer = hardwareSports,
                TimeStamp = DateTime.UtcNow.AddDays(-2)
            };

            var bobSoftwareDishGrant = new UsersGrantedAccessToCustomer
            {
                Users = new List<User> {bob},
                Customer = softwareDish,
                TimeStamp = DateTime.UtcNow.AddDays(-1)
            };

            var bus = GetBus();

            // Act
            bus.Send(Destination, bobSoftwareDishGrant);
            Thread.Sleep(5000);
            bus.Send(Destination, bobHardwareSportsGrant);
            Thread.Sleep(5000);

            // Assert
            using (var context = new AcceptanceContext())
            {
                var bobHardwareSportsGrantExists =
                    context.UserCustomers.Any(
                        uc => uc.CustomerId.Equals(hardwareSports.Id) && uc.UserId.Equals(bob.Id));

                var bobSoftwareDishGrantExists =
                    context.UserCustomers.Any(
                        uc => uc.CustomerId.Equals(softwareDish.Id) && uc.UserId.Equals(bob.Id));

                Assert.That(bobHardwareSportsGrantExists, Is.True, "Bob Should have been granted access to the customer Hardware Sports");
                Assert.That(bobSoftwareDishGrantExists, Is.True, "Bob Should have been granted access to the customer Software Dish");
            }
        }
    }
}
