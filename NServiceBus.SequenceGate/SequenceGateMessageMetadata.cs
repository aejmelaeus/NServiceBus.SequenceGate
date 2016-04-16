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
        public string ScopeIdPropertyName { get; set; }

        /// <summary>
        /// When present the tracked objects are expected to be in the collection.
        /// Both simple and complex types are supported.
        /// If the collection holds a complex type, for example a "User" class the 
        /// ObjectIdPropertyName is expected to point out the Id property of the
        /// object that is tracked.
        /// If the collection is of primitive type, the ObjectIdPropertyName should
        /// be left null or empty. One could use primitive types when the message 
        /// intent is to delete or revoke something. When adding or granting something
        /// a complex type is preferred, since then the message is "self sufficient",
        /// containing business card data about a user.
        /// </summary>
        public string CollectionPropertyName { get; set; }

        /// <summary>
        /// Validates the metadata
        /// </summary>
        /// <returns>A list of strings with error messages</returns>
        internal List<string> Validate()
        {
            var result = new List<string>();

            if (MessageType == null)
            {
                result.Add("MessageType missing.");
                return result;
            }

            if (string.IsNullOrEmpty(CollectionPropertyName))
            {
                var validObjectId = ValidateProperty(ObjectIdPropertyName);
                if (!validObjectId)
                {
                    result.Add($"Metadata for {MessageType.Name} is invalid. ObjectIdProperty: {ObjectIdPropertyName} is missing");
                }
            }
            else
            {
                // Metadata for CollectionMessage is invalid. Collection with PropertyName WrongCollection does not exist on message type
                var collectionPropertyInfo = MessageType.GetProperty(CollectionPropertyName);
                if (collectionPropertyInfo == default(PropertyInfo))
                {
                    result.Add($"Metadata for {MessageType.Name} is invalid. Collection with PropertyName {CollectionPropertyName} does not exist on message type or is not of type ICollection");
                }
            }

            var validTimeStamp = ValidateProperty(TimeStampPropertyName, typeof (DateTime));
            if (!validTimeStamp)
            {
                result.Add($"Metadata for {MessageType.Name} is invalid. TimeStampProperty: {TimeStampPropertyName} is missing or not of type System.DateTime");
            }

            var validScopeId = ValidateProperty(ScopeIdPropertyName, required: false);
            if (!validScopeId)
            {
                result.Add($"Metadata for {MessageType.Name} is invalid. ScopeIdProperty: {ScopeIdPropertyName} is missing");
            }
            
            return result;
        }

        private bool ValidateProperty(string unsplittedPropertyName, Type expectedType = null, bool required = true)
        {
            if (!required && string.IsNullOrEmpty(unsplittedPropertyName))
            {
                return true;
            }

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
