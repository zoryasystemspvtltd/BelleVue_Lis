USE [LISStaging]
GO
/****** Object:  View [dbo].[vwHISTestParameters]    Script Date: 9/24/2021 12:36:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwHISTestParameters]
AS 

SELECT A.TESTID as TestId,A.TCODE AS TestAlias,A.TESTNM AS TestName,B.COMPID AS ParameterCode,B.COMPNM AS Parameter,
B.METHOD AS MethodName,B.SEX AS Gender,B.FROMAGE AS AgeFrom,B.TOAGE AS AgeTo,B.TP1 AS AgeType,
B.REFRANGETO AS MinValue,B.MAXRAFRANGE AS MaxValue
FROM [NEOSOFT]..[LIS_ZO].[LIS_TESTMAST] A
INNER JOIN [NEOSOFT]..[LIS_ZO].[LIS_TESTPARAM] B ON A.TESTID = B.TESTID



GO
/****** Object:  View [dbo].[vwTestMaster]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vwTestMaster]
AS
SELECT [TESTID] AS TestId  
      ,[TCODE]  AS TestAlias
      ,[TESTNM]   AS TestName
      ,[SAMPLEID]  AS SampleId 
      ,[SAMPLENM]  AS [Sample]
      ,[ACTIVE] AS Active
  FROM [NEOSOFT]..[LIS_ZO].[LIS_TESTMAST]
  WHERE ACTIVE = 1


GO
/****** Object:  View [dbo].[vwTestReq]    Script Date: 11/10/2021 11:35:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vwTestReq]
AS
SELECT [TYP]     
      ,[CANCELLED_HDR]    
      ,[CANCELLED_DTL]    
      ,[IPNO]    
      ,[BEDNO]     
      ,[MRNO]   
      ,[REQID]    
      ,[REQNO]  
      ,[DEPTNM] 
      ,[TESTID]
      ,[GROUPID]    
      ,[GROUPNM]  
      ,[DEPTID]    
      ,[TESTNM]     
      ,[PATIENTNM]
      ,[AGE]
      ,[YMD]     
      ,[SX]    
      ,[REQDTTM]   
      ,[RCDATE]      
      ,[SADATE]
      ,[COLDATE]     
      ,[COLLTIME]
      ,[COLLDTTM]     
      ,[PRINTDT]    
      ,[PRINTTM]     
      ,[PRINTDTTM]     
      ,[APPROVEDDT]     
      ,[APPROVEDTM]    
      ,[APPROVEDTTM]
      ,[PERFORMEDDT]     
      ,[PERFORMEDTM]    
      ,[PERFORMDTTM]     
      ,[DRNAME]    
      ,[IPOPDOCNM]
	  ,[EDCOUNT]
  FROM [NEOSOFT]..[LIS_ZO].[LIS_TESTREQ]
  WHERE CAST(REQDTTM AS DATE) >= CAST(GETDATE() AS DATE)
  AND [DEPTNM] NOT IN (
	'RADIOLOGY'
	,'C.T.SCAN'
	,'SONOGRAPHY'
	,'MAMOGRAPHY'
	,'M.R.I'
	,'CARDIOLOGY'
	,'EYE CLINIC'
	,'GEN. PHYS. CON.(PKG.)'
	,'GYANECOLOGIST CONSULT'
	,'IMMUNO - HISTOCHEMIST'
	,'NEUROLOGY'
)



GO