USE [LISStaging]
GO

/****** Object:  Table [dbo].[Staging_TestResult_RERUN]    Script Date: 5/26/2024 11:33:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Staging_TestResult_RERUN](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[REQID] [nvarchar](20) NULL,
	[REQNO] [nvarchar](20) NULL,
	[IPNO] [nvarchar](20) NULL,
	[MRNO] [nvarchar](20) NULL,
	[TESTID] [nvarchar](20) NULL,
	[TESTNM] [nvarchar](100) NULL,
	[PATIENTNAME] [varchar](max) NULL,
	[SPECIMENCODE] [nvarchar](20) NULL,
	[BARCODE] [nvarchar](30) NULL,
	[PARAMCODE] [nvarchar](max) NULL,
	[PARAMNAME] [nvarchar](max) NULL,
	[PARAMVALUE] [nvarchar](max) NULL,
	[PARAMUNIT] [nvarchar](max) NULL,
	[REQDT] [datetime] NOT NULL,
	[RESULTDT] [datetime] NOT NULL,
	[CREATEDON] [datetime] NOT NULL,
	[SENTTOHIS] [int] NOT NULL,
	[HASPARAM] [int] NOT NULL,
	[EquipmentId] [int] NULL,
	[Equipment] [nvarchar](250) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


