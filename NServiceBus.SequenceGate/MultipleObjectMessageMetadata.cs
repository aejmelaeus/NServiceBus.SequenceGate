using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    public class MultipleObjectMessageMetadata : MessageMetadata
    {
        private static IEnumerable<Type> AllowedBasicCollectionTypes { get; } = new List<Type>
        {
            typeof (string),
            typeof (int),
            typeof (long),
            typeof (Guid)
        };

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

        internal override MessageTypes MessageType => string.IsNullOrEmpty(ObjectIdPropertyName)
            ? MessageTypes.PrimitiveCollection
            : MessageTypes.ComplexCollection;

        internal override void MetadataSpecificValidation(List<ValidationError> result)
        {
            var collectionPropertyInfo = Type.GetProperty(CollectionPropertyName);

            if (collectionPropertyInfo == default(PropertyInfo) || !PropertyIsOfTypeIEnumerable(collectionPropertyInfo))
            {
                result.Add(ValidationError.CollectionPropertyMissingOrNotICollection);
            }
            else
            {
                var collectionObjectType = collectionPropertyInfo.PropertyType.GetGenericArguments().Single();

                if (MessageType == MessageTypes.PrimitiveCollection)
                {
                    if (!AllowedBasicCollectionTypes.Contains(collectionObjectType))
                    {
                        result.Add(ValidationError.CollectionObjectTypeNotInAllowedBasicCollectionTypes);
                    }
                }
                else
                {
                    var validCollectionObjectId = ValidateProperty(ObjectIdPropertyName, collectionObjectType);

                    if (!validCollectionObjectId)
                    {
                        result.Add(ValidationError.ObjectIdPropertyMissingOnObjectInCollection);
                    }
                }
            }
        }

        private bool PropertyIsOfTypeIEnumerable(PropertyInfo collectionPropertyInfo)
        {
            return collectionPropertyInfo.PropertyType.GetInterface("ICollection") != null;
        }
    }
}