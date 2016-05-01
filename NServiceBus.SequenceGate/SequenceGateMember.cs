using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<MessageMetadata> Messages { get; set; }

        internal Dictionary<string, IEnumerable<ValidationError>> Validate()
        {
            var result = new Dictionary<string, IEnumerable<ValidationError>>();

            if (!AllMessagesHasConsistentScope())
            {
                result.Add(Id, new[] { ValidationError.AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent});
            }

            return result;
        }

        private bool AllMessagesHasConsistentScope()
        {
            var firstMessageMetadata = Messages.First();

            if (string.IsNullOrEmpty(firstMessageMetadata.ScopeIdPropertyName))
            {
                return Messages.TrueForAll(m => string.IsNullOrEmpty(m.ScopeIdPropertyName));
            }
            return Messages.TrueForAll(m => !string.IsNullOrEmpty(m.ScopeIdPropertyName));
        }
    }
}