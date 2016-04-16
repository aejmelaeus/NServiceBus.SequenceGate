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
        /// A message can contain a scope.
        /// The scope can be a "client" or "category" in the system.
        /// A message where scope could be used could be "UserGrantedDiscountOnProductCategory".
        /// In this case the product category would be the ScopeId.
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// Validates the metadata
        /// </summary>
        /// <returns>Returns a string</returns>
        internal List<string> Validate()
        {
            var result = new List<string>();

            if (MessageType == null)
            {
                result.Add("MessageType missing.");
                return result;
            }

            var validObjectId = ValidateProperty(ObjectIdPropertyName);
            if (!validObjectId)
            {
                result.Add($"Metadata for {MessageType.Name} is invalid. ObjectIdProperty: {ObjectIdPropertyName} is missing");
            }

            var validTimeStamp = ValidateProperty(TimeStampPropertyName, typeof (DateTime));
            if (!validTimeStamp)
            {
                result.Add($"Metadata for {MessageType.Name} is invalid. TimeStampProperty: {TimeStampPropertyName} is missing or not of type System.DateTime");
            }
            
            return result;
        }

        private bool ValidateProperty(string unsplittedPropertyName, Type expectedType = null)
        {
            if (string.IsNullOrEmpty(unsplittedPropertyName))
            {
                return false;
            }
            
            var properties = unsplittedPropertyName.Split('.');
            var propertiesQueue = new Queue<string>(properties);
            var type = MessageType;

            while (true)
            {
                var propertyName = propertiesQueue.Dequeue();

                var propertyInfo = type.GetProperty(propertyName);

                if (propertyInfo == default(PropertyInfo))
                {
                    return false;
                }

                if (propertiesQueue.Count == 0)
                {
                    if (expectedType == default (Type))
                    {
                        return true;
                    }
                    return expectedType == propertyInfo.PropertyType;
                }

                type = propertyInfo.PropertyType;
            }
        }
    }
}
