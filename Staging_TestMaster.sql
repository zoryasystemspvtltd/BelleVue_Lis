USE [LISStaging]
GO

/****** Object:  Table [dbo].[Staging_TestMaster]    Script Date: 5/26/2024 11:30:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Staging_TestMaster](
	[TestId] [varchar](8) NOT NULL,
	[TestAlias] [varchar](30) NOT NULL,
	[TestName] [varchar](100) NOT NULL,
	[SampleId] [varchar](8) NOT NULL,
	[Sample] [varchar](100) NOT NULL,
 CONSTRAINT [PK_dbo.Staging_TestMaster] PRIMARY KEY CLUSTERED 
(
	[TestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


