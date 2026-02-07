USE [LISStaging]
GO

/****** Object:  Table [dbo].[Staging_TestAcknowledgement]    Script Date: 5/26/2024 11:30:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Staging_TestAcknowledgement](
	[RequesitionId] [varchar](30) NOT NULL,
	[RequesitionNumber] [varchar](30) NOT NULL,
	[TestId] [varchar](8) NOT NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_dbo.Staging_TestAcknowledgement] PRIMARY KEY CLUSTERED 
(
	[RequesitionId] ASC,
	[TestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


