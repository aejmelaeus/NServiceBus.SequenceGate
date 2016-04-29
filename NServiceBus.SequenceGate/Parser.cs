using System;
using System.Collections;
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

            var timeStamp = GetDateTime(messageMetadata.TimeStampPropertyName, message).Ticks;
            var sequenceGateId = _configuration.GetSequenceGateIdForMessage(message);

            var scopeId = string.IsNullOrEmpty(messageMetadata.ScopeIdPropertyName) ?
                string.Empty : 
                GetString(messageMetadata.ScopeIdPropertyName, message);

            if (messageMetadata.MessageType == MessageMetadata.MessageTypes.Single)
            {
                var trackedObject = new TrackedObject();

                trackedObject.ObjectId = GetString(messageMetadata.ObjectIdPropertyName, message);
                trackedObject.SequenceAnchor = timeStamp;
                trackedObject.ScopeId = scopeId;
                trackedObject.SequenceGateId = sequenceGateId;
                
                result.Add(trackedObject);
            }
            else
            {
                var collectionPropertyInfo = message.GetType().GetProperty(messageMetadata.CollectionPropertyName);
                IEnumerable collection = (IEnumerable) collectionPropertyInfo.GetValue(message);

                foreach (var item in collection)
                {
                    var trackedObject = new TrackedObject();
                    trackedObject.SequenceAnchor = timeStamp;
                    trackedObject.ScopeId = scopeId;
                    trackedObject.SequenceGateId = sequenceGateId;

                    if (messageMetadata.MessageType == MessageMetadata.MessageTypes.PrimitiveCollection)
                    {
                        trackedObject.ObjectId = item.ToString();
                    }
                    else
                    {
                        trackedObject.ObjectId = GetString(messageMetadata.ObjectIdPropertyName, item);
                    }

                    result.Add(trackedObject);
                }
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
