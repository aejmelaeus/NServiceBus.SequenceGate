using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Validates the metadata
        /// </summary>
        /// <returns>Returns a string</returns>
        internal List<string> Validate()
        {
            var result = new List<string>
            {
                ValidateObjectId(),
                ValidateTimeStamp()
            };

            result.RemoveAll(string.IsNullOrEmpty);

            return result;
        }

        private string ValidateObjectId()
        {
            var properties = ObjectIdPropertyName.Split('.');
            var propertiesQueue = new Queue<string>(properties);
            var type = MessageType;

            while (true)
            {
                var propertyName = propertiesQueue.Dequeue();

                var propertyInfo = type.GetProperty(propertyName);

                if (propertyInfo == default(PropertyInfo))
                {
                    return $"Metadata for {MessageType.Name} is invalid. ObjectIdPropertyName {ObjectIdPropertyName} is missing";
                }

                if (propertiesQueue.Count == 0)
                {
                    return string.Empty;
                }

                type = propertyInfo.PropertyType;
            }
        }

        private string ValidateTimeStamp()
        {
            var properties = TimeStampPropertyName.Split('.');
            var propertiesQueue = new Queue<string>(properties);
            var type = MessageType;

            while (true)
            {
                var propertyName = propertiesQueue.Dequeue();

                var propertyInfo = type.GetProperty(propertyName);

                if (propertyInfo == default(PropertyInfo))
                {
                    return $"Metadata for {MessageType.Name} is invalid. TimeStampPropertyName {TimeStampPropertyName} is missing";
                }

                if (propertiesQueue.Count == 0)
                {
                    if (propertyInfo.PropertyType == typeof (DateTime))
                    {
                        return string.Empty;
                    }
                    return $"Metadata for {MessageType.Name} is invalid. Property for TimeStampPropertyName is not of type System.DateTime";
                }

                type = propertyInfo.PropertyType;
            }
        }
    }
}
