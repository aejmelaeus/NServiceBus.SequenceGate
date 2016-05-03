namespace NServiceBus.SequenceGate.Tests.Acceptance
{
    public abstract class TestFixtureBase
    {
        protected const string Destination = "NServiceBus.SequenceGate.Tests.Acceptance.Endpoint";

        protected ISendOnlyBus GetBus()
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
