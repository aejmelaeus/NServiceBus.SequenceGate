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

        public IEnumerable<MessageMetadata> Messages => _messages;

        /// <summary>
        /// The metadata of the actual messages.
        /// </summary>
        private readonly List<MessageMetadata> _messages = new List<MessageMetadata>();

        internal Dictionary<string, IEnumerable<ValidationError>> Validate()
        {
            var result = new Dictionary<string, IEnumerable<ValidationError>>();

            if (string.IsNullOrEmpty(Id))
            {
                result.Add(typeof(SequenceGateMember).FullName, new[] { ValidationError.IdMissingOnSequenceGateMember });
            }

            if (Messages == null || !Messages.Any())
            {
                result.Add(Id, new[] { ValidationError.SequenceMetadataIsMissingOnMember });
                return result;
            }

            if (!AllMessagesHasConsistentScope())
            {
                result.Add(Id, new[] { ValidationError.AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent });
            }

            foreach (var messageMetadata in Messages)
            {
                var messageMetadataValidationResult = messageMetadata.Validate();
                if (messageMetadataValidationResult.Any())
                {
                    result.Add(messageMetadata.Type.FullName, messageMetadataValidationResult);
                }
            }

            return result;
        }

        private bool AllMessagesHasConsistentScope()
        {
            var firstMessageMetadata = Messages.First();

            if (string.IsNullOrEmpty(firstMessageMetadata.ScopeIdPropertyName))
            {
                return _messages.TrueForAll(m => string.IsNullOrEmpty(m.ScopeIdPropertyName));
            }
            return _messages.TrueForAll(m => !string.IsNullOrEmpty(m.ScopeIdPropertyName));
        }

        internal void AddMessage(MessageMetadata messageMetadata)
        {
            _messages.Add(messageMetadata);
        }
    }

    public static class SequenceGateMemberExtensions
    {
        public static SequenceGateMember HasSingleObjectMessage<TMessageType>(this SequenceGateMember sequenceGateMember,
            Action<SingleObjectMessageMetadata> configureMessageMetadata) where TMessageType : class
        {
            var messageMetadata = new SingleObjectMessageMetadata { Type = typeof(TMessageType) };
            configureMessageMetadata(messageMetadata);
            sequenceGateMember.AddMessage(messageMetadata);
            return sequenceGateMember;
        }

        public static SequenceGateMember HasMultipleObjectsMessage<TMessageType>(this SequenceGateMember sequenceGateMember,
            Action<MultipleObjectMessageMetadata> configureMessageMetadata) where TMessageType : class
        {
            var messageMetadata = new MultipleObjectMessageMetadata { Type = typeof(TMessageType) };
            configureMessageMetadata(messageMetadata);
            sequenceGateMember.AddMessage(messageMetadata);
            return sequenceGateMember;
        }
    }
}