﻿using System;
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
        private static IEnumerable<Type> AllowedBasicCollectionTypes { get; } = new List<Type>
        {
            typeof (string),
            typeof (int),
            typeof (long),
            typeof (Guid)
        };

        internal enum ValidationErrors
        {
            MessageTypeMissing,
            ObjectIdPropertyMissing,
            TimeStampPropertyMissingOrNotDateTime,
            ScopeIdPropertyMissing,
            CollectionPropertyMissingOrNotICollection,
            ObjectIdPropertyMissingOnObjectInCollection,
            CollectionObjectTypeNotInAllowedBasicCollectionTypes
        }

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
        internal MessageTypes MessageType
        {
            get
            {
                if (string.IsNullOrEmpty(CollectionPropertyName))
                {
                    return MessageTypes.Single;
                }
                if (string.IsNullOrEmpty(ObjectIdPropertyName))
                {
                    return MessageTypes.PrimitiveCollection;
                }
                return MessageTypes.ComplexCollection;
            }
        }

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

            if (Type == null)
            {
                result.Add(ValidationErrors.MessageTypeMissing);
                return result;
            }

            if (MessageType == MessageTypes.Single)
            {
                ValidateObjectId(result);
            }
            else
            {
                ValidateCollection(result);
            }

            ValidateTimeStamp(result);

            ValidateScope(result);

            return result;
        }

        private void ValidateScope(List<ValidationErrors> result)
        {
            var validScopeId = ValidateProperty(ScopeIdPropertyName, Type, required: false);
            if (!validScopeId)
            {
                result.Add(ValidationErrors.ScopeIdPropertyMissing);
            }
        }

        private void ValidateTimeStamp(List<ValidationErrors> result)
        {
            var validTimeStamp = ValidateProperty(TimeStampPropertyName, Type, typeof (DateTime));
            if (!validTimeStamp)
            {
                result.Add(ValidationErrors.TimeStampPropertyMissingOrNotDateTime);
            }
        }

        private void ValidateCollection(List<ValidationErrors> result)
        {
            var collectionPropertyInfo = Type.GetProperty(CollectionPropertyName);

            if (collectionPropertyInfo == default(PropertyInfo) || !PropertyIsOfTypeIEnumerable(collectionPropertyInfo))
            {
                result.Add(ValidationErrors.CollectionPropertyMissingOrNotICollection);
            }
            else
            {
                var collectionObjectType = collectionPropertyInfo.PropertyType.GetGenericArguments().Single();

                if (MessageType == MessageTypes.PrimitiveCollection)
                {
                    if (!AllowedBasicCollectionTypes.Contains(collectionObjectType))
                    {
                        result.Add(ValidationErrors.CollectionObjectTypeNotInAllowedBasicCollectionTypes);
                    }
                }
                else
                {
                    var validCollectionObjectId = ValidateProperty(ObjectIdPropertyName, collectionObjectType);

                    if (!validCollectionObjectId)
                    {
                        result.Add(ValidationErrors.ObjectIdPropertyMissingOnObjectInCollection);
                    }
                }
            }
        }

        private void ValidateObjectId(List<ValidationErrors> result)
        {
            var validObjectId = ValidateProperty(ObjectIdPropertyName, Type);

            if (!validObjectId)
            {
                result.Add(ValidationErrors.ObjectIdPropertyMissing);
            }
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
