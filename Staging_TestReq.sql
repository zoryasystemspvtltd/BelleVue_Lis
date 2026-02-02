USE [LISStaging]
GO

/****** Object:  Table [dbo].[Staging_TestReq]    Script Date: 5/26/2024 11:32:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Staging_TestReq](
	[REQID] [varchar](30) NOT NULL,
	[TESTID] [varchar](8) NOT NULL,
	[TYP] [varchar](3) NULL,
	[CANCELLED_HDR] [varchar](9) NULL,
	[CANCELLED_DTL] [varchar](9) NULL,
	[IPNO] [varchar](50) NULL,
	[BEDNO] [varchar](50) NULL,
	[MRNO] [varchar](40) NULL,
	[REQNO] [varchar](30) NOT NULL,
	[DEPTNM] [varchar](42) NULL,
	[GROUPID] [varchar](10) NULL,
	[GROUPNM] [varchar](50) NULL,
	[DEPTID] [varchar](8) NULL,
	[TESTNM] [varchar](100) NULL,
	[PATIENTNM] [varchar](100) NOT NULL,
	[AGE] [numeric](7, 2) NULL,
	[YMD] [varchar](2) NULL,
	[SX] [varchar](3) NULL,
	[REQDTTM] [datetime2](7) NOT NULL,
	[RCDATE] [varchar](12) NULL,
	[SADATE] [varchar](12) NULL,
	[COLDATE] [varchar](12) NULL,
	[COLLTIME] [datetime2](7) NULL,
	[PRINTDT] [datetime2](7) NULL,
	[PRINTTM] [datetime2](7) NULL,
	[PRINTDTTM] [varchar](22) NULL,
	[APPROVEDDT] [datetime2](7) NULL,
	[APPROVEDTM] [datetime2](7) NULL,
	[APPROVEDTTM] [varchar](22) NULL,
	[PERFORMEDDT] [datetime2](7) NULL,
	[PERFORMEDTM] [datetime2](7) NULL,
	[DRNAME] [varchar](100) NULL,
	[IPOPDOCNM] [varchar](75) NULL,
	[Modified] [int] NOT NULL DEFAULT ((1)),
	[Acknowledged] [int] NOT NULL DEFAULT ((0)),
	[EDCount] [int] NOT NULL DEFAULT ((0))
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


