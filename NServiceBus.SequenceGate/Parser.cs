using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceBus.SequenceGate
{
    internal class Parser : IParser
    {
        private readonly SequenceGateConfiguration _configuration;

        public Parser(SequenceGateConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<TrackedObject> Parse(object message)
        {
            var result = new List<TrackedObject>();
            var messageMetadata = _configuration.GetMessageMetadata(message);

            if (string.IsNullOrEmpty(messageMetadata.CollectionPropertyName))
            {
                var trackedObject = new TrackedObject();

                trackedObject.ObjectId = GetString(messageMetadata.ObjectIdPropertyName, message);
                trackedObject.TimeStampUTC = GetDateTime(messageMetadata.TimeStampPropertyName, message);
                trackedObject.ScopeId = GetString(messageMetadata.ScopeIdPropertyName, message);
                trackedObject.SequenceGateId = _configuration.GetSequenceGateIdForMessage(message);

                result.Add(trackedObject);
            }

            return result;
        }

        private DateTime GetDateTime(string unsplittedPropertyName, object message)
        {
            return (DateTime) GetPropertyValue(unsplittedPropertyName, message);
        }

        private string GetString(string unsplittedPropertyName, object message)
        {
            return GetPropertyValue(unsplittedPropertyName, message).ToString(); 
        }

        private object GetPropertyValue(string unsplittedPropertyName, object message)
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
                    return propertyInfo.GetValue(obj);
                }

                type = propertyInfo.PropertyType;
                obj = propertyInfo.GetValue(obj);
            }
        }
    }
}
