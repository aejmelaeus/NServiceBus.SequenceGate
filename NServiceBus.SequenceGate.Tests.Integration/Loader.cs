using System;
using NServiceBus.SequenceGate.EntityFramework;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Integration
{
    [TestFixture]
    public class Loader
    {
        //[Ignore("Should only be used manually...")]
        [Test]
        public void Load()
        {
            int total = 0;

            while (true)
            {
                using (var context = new SequenceGateContext())
                {
                    string endpointName = Guid.NewGuid().ToString();
                    string scopeId = Guid.NewGuid().ToString();
                    string sequenceGateId = Guid.NewGuid().ToString();
                    long sequenceAncor = DateTime.UtcNow.Ticks;

                    var member = new SequenceMember();
                    member.EndpointName = endpointName;
                    member.ScopeId = scopeId;
                    member.SequenceGateId = sequenceGateId;

                    for (int i = 0; i < 100; i++)
                    {
                        var o = new SequenceObject();
                        o.Id = Guid.NewGuid().ToString();
                        o.SequenceAnchor = sequenceAncor;
                        
                        member.Objects.Add(o);
                    }

                    context.SequenceMembers.Add(member);

                    context.SaveChanges();
                }

                Console.WriteLine("Total: " + total);
                total += 100;
            }
        }
    }
}
