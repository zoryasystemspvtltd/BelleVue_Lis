USE [LISStaging]
GO
/****** Object:  Table [dbo].[Archive_Staging_TestReq]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Archive_Staging_TestReq](
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
	[Modified] [int] NOT NULL,
	[Acknowledged] [int] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExecutionLog]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExecutionLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProcudereName] [varchar](255) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[ExecutedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_ExecutionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MigrationHistory]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MigrationHistory](
	[Migration_PK] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[Migration_PK] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Staging_TestAcknowledgement]    Script Date: 11/10/2021 11:35:00 PM ******/
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
/****** Object:  Table [dbo].[Staging_TestMaster]    Script Date: 11/10/2021 11:35:00 PM ******/
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
/****** Object:  Table [dbo].[Staging_Testparameter]    Script Date: 11/10/2021 11:35:00 PM ******/
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
	[MaxValue] [varchar](20) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Staging_TestReq]    Script Date: 11/10/2021 11:35:00 PM ******/
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
	[Acknowledged] [int] NOT NULL DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestResultDetails]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestResultDetails](
	[Id] [bigint] NOT NULL,
	[LISParamCode] [varchar](100) NULL,
	[LISParamValue] [varchar](100) NULL,
	[LISParamUnit] [varchar](200) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[TestResultId] [bigint] NOT NULL,
 CONSTRAINT [PK_TestResultDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TestResults]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TestResults](
	[Id] [bigint] NOT NULL,
	[SampleNo] [varchar](100) NULL,
	[HISTestCode] [varchar](100) NULL,
	[LISTestCode] [varchar](100) NULL,
	[SpecimenCode] [varchar](100) NULL,
	[SpecimenName] [varchar](255) NULL,
	[ResultDate] [datetime] NOT NULL,
	[SampleCollectionDate] [datetime] NOT NULL,
	[SampleReceivedDate] [datetime] NOT NULL,
	[AuthorizationDate] [datetime] NULL,
	[AuthorizedBy] [varchar](100) NULL,
	[ReviewDate] [datetime] NULL,
	[ReviewedBy] [varchar](100) NULL,
	[TechnicianNote] [varchar](1000) NULL,
	[DoctorNote] [varchar](1000) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[PatientId] [bigint] NOT NULL,
	[TestRequestId] [bigint] NOT NULL,
	[EquipmentId] [int] NOT NULL,
 CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO