using System;

namespace NServiceBus.SequenceGate.Repository
{
    public class Gate
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
        /// The id of the scope, can for example be a Customer Id
        /// </summary>
        public string ScopeId { get; set; }
        /// <summary>
        /// The message time stamp in UTC
        /// </summary>
        public DateTime TimeStampUTC { get; set; }
    }
}
