using System.Data.Entity;
using System.Security.Policy;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Repository
{
    public class AcceptanceContext : DbContext
    {
        public AcceptanceContext() : base("AcceptanceContext")
        {
            // This sure is a whacky context...
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VIPs> VIPs { get; set; }
        public DbSet<UserCustomer> UserCustomers { get; set; }
    }
}
