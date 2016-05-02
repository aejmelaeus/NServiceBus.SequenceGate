using NServiceBus.SequenceGate.Tests.Acceptance.Messages;
using NServiceBus.SequenceGate.Tests.Acceptance.Repository;
using User = NServiceBus.SequenceGate.Tests.Acceptance.Repository.User;

namespace NServiceBus.SequenceGate.Tests.Acceptance.Endpoint
{
    public class UserEmailUpdatedHandler : IHandleMessages<UserEmailUpdated>
    {
        public void Handle(UserEmailUpdated message)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.Find(message.UserId);

                if (user == default(User))
                {
                    user = new User { Id = message.UserId };
                    context.Users.Add(user);
                }

                user.Email = message.Email;

                context.SaveChanges();
            }
        }
    }
}
