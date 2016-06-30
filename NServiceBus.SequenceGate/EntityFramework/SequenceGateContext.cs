using System.Data.Entity;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class SequenceGateContext : DbContext
    {
        public SequenceGateContext() : base ("NServiceBus/SequenceGate")
        {
            // Nothing here...
        }
        
        public DbSet<TrackedObject> TrackedObjects { get; set; }
    }
}
