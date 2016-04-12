using System;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// A member message in the Sequence Gate.
    /// </summary>
    public class SequenceGateMessageMetadata
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

        internal string Validate()
        {
            var objectIdPropertyInfo = MessageType.GetProperty(ObjectIdPropertyName);

            if (objectIdPropertyInfo == default(PropertyInfo))
            {
                return $"Metadata for {MessageType.Name} is invalid. ObjectIdPropertyName {ObjectIdPropertyName} is missing";
            }

            var timeStampPropertyInfo = MessageType.GetProperty(TimeStampPropertyName);

            if (timeStampPropertyInfo == default(PropertyInfo))
            {
                return $"Metadata for {MessageType.Name} is invalid. TimeStampPropertyName {TimeStampPropertyName} is missing";
            }

            return string.Empty;
        }
    }
}
