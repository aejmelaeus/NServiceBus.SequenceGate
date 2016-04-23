using System;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// Holds the information needed to discard old messages
    /// </summary>
    internal class TrackedObject
    {
        /// <summary>
        /// The id of the gate, for example:
        /// - UserActions
        /// - UserEmailUpdate
        /// </summary>
        public string SequenceGateId { get; set; }
        /// <summary>
        /// The Id of the object, for example the User Id
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// The message time stamp in UTC
        /// </summary>
        public long SequenceAnchor { get; set; }
        /// <summary>
        /// The scope id, can for example be a specific client
        /// </summary>
        public string ScopeId { get; set; }      
    }
}
