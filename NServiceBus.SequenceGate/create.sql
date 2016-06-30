CREATE TABLE [dbo].[TrackedObjects] (
    [Id] [bigint] NOT NULL IDENTITY,
    [SequenceGateId] [nvarchar](max) NOT NULL,
    [ObjectId] [nvarchar](max) NOT NULL,
    [SequenceAnchor] [bigint] NOT NULL,
    [ScopeId] [nvarchar](max) NOT NULL,
    [EndpointName] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_dbo.TrackedObjects] PRIMARY KEY ([Id])
)