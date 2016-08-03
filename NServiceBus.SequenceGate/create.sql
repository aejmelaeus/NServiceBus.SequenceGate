CREATE TABLE [dbo].[SequenceMembers] (
    [Id] [bigint] NOT NULL IDENTITY,
    [SequenceGateId] [nvarchar](128) NOT NULL,
    [ScopeId] [nvarchar](128) NOT NULL,
    [EndpointName] [nvarchar](128) NOT NULL,
    CONSTRAINT [PK_dbo.SequenceMembers] PRIMARY KEY ([Id])
)
CREATE UNIQUE INDEX [IX_SequenceGateId_ScopeId_EndpointName] ON [dbo].[SequenceMembers]([SequenceGateId], [ScopeId], [EndpointName])
CREATE TABLE [dbo].[SequenceObjects] (
    [Id] [nvarchar](128) NOT NULL,
    [SequenceMemberId] [bigint] NOT NULL,
    [SequenceAnchor] [bigint] NOT NULL,
    CONSTRAINT [PK_dbo.SequenceObjects] PRIMARY KEY ([Id], [SequenceMemberId])
)
CREATE INDEX [IX_SequenceMemberId] ON [dbo].[SequenceObjects]([SequenceMemberId])
ALTER TABLE [dbo].[SequenceObjects] ADD CONSTRAINT [FK_dbo.SequenceObjects_dbo.SequenceMembers_SequenceMemberId] FOREIGN KEY ([SequenceMemberId]) REFERENCES [dbo].[SequenceMembers] ([Id]) ON DELETE CASCADE