using System;
using System.Collections.Generic;
using System.Threading;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;
using NUnit.Framework;
using User = NServiceBus.SequenceGate.Tests.Acceptance.Messages.User;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class CollectionMessageTests
    {
        [Test]
        public void Send_WhenMessagesAreInACollection_OlderMessageDiscarded()
        {
            // Arrange

            /*
            ** We pretend that Bob and Mary was granted VIP
            ** status. But somehow the processing of the message
            ** failed and ended up in the error queue.
            ** Directly after that Bob's VIP status was revoked.
            ** When retrying the failed messages from the error
            ** queue only Marys VIP grant should be registered
            ** since it is the newest seen 'VIPAction' for Marys
            ** UserId.
            */

            var bobUserId = Guid.NewGuid();
            var maryUserId = Guid.NewGuid();

            var bob = new User();
            bob.Id = bobUserId;
            bob.Email = "bob@bob.com";

            var mary = new User();
            mary.Id = maryUserId;
            mary.Email = "mary@mary.com";

            var newVIPs = new VIPStatusGranted();
            newVIPs.Users = new List<User> { bob, mary };
            newVIPs.TimeStamp = DateTime.UtcNow.AddDays(-2);

            var revokedVIPs = new VIPStatusRevoked();
            revokedVIPs.UserIds = new List<Guid> { bobUserId };
            revokedVIPs.TimeStamp = DateTime.UtcNow.AddDays(-1);

            var bus = GetBus();

            // Act
            bus.Send(destination, revokedVIPs);
            Thread.Sleep(5000);
            bus.Send(destination, newVIPs);
            Thread.Sleep(5000);

            // Assert
            using (var context = new UserContext())
            {
                var bobVIP = context.VIPs.Find(bobUserId);
                Assert.That(bobVIP, Is.Null);

                var maryVIP = context.VIPs.Find(maryUserId);
                Assert.That(maryVIP.UserId, Is.EqualTo(maryUserId));
            }
        }

        /*
        ** TODOS: Move these to a common base class.
        */

        const string destination = "NServiceBus.SequenceGate.Tests.Acceptance.Endpoint";

        private ISendOnlyBus GetBus()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("NServiceBus.SequenceGate.Tests.Acceptance");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();

            return Bus.CreateSendOnly(busConfiguration);
        }
    }
}
