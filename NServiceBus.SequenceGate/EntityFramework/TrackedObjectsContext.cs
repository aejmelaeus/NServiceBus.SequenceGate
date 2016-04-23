using System.Data.Entity;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class TrackedObjectsContext : DbContext
    {
        public TrackedObjectsContext() : base ("NServiceBus/SequenceGate")
        {
            // Nothing here...
        }

        public DbSet<TrackedObjectEntity> TrackedObjectEntities { get; set; }
    }
}
