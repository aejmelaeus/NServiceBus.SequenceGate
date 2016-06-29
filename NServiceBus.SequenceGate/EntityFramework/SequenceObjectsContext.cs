using System.Data.Entity;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class SequenceObjectsContext : DbContext
    {
        public SequenceObjectsContext() : base ("NServiceBus/SequenceGate")
        {
            // Nothing here...
        }
        
        public DbSet<SequenceObject> TrackedObjectEntities { get; set; }
    }
}
