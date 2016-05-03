using System;
using System.Threading;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class SimpleMessageTests : TestFixtureBase
    {
        [Test]
        public void Send_WithOlderMessage_OlderMessageDiscarded()
        {
            // Arrange

            /*
            ** In this case a users email becomes
            ** updated. We pretend that the first
            ** message fails and that the users
            ** email is changed again before a 
            ** retry of the first message.
            */

            var userId = Guid.NewGuid();
            const string newerEmail = "newer.test@test.com";
            const string olderEmail = "older.test@test.com";

            var newerMessage = new UserEmailUpdated
            {
                UserId = userId,
                Email = newerEmail,
                TimeStampUtc = DateTime.UtcNow.AddDays(-1)
            };

            var olderMessage = new UserEmailUpdated
            {
                UserId = userId,
                Email = olderEmail,
                TimeStampUtc = DateTime.UtcNow.AddDays(-2)
            };

            var bus = GetBus();

            bus.Send(Destination, newerMessage);
            
            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Act
            bus.Send(Destination, olderMessage);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Assert
            using (var context = new AcceptanceContext())
            {
                var user = context.Users.Find(userId);

                Assert.That(user.Email, Is.EqualTo(newerEmail));
            }
        }
    }
}
