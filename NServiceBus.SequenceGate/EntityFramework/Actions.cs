using System.Collections.Generic;

namespace NServiceBus.SequenceGate.EntityFramework
{
    internal class Actions
    {
        public List<string> IdsToAdd { get; set; } = new List<string>();
        public List<string> IdsToUpdate { get; set; } = new List<string>();
        public List<string> IdsToDismiss { get; set; } = new List<string>();
    }
}