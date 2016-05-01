using System.Collections.Generic;
using System.Linq;
using NServiceBus.SequenceGate.Tests.Unit.Messages;
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
            const string sequenceGateMemberId = "SomeId";

            var sequenceGateMember = new SequenceGateMember
            {
                Id = sequenceGateMemberId,
                Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                {
                    new NServiceBus.SequenceGate.MessageMetadata
                    {
                        Type = typeof(UserActivated),
                        ObjectIdPropertyName = nameof(UserActivated.ObjectId),
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
            // var sequenceMemberErrors = result[sequenceGateMemberId];
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
            // Assert.That(sequenceMemberErrors.First() == ValidationError.AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent);
        }

        [Test]
        public void Validate_MetadatasHasInconsistentScopes_CorrectValidationErrorReturned()
        {
            // Arrange
            const string sequenceGateMemberId = "SomeId";

            var sequenceGateMember = new SequenceGateMember
            {
                Id = sequenceGateMemberId,
                Messages = new List<NServiceBus.SequenceGate.MessageMetadata>
                {
                    new NServiceBus.SequenceGate.MessageMetadata
                    {
                        Type = typeof(UserActivated),
                        ObjectIdPropertyName = nameof(UserActivated.ObjectId),
                        TimeStampPropertyName = nameof(UserActivated.TimeStamp),
                        ScopeIdPropertyName = nameof(UserActivated.ARandomProperty)
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
            var sequenceMemberErrors = result[sequenceGateMemberId];

            // Assert
            Assert.That(sequenceMemberErrors.First() == ValidationError.AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent);
        }
    }
}