using System;
using System.Threading;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class SimpleMessageTests
    {
        const string destination = "NServiceBus.SequenceGate.Tests.Acceptance.Endpoint";

        [Test]
        public void Send_WithOlderMessage_OlderMessageDiscarded()
        {
            // Arrange
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

            bus.Send(destination, newerMessage);
            
            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Act
            bus.Send(destination, olderMessage);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Assert
            using (var context = new UserContext())
            {
                var user = context.Users.Find(userId);

                Assert.That(user.Email, Is.EqualTo(newerEmail));
            }
        }

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
