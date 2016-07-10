using System.Collections.Generic;

namespace NServiceBus.SequenceGate
{
    public class SingleObjectMessageMetadata : MessageMetadata
    {
        internal override MessageTypes MessageType => MessageTypes.Single;

        internal override void MetadataSpecificValidation(List<ValidationError> result)
        {
            var validObjectId = ValidateProperty(ObjectIdPropertyName, Type);

            if (!validObjectId)
            {
                result.Add(ValidationError.ObjectIdPropertyMissing);
            }
        }
    }
}