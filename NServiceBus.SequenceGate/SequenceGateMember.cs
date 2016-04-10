using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// A member in the sequence gate.
    /// Can consist of a single message, for example "UserEmailUpdated".
    /// Or several messages that cancels out each other, for example "UserPermissionGranted" or "UserPermissionRevoked".
    /// </summary>
    public class SequenceGateMember
    {
        /// <summary>
        /// The Id of the member, for example "UserEmailUpdated" or "UserPermissionActions".
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The metadata of the actual messages.
        /// </summary>
        public List<SequenceGateMessage> Messages { get; set; }
    }
}