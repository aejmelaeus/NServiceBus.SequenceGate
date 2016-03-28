using System;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace NServiceBus.SequenceGate.Repository
{
    internal class MessageClass
    {
        public DateTime TimeStamp { get; set; }
        public string ObjectId { get; set; }
    }

    public class SequenceGateBehavior : IBehavior<IncomingContext>
    {
        public void Invoke(IncomingContext context, Action next)
        {
            var member = new SequenceGateMember();
            member.ObjectIdProperty = 
        }
    }
}
