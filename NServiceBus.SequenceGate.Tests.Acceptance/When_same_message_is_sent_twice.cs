using NServiceBus.AcceptanceTesting;
using NServiceBus.SequenceGate.Tests.Acceptance.Infrastructure;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    [TestFixture]
    public class When_same_message_is_sent_twice
    {
        [Test]
        public void Then_message_is_passed_through()
        {
            
        }

        public class Context : ScenarioContext
        {
            public int MessageCount { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(busConfiguration =>
                {
                    busConfiguration.EnableInstallers();


                });
            }
        }
    }
}
