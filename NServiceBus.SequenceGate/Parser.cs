using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.SequenceGate.EntityFramework;

namespace NServiceBus.SequenceGate
{
    internal class Parser : IParser
    {
        internal const string ScopeIdNotApplicable = "NotApplicable";

        private readonly SequenceGateConfiguration _configuration;

        public Parser(SequenceGateConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public ParsedMessage Parse(object message)
        {
            var messageMetadata = _configuration.GetMessageMetadata(message);

            string endpointName = _configuration.EndpointName;
            long sequenceAnchor = GetDateTime(messageMetadata.TimeStampPropertyName, message).Ticks;
            string sequenceGateId = _configuration.GetSequenceGateIdForMessage(message);
            string scopeId = GetScopeId(message, messageMetadata);

            var result = new ParsedMessage(endpointName, sequenceGateId, scopeId, sequenceAnchor);

            if (messageMetadata is SingleObjectMessageMetadata)
            {
                var objectId = GetString(messageMetadata.ObjectIdPropertyName, message);
                result.AddObjectId(objectId);
            }
            else
            {
                var multipleObjectsMessageMetadata = messageMetadata as MultipleObjectMessageMetadata;
                var collectionPropertyInfo = message.GetType().GetProperty(multipleObjectsMessageMetadata.CollectionPropertyName);
                IEnumerable collection = (IEnumerable)collectionPropertyInfo.GetValue(message);

                foreach (var item in collection)
                {
                    string objectId = string.Empty;

                    if (messageMetadata.MessageType == MessageMetadata.MessageTypes.PrimitiveCollection)
                    {
                        objectId = item.ToString();
                    }
                    else
                    {
                        objectId = GetString(messageMetadata.ObjectIdPropertyName, item);
                    }

                    result.AddObjectId(objectId);
                }
            }

            return result;
        }

        private string GetScopeId(object message, MessageMetadata messageMetadata)
        {
            return string.IsNullOrEmpty(messageMetadata.ScopeIdPropertyName) ?
                ScopeIdNotApplicable :
                GetString(messageMetadata.ScopeIdPropertyName, message);
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
