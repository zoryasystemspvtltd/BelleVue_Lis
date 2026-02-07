USE [LISStaging]
GO

/****** Object:  Table [dbo].[Staging_Testparameter]    Script Date: 5/26/2024 11:31:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Staging_Testparameter](
	[TestId] [varchar](8) NOT NULL,
	[TestAlias] [varchar](15) NOT NULL,
	[TestName] [varchar](100) NOT NULL,
	[ParameterCode] [varchar](15) NOT NULL,
	[Parameter] [varchar](150) NOT NULL,
	[MethodName] [varchar](200) NOT NULL,
	[Gender] [varchar](150) NOT NULL,
	[AgeFrom] [decimal](15, 2) NOT NULL CONSTRAINT [DF_AgeFrom]  DEFAULT ((0)),
	[AgeTo] [decimal](15, 2) NOT NULL CONSTRAINT [DF_AgeTo]  DEFAULT ((0)),
	[AgeType] [varchar](15) NOT NULL,
	[MinValue] [varchar](15) NOT NULL,
	[MaxValue] [varchar](20) NOT NULL,
	[Unit] [varchar](100) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


