using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.SequenceGate.Repository
{
    internal class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    internal class MetaData
    {
        public DateTime TimeStamp { get; set; }
    }

    internal class Service
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal class UsersAddedToServiceOnCompany
    {
        public List<User> Users { get; set; }
        public Service Service { get; set; }
        public Company Company { get; set; }
        public MetaData MetaData { get; set; }
    }
}
