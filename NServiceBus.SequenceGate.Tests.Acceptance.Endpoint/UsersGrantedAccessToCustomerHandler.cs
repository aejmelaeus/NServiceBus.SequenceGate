using System;
using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Endpoint
{
    public class UsersGrantedAccessToCustomerHandler : IHandleMessages<UsersGrantedAccessToCustomer>
    {
        public void Handle(UsersGrantedAccessToCustomer message)
        {
            using (var context = new AcceptanceContext())
            {
                foreach (var user in message.Users)
                {
                    var userCustomer = new UserCustomer
                    {
                        UserId = user.Id,
                        CustomerId = message.Customer.Id
                    };

                    context.UserCustomers.Add(userCustomer);
                }

                context.SaveChanges();
            }
        }
    }
}
