using System;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// Holds the information needed to discard old messages
    /// </summary>
    internal class GateData
    {
        /// <summary>
        /// The Database Id
        /// </summary>
        public long Id { get; set; }
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
        public DateTime TimeStampUTC { get; set; }
    }
}
