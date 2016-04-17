using System;
using System.Collections.Generic;

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

                trackedObject.ObjectId = GetObjectId(messageMetadata, message);
                trackedObject.TimeStampUTC = GetTimeStamp(messageMetadata, message);
                trackedObject.ScopeId = GetScopeId(messageMetadata, message);
                trackedObject.SequenceGateId = _configuration.GetSequenceGateIdForMessage(message);

                result.Add(trackedObject);
            }

            return result;
        }

        private string GetScopeId(MessageMetadata messageMetadata, object message)
        {
            var objectIdProperty = messageMetadata.MessageType.GetProperty(messageMetadata.ScopeIdPropertyName);
            var propertyValue = objectIdProperty.GetValue(message).ToString();
            return propertyValue;
        }

        private string GetObjectId(MessageMetadata messageMetadata, object message)
        {
            var objectIdProperty = messageMetadata.MessageType.GetProperty(messageMetadata.ObjectIdPropertyName);
            var propertyValue = objectIdProperty.GetValue(message).ToString();
            return propertyValue;
        }

        private DateTime GetTimeStamp(MessageMetadata messageMetadata, object message)
        {
            var timeStampProperty = messageMetadata.MessageType.GetProperty(messageMetadata.TimeStampPropertyName);
            var propertyValue = timeStampProperty.GetValue(message);

            return (DateTime) propertyValue;
        }
    }
}
