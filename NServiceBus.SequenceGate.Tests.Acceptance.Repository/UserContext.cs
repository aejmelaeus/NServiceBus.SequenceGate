using System.Data.Entity;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository
{
    public class UserContext : DbContext
    {
        public UserContext() : base("UserContext")
        {
            // Nothing here...
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VIPs> VIPs { get; set; } 
    }
}
