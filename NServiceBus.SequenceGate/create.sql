IF NOT  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[NServiceBus].[Order.Endpoint.SequenceGate]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Order.Endpoint.SequenceGate](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[ObjectId] [nvarchar](255) NOT NULL,
		[SequenceGateId] [nvarchar](255) NOT NULL,
		[ScopeId] [nvarchar](255),
		[SequenceAnchor] [bigint] NOT NULL,
	) ON [PRIMARY]
END	