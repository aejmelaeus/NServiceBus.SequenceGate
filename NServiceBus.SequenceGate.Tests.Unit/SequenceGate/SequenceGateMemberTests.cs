using System.Linq;
using NUnit.Framework;
using NServiceBus.SequenceGate.Tests.Unit.Messages;

namespace NServiceBus.SequenceGate.Tests.Unit.SequenceGate
{
    [TestFixture]
    public class SequenceGateMemberTests
    {
        [Test]
        public void Big_Fat_Fail()
        {
            Assert.Fail("This should fail!");
        }

        [Test]
        public void Validate_AllMetadatasHasConsistentScopeIdSet_PassesValiation()
        {
            // Arrange
            const string sequenceGateMemberId = "SomeId";

            var sequenceGateMember = new SequenceGateMember
            {
                Id = sequenceGateMemberId
            };

            sequenceGateMember.HasSingleObjectMessage<UserActivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserActivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserActivated.TimeStamp);
            });

            sequenceGateMember.HasSingleObjectMessage<UserDeactivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserDeactivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserDeactivated.TimeStamp);
            });
            
            // Act
            var result = sequenceGateMember.Validate();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void Validate_MetadatasHasInconsistentScopes_CorrectValidationErrorReturned()
        {
            // Arrange
            const string sequenceGateMemberId = "SomeId";

            var sequenceGateMember = new SequenceGateMember
            {
                Id = sequenceGateMemberId
            };

            sequenceGateMember.HasSingleObjectMessage<UserActivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserActivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserActivated.TimeStamp);
                metadata.ScopeIdPropertyName = nameof(UserActivated.ARandomProperty);
            });

            sequenceGateMember.HasSingleObjectMessage<UserDeactivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserDeactivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserDeactivated.TimeStamp);
            });

            // Act
            var result = sequenceGateMember.Validate();
            var sequenceMemberErrors = result[sequenceGateMemberId];

            // Assert
            Assert.That(sequenceMemberErrors.First() == ValidationError.AllMessagesInASequenceGateMemberMustHaveScopeSetConsistent);
        }

        [Test]
        public void Validate_IdIsNullOrEmpty_CorrectValidationError()
        {
            // Arrange
            var sequenceGateMember = new SequenceGateMember
            {
                Id = string.Empty
            };

            sequenceGateMember.HasSingleObjectMessage<UserActivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserActivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserActivated.TimeStamp);
            });

            // Act
            var result = sequenceGateMember.Validate();
            var sequenceMemberErrors = result[typeof(SequenceGateMember).FullName];

            // Assert
            Assert.That(sequenceMemberErrors.First() == ValidationError.IdMissingOnSequenceGateMember);
        }

        [Test]
        public void Validate_NoMessageMetadataGiven_CorrectValidation()
        {
            // Arrange
            const string sequenceGateId = "abc123";

            var sequenceGateMember = new SequenceGateMember
            {
                Id = sequenceGateId
            };

            // Act
            var result = sequenceGateMember.Validate();
            var sequenceMemberErrors = result[sequenceGateId];

            // Assert
            Assert.That(sequenceMemberErrors.Contains(ValidationError.SequenceMetadataIsMissingOnMember));
        }

        [Test]
        public void Validate_ValidatesAllMessageMetadatas_VaidationErrorsFromMessageMetadatasReturned()
        {
            // Arrange
            var sequenceGateMember = new SequenceGateMember();

            sequenceGateMember.HasSingleObjectMessage<UserActivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = nameof(UserActivated.ObjectId);
                metadata.TimeStampPropertyName = nameof(UserActivated.TimeStamp);
            })
            .HasSingleObjectMessage<UserDeactivated>(metadata =>
            {
                metadata.ObjectIdPropertyName = "Some.Bogus";
                metadata.TimeStampPropertyName = nameof(UserDeactivated.TimeStamp);
            });

            // Act
            var result = sequenceGateMember.Validate();
            var sequenceMemberErrors = result[typeof(UserDeactivated).FullName];

            // Assert
            Assert.That(sequenceMemberErrors.First() == ValidationError.ObjectIdPropertyMissing);
        }
    }
}