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
        /// Instantiates a sequence gate member. A message class that participates in the gate.
        /// Could be for example messages granting and revoking permissions for a user.
        /// The property names in this example can be notated with a dot to indicate nested properties.
        /// </summary>
        /// <param name="messageType">The type of the message</param>
        /// <param name="objectIdPropertyName">The property name of the object id.</param>
        /// <param name="timeStampPropertyName">The property name of the time stamp.</param>
        /// <param name="collectionPropertyName">The property name of the collection.</param>
        /// <param name="scopeIdPropertyName">The property name of the collection.</param>
        public SequenceGateMember(
            Type messageType, 
            string objectIdPropertyName, 
            string timeStampPropertyName,
            string collectionPropertyName,
            string scopeIdPropertyName)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// The type of the message
        /// </summary>
        public Type MessageType { get; private set; }
        /// <summary>
        /// The property of the object id
        /// </summary>
        public PropertyInfo ObjectIdProperty { get; private set; }
        /// <summary>
        /// The time stamp property of the message
        /// </summary>
        public PropertyInfo TimeStampProperty { get; private set; }
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
