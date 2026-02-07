USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[usp_HISDataIntegration]    Script Date: 9/24/2021 12:37:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_HISDataIntegration] 
AS
BEGIN
	SET NOCOUNT ON;
	-- Execution Log
	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISDataIntegration]','HIS Data Integration begins',getdate())

	DECLARE @insertFlag int;
	DECLARE @updateFlag int;
	DECLARE @count int;

	SET @insertFlag = 0
	SET @updateFlag = 0

	IF OBJECT_ID(N'tempdb..#TestReqKey') IS NOT NULL
	BEGIN
		DROP TABLE #TestReqKey
	END
	 
	CREATE TABLE #TestReqKey
	(
	 ReqDateTime DateTime2,
	 ReqDate Date, 
	 ReqTime Time,
	 ReqId varchar(30),
	 ReqNo varchar(30),
	 TestId varchar(8)
	)

	INSERT INTO #TestReqKey (ReqDateTime,ReqDate,ReqTime,ReqId,ReqNo,TestId)
	SELECT REQDTTM, CAST(REQDTTM AS DATE) AS REQDT, CAST(REQDTTM AS TIME) AS REQTM,REQID,REQNO,TESTID
	FROM [dbo].[vwTestReq]

	SELECT @count = COUNT(1) 
	FROM [dbo].[Staging_TestReq] A
	JOIN #TestReqKey B ON CAST(A.REQDTTM AS DATE) = B.ReqDate
	
	IF (@count > 0 )
	BEGIN
		SET @insertFlag = 0
		SET @updateFlag = 1
	END
	ELSE
	BEGIN
		SET @insertFlag = 1
		SET @updateFlag = 0
	END

	IF (@insertFlag = 1 )
	BEGIN
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
           ,[Modified])
		SELECT 
		    REQID
           ,TESTID
           ,TYP
           ,CANCELLED_HDR
           ,CANCELLED_DTL
           ,IPNO
           ,BEDNO
           ,MRNO
           ,REQNO
           ,DEPTNM
           ,GROUPID
           ,GROUPNM
           ,DEPTID
           ,TESTNM
           ,PATIENTNM
           ,AGE
           ,YMD
           ,SX
           ,REQDTTM
           ,RCDATE
           ,SADATE
           ,COLDATE
           ,COLLTIME
           ,PRINTDT
           ,PRINTTM
           ,PRINTDTTM
           ,APPROVEDDT
           ,APPROVEDTM
           ,APPROVEDTTM
           ,PERFORMEDDT
           ,PERFORMEDTM
           ,DRNAME
           ,IPOPDOCNM           
		    ,@updateFlag
		FROM [dbo].[vwTestReq] 
	END

	IF (@updateFlag = 1 )
	BEGIN

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
           ,[Modified])
		SELECT 
		    A.REQID
           ,A.TESTID
           ,TYP
           ,CANCELLED_HDR
           ,CANCELLED_DTL
           ,IPNO
           ,BEDNO
           ,MRNO
           ,A.REQNO
           ,DEPTNM
           ,GROUPID
           ,GROUPNM
           ,DEPTID
           ,TESTNM
           ,PATIENTNM
           ,AGE
           ,YMD
           ,SX
           ,REQDTTM
           ,RCDATE
           ,SADATE
           ,COLDATE
           ,COLLTIME
           ,PRINTDT
           ,PRINTTM
           ,PRINTDTTM
           ,APPROVEDDT
           ,APPROVEDTM
           ,APPROVEDTTM
           ,PERFORMEDDT
           ,PERFORMEDTM
           ,DRNAME
           ,IPOPDOCNM           
		    ,@updateFlag
		FROM [dbo].[vwTestReq] A
		JOIN (
			SELECT A.ReqDateTime,A.ReqDate,A.ReqTime,A.ReqId,A.ReqNo,A.TestId
			FROM #TestReqKey A
			LEFT JOIN [dbo].[Staging_TestReq] B ON CAST(B.REQDTTM AS DATE) = A.ReqDate AND A.ReqNo = B.REQNO AND A.ReqId = B.REQID AND A.TestId = B.TESTID
			WHERE (ISNULL(B.REQNO,'') = '' OR B.REQNO IS NULL)
		) B ON CAST(B.ReqDateTime AS DATE) = CAST(A.REQDTTM AS DATE) AND A.ReqNo = B.ReqNo AND A.ReqId = B.ReqId AND A.TestId = B.TestId

		UPDATE [dbo].[Staging_TestReq]		
		  SET [DEPTNM]   = A.[DEPTNM]   
		  ,[TESTID] = A.[TESTID]
		  ,[GROUPID] = A.[GROUPID]
		  ,[GROUPNM] = A.[GROUPNM]
		  ,[DEPTID] = A.[DEPTID]
		  ,[TESTNM] = A.[TESTNM]
		  ,[PATIENTNM] = A.[PATIENTNM]
		  ,[AGE] = A.[AGE]
		  ,[YMD] = A.[YMD]
		  ,[SX] = A.[SX]
		FROM [dbo].[vwTestReq] A JOIN 
		(
			SELECT A.ReqDateTime,A.ReqDate,A.ReqTime,A.ReqId,A.ReqNo,A.TestId
			FROM #TestReqKey A
			JOIN [dbo].[Staging_TestReq] B ON CAST(B.REQDTTM AS DATE) = A.ReqDate AND A.ReqNo = B.REQNO AND A.ReqId = B.REQID AND A.TestId = B.TESTID
			WHERE CAST(B.REQDTTM AS TIME) < A.ReqTime
		) B ON CAST(B.ReqDateTime AS DATE) = CAST(A.REQDTTM AS DATE) AND A.ReqNo = B.ReqNo AND A.ReqId = B.ReqId AND A.TestId = B.TestId
	END
  
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



GO