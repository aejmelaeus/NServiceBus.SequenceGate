using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    /// <summary>
    /// A member message in the Sequence Gate.
    /// </summary>
    public class MessageMetadata
    {
        internal enum ValidationErrors
        {
            MessageTypeMissing,
            ObjectIdPropertyMissing,
            TimeStampPropertyMissingOrNotDateTime,
            ScopeIdPropertyMissing,
            CollectionPropertyMissingOrNotICollection,
            ObjectIdPropertyMissingOnObjectInCollection
        }
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
        /// <returns>A list of validation errors. Empty collection indicates success</returns>
        internal List<ValidationErrors> Validate()
        {
            var result = new List<ValidationErrors>();

            if (MessageType == null)
            {
                result.Add(ValidationErrors.MessageTypeMissing);
                return result;
            }

            if (string.IsNullOrEmpty(CollectionPropertyName))
            {
                var validObjectId = ValidateProperty(ObjectIdPropertyName, MessageType);
                if (!validObjectId)
                {
                    result.Add(ValidationErrors.ObjectIdPropertyMissing);
                }
            }
            else
            {
                var collectionPropertyInfo = MessageType.GetProperty(CollectionPropertyName);
                if (collectionPropertyInfo == default(PropertyInfo) || !PropertyIsOfTypeIEnumerable(collectionPropertyInfo))
                {   
                    result.Add(ValidationErrors.CollectionPropertyMissingOrNotICollection);
                }
                else
                {
                    var collectionObjectType = collectionPropertyInfo.PropertyType.GetGenericArguments().Single();
                    var validCollectionObjectId = ValidateProperty(ObjectIdPropertyName, collectionObjectType);
                    if (!validCollectionObjectId)
                    {
                        result.Add(ValidationErrors.ObjectIdPropertyMissingOnObjectInCollection);
                    }
                }
            }

            var validTimeStamp = ValidateProperty(TimeStampPropertyName, MessageType, typeof (DateTime));
            if (!validTimeStamp)
            {
                result.Add(ValidationErrors.TimeStampPropertyMissingOrNotDateTime);
            }

            var validScopeId = ValidateProperty(ScopeIdPropertyName, MessageType, required: false);
            if (!validScopeId)
            {
                result.Add(ValidationErrors.ScopeIdPropertyMissing);
            }
            
            return result;
        }

        private bool PropertyIsOfTypeIEnumerable(PropertyInfo collectionPropertyInfo)
        {
            return collectionPropertyInfo.PropertyType.GetInterface("ICollection") != null;
        }

        private bool ValidateProperty(string unsplittedPropertyName, Type rootObjectType, Type expectedPropertyType = null, bool required = true)
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
