using Autofac;
using NServiceBus.SequenceGate.Repository;

namespace NServiceBus.SequenceGate
{
    public static class SequenceGateBusConfiguration
    {
        public static BusConfiguration SequenceGate(this BusConfiguration busConfiguration, SequenceGateConfiguration sequenceGateConfiguration)
        {
            sequenceGateConfiguration.Validate();

            IPersistence persistence = new EntityFramework.Persistence();
            IParser parser = new Parser(sequenceGateConfiguration);
            IMutator mutator = new Mutator(sequenceGateConfiguration);
            var sequenceGate = new SequenceGate(sequenceGateConfiguration, persistence, parser, mutator);

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(sequenceGate);
            IContainer container = builder.Build();
            busConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            busConfiguration.Pipeline.Register<SequenceGateRegistration>();

            return busConfiguration;
        }
    }
}
