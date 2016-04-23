using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    internal class Mutator
    {
        private readonly SequenceGateConfiguration _configuration;

        public Mutator(SequenceGateConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object Mutate(object message, List<string> objectIdsToDismiss)
        {
            var messageMetadata = _configuration.GetMessageMetadata(message);

            if (messageMetadata.MessageType == MessageMetadata.MessageTypes.Single)
            {
                var objectId = GetPropertyValue(messageMetadata.ObjectIdPropertyName, message);
                if (objectIdsToDismiss.Contains(objectId))
                {
                    return null;
                }
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
