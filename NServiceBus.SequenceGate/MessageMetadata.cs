using System;
using System.Reflection;
using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// A member message in the Sequence Gate.
    /// </summary>
    public abstract class MessageMetadata
    {
        /// <summary>
        /// Describes the different types of message
        /// </summary>
        internal enum MessageTypes
        {
            /// <summary>
            /// Message contains a single object
            /// </summary>
            Single,
            /// <summary>
            /// Message contains a collection with primitive values
            /// </summary>
            PrimitiveCollection,
            /// <summary>
            /// Message contains a collection of complex object and the object 
            /// id is descibed in the ObjectIdPropertyName property
            /// </summary>
            ComplexCollection
        }

        /// <summary>
        /// The actual message type
        /// </summary>
        internal abstract MessageTypes MessageType { get; }
        
        /// <summary>
        /// The type of the message
        /// </summary>
        public Type Type { get; set; }
        
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

        internal abstract void MetadataSpecificValidation(List<ValidationError> result);

        /// <summary>
        /// Validates the metadata
        /// </summary>
        /// <returns>A list of validation errors. Empty collection indicates success</returns>
        internal List<ValidationError> Validate()
        {
            var result = new List<ValidationError>();

            if (Type == null)
            {
                result.Add(ValidationError.MessageTypeMissing);
                return result;
            }

            MetadataSpecificValidation(result);

            ValidateTimeStamp(result);

            ValidateScope(result);

            return result;
        }

        private void ValidateScope(List<ValidationError> result)
        {
            var validScopeId = ValidateProperty(ScopeIdPropertyName, Type, required: false);
            if (!validScopeId)
            {
                result.Add(ValidationError.ScopeIdPropertyMissing);
            }
        }

        private void ValidateTimeStamp(List<ValidationError> result)
        {
            var validTimeStamp = ValidateProperty(TimeStampPropertyName, Type, typeof (DateTime));
            if (!validTimeStamp)
            {
                result.Add(ValidationError.TimeStampPropertyMissingOrNotDateTime);
            }
        }

        protected bool ValidateProperty(string unsplittedPropertyName, Type rootObjectType, Type expectedPropertyType = null, bool required = true)
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
            var type = rootObjectType;

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
                    if (expectedPropertyType == default (Type))
                    {
                        return true;
                    }
                    return expectedPropertyType == propertyInfo.PropertyType;
                }

                type = propertyInfo.PropertyType;
            }
        }
    }
}
