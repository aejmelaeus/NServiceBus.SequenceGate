using System.Collections.Generic;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.SequenceGate.Tests.Unit.SequenceGate
{
    [TestFixture]
    public class SequenceGateMemberTests
    {
        [Test]
        public void Validate_AllMetadatasHasConsistentScopeIdSet_PassesValiation()
        {
            // Arrange
            var sequenceGateMember = new SequenceGateMember
            {
                Id = "SomeId",
                Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                {
                    new NServiceBus.SequenceGate.MessageMetadata
                    {
                        Type = typeof(UserActivated),
                        ObjectIdPropertyName = nameof(UserActivated.ObjectId),
                        ScopeIdPropertyName = nameof(UserActivated.ARandomProperty),
                        TimeStampPropertyName = nameof(UserActivated.TimeStamp)
                    },
                    new NServiceBus.SequenceGate.MessageMetadata
                    {
                        Type = typeof(UserDeactivated),
                        ObjectIdPropertyName = nameof(UserDeactivated.ObjectId),
                        TimeStampPropertyName = nameof(UserDeactivated.TimeStamp)
                    }
                }
            };

            // Act
            var result = sequenceGateMember.Validate();

            // Assert
        }
    }
}