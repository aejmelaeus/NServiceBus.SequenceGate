using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    internal class Mutator : IMutator
    {
        private readonly SequenceGateConfiguration _configuration;

        public Mutator(SequenceGateConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object Mutate(object message, IEnumerable<string> objectIdsToDismiss)
        {
            var messageMetadata = _configuration.GetMessageMetadata(message);

            if (messageMetadata.MessageType == MessageMetadata.MessageTypes.Single)
            {
                var objectId = GetPropertyValue(messageMetadata.ObjectIdPropertyName, message);
                if (objectIdsToDismiss.Contains(objectId))
                {
                    return null;
                }

                return message;
            }

            var collectionPropertyInfo = message.GetType().GetProperty(messageMetadata.CollectionPropertyName);
            var collection = collectionPropertyInfo.GetValue(message) as IList;

            var itemsToRemove = new List<object>();

            /*
            ** Foreach foreach foreach ahead. Wohaa...
            */

            foreach (var item in collection)
            {
                var objectId = messageMetadata.MessageType == MessageMetadata.MessageTypes.PrimitiveCollection  ? 
                    item.ToString() : GetPropertyValue(messageMetadata.ObjectIdPropertyName, item);

                if (objectIdsToDismiss.Contains(objectId))
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
            {
                collection.Remove(item);
            }

            return message;
        }

        private string GetPropertyValue(string unsplittedPropertyName, object message)
        {
            var properties = unsplittedPropertyName.Split('.');
            var propertiesQueue = new Queue<string>(properties);
            var type = message.GetType();
            var obj = message;

            while (true)
            {
                var propertyName = propertiesQueue.Dequeue();

                var propertyInfo = type.GetProperty(propertyName);

                if (propertyInfo == default(PropertyInfo))
                {
                    throw new Exception($"Failed to parse {type.Name}");
                }

                if (propertiesQueue.Count == 0)
                {
                    return propertyInfo.GetValue(obj).ToString();
                }

                type = propertyInfo.PropertyType;
                obj = propertyInfo.GetValue(obj);
            }
        }
    }
}
