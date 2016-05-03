using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Endpoint
{
    public class VIPHandlers : IHandleMessages<VIPStatusGranted>, IHandleMessages<VIPStatusRevoked>
    {
        public void Handle(VIPStatusGranted message)
        {
            using (var context = new AcceptanceContext())
            {
                foreach (var newVIP in message.Users)
                {
                    var VIPEntity = new VIPs { UserId = newVIP.Id };
                    context.VIPs.Add(VIPEntity);
                }

                context.SaveChanges();
            }
        }

        public void Handle(VIPStatusRevoked message)
        {
            using (var context = new AcceptanceContext())
            {
                foreach (var userId in message.UserIds)
                {
                    var VIPEntity = context.VIPs.Find(userId);
                    if (VIPEntity != default(VIPs))
                    {
                        context.VIPs.Remove(VIPEntity);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
