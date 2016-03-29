using System;
using System.Reflection;

namespace NServiceBus.SequenceGate.Repository
{
    /// <summary>
    /// A member message in the Sequence Gate.
    /// Optional collection handling. When collection property name is present a the
    /// gated objects are retrieved from the collection with the provided object id 
    /// property name.
    /// </summary>
    public class SequenceGateMember
    {
        /// <summary>
        /// The type of the message
        /// </summary>
        public Type MessageType { get; set; }
        /// <summary>
        /// The property of the object id
        /// </summary>
        public string ObjectIdPropertyName { get; set; }
        /// <summary>
        /// The time stamp property of the message
        /// </summary>
        public string TimeStampPropertyName { get; set; }
        /// <summary>
        /// If present the object id will be from the collection
        /// </summary>
        public string CollectionPropertyName { get; set; }
        /// <summary>
        /// If set there will be one sequence gate per scope.
        /// For example a scope id could be a client id in the system.
        /// This gives the possibility to have different sequence gates for
        /// users on different clients in the system.
        /// </summary>
        public string ScopeIdPropertyName { get; set; }
    }
}
