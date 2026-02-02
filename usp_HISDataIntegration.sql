USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[usp_HISDataIntegration]    Script Date: 5/26/2024 11:20:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[usp_HISDataIntegration] 
AS
BEGIN
	SET NOCOUNT ON;
	-- Execution Log
	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISDataIntegration]','HIS Data Integration begins',getdate())

	INSERT INTO [dbo].[Staging_TestReq]
			([REQID]
           ,[TESTID]
           ,[TYP]
           ,[CANCELLED_HDR]
           ,[CANCELLED_DTL]
           ,[IPNO]
           ,[BEDNO]
           ,[MRNO]
           ,[REQNO]
           ,[DEPTNM]
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
           ,[PRINTDT]
           ,[PRINTTM]
           ,[PRINTDTTM]
           ,[APPROVEDDT]
           ,[APPROVEDTM]
           ,[APPROVEDTTM]
           ,[PERFORMEDDT]
           ,[PERFORMEDTM]
           ,[DRNAME]
           ,[IPOPDOCNM]
           ,[Modified]
		   ,[EDCOUNT])
		SELECT 
		    A.REQID
           ,A.TESTID
           ,A.TYP
           ,A.CANCELLED_HDR
           ,A.CANCELLED_DTL
           ,A.IPNO
           ,A.BEDNO
           ,A.MRNO
           ,A.REQNO
           ,A.DEPTNM
           ,A.GROUPID
           ,A.GROUPNM
           ,A.DEPTID
           ,A.TESTNM
           ,A.PATIENTNM
           ,A.AGE
           ,A.YMD
           ,A.SX
           ,A.REQDTTM
           ,A.RCDATE
           ,A.SADATE
           ,A.COLDATE
           ,A.COLLTIME
           ,A.PRINTDT
           ,A.PRINTTM
           ,A.PRINTDTTM
           ,A.APPROVEDDT
           ,A.APPROVEDTM
           ,A.APPROVEDTTM
           ,A.PERFORMEDDT
           ,A.PERFORMEDTM
           ,A.DRNAME
           ,A.IPOPDOCNM           
		    ,0
			,A.EDCOUNT
		FROM [dbo].[vwTestReq] A
		LEFT JOIN [dbo].[Staging_TestReq] B ON CAST(B.REQDTTM AS DATE) = CAST(B.REQDTTM AS DATE)
				 AND A.REQNO = B.REQNO AND A.REQID = B.REQID AND A.TESTID = B.TESTID
		WHERE B.REQNO IS NULL

		
	UPDATE B	
		  SET B.[DEPTNM]   = A.[DEPTNM]   
		  ,B.[TESTID] = A.[TESTID]
		  ,B.[GROUPID] = A.[GROUPID]
		  ,B.[GROUPNM] = A.[GROUPNM]
		  ,B.[DEPTID] = A.[DEPTID]
		  ,B.[TESTNM] = A.[TESTNM]
		  ,B.[PATIENTNM] = A.[PATIENTNM]
		  ,B.[AGE] = A.[AGE]
		  ,B.[YMD] = A.[YMD]
		  ,B.[SX] = A.[SX]
		  ,B.EDCount = a.EDCOUNT
		  ,B.CANCELLED_HDR = A.CANCELLED_HDR
		  ,B.CANCELLED_DTL = A.CANCELLED_DTL
		  ,B.[Modified] = CASE WHEN A.EDCOUNT <> B.EDCount THEN 1 ELSE 2 END --2: Cancelled, 1: Demography updated
		FROM [dbo].[vwTestReq] A 
		INNER JOIN [dbo].[Staging_TestReq] B ON CAST(B.REQDTTM AS DATE) = CAST(B.REQDTTM AS DATE)
			 AND A.REQNO = B.REQNO AND A.REQID = B.REQID AND A.TESTID = B.TESTID
		WHERE (A.CANCELLED_HDR IS NOT NULL AND B.CANCELLED_HDR IS NULL)
		OR (A.CANCELLED_DTL IS NOT NULL AND B.CANCELLED_DTL IS NULL)
		OR (A.EDCOUNT <> B.EDCount)
  

    INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISDataIntegration]','HIS Data Integration ends',getdate())


    BEGIN TRY  
     EXEC [dbo].[Usp_SynchHISData]
	END TRY  
	BEGIN CATCH  
		IF @@ERROR > 0
		BEGIN
			DECLARE @MESSAGE VARCHAR(MAX)

			SELECT @MESSAGE = CAST(ERROR_NUMBER() AS VARCHAR) + ':' + CAST(ERROR_LINE() AS VARCHAR) + ':' + ERROR_MESSAGE();
			INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[Usp_SynchHISData]',@MESSAGE,getdate())
		END
	END CATCH 

END



