USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[usp_HISTestIntegration]    Script Date: 5/26/2024 11:21:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[usp_HISTestIntegration] 
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISTestIntegration] ','HIS Test Integration begins',getdate())

	CREATE TABLE #TestTmp
	(
		TestId varchar(8) NOT NULL,
		Modified int NOT NULL Default 0
	)

	INSERT INTO  #TestTmp
	SELECT DISTINCT TestId,0
	FROM [dbo].[vwTestMaster]	

	--insert test
	UPDATE #TestTmp
	SET Modified = 0
	FROM #TestTmp A
	LEFT JOIN [dbo].[Staging_TestMaster] B ON A.TestId = B.TestId
	WHERE B.TestId IS NULL

	--update test
	UPDATE #TestTmp
	SET Modified = 1
	FROM #TestTmp A
	INNER JOIN [dbo].[Staging_TestMaster] B ON A.TestId = B.TestId

	INSERT INTO [dbo].[Staging_TestMaster]
           ([TestId]
           ,[TestAlias]
           ,[TestName]
           ,[SampleId]
           ,[Sample])
	SELECT
	 A.[TestId]
	,ISNULL([TestAlias],'')
	,ISNULL([TestName],'')
	,ISNULL([SampleId],'')
	,ISNULL([Sample],'')
	FROM [dbo].[vwTestMaster] A
	INNER JOIN #TestTmp B	ON A.TestId = B.TestId
	WHERE B.Modified = 0

	UPDATE [dbo].[Staging_TestMaster]
	SET TestAlias = ISNULL(B.TestAlias,''),
	TestName = ISNULL(B.TestName,''),
	SampleId = ISNULL(B.SampleId,''),
	[Sample] = ISNULL(B.[Sample],'')
	FROM [dbo].[Staging_TestMaster] A
	INNER JOIN 
	(   SELECT 
		 A.[TestId]
        ,[TestAlias]
        ,[TestName]
        ,[SampleId]
        ,[Sample]
		FROM [dbo].[vwTestMaster] A
		INNER JOIN  #TestTmp B ON A.TestId = B.TestId
		WHERE B.Modified = 1
	) B ON A.TestId = B.TestId

		INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISTestIntegration] ','HIS Test Integration ends',getdate())

		-- DATA PURGING FOR TEST REQUISITION 
		BEGIN TRANSACTION
		BEGIN TRY
			INSERT INTO [dbo].[Archive_Staging_TestReq]
			SELECT * FROM [dbo].[Staging_TestReq] 
			WHERE CAST(REQDTTM AS DATE) < CAST(GETDATE() AS DATE)


			DELETE FROM [dbo].[Staging_TestReq] 
			WHERE CAST(REQDTTM AS DATE) < CAST(GETDATE() AS DATE)
		END TRY
		BEGIN CATCH
			INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) 
			VALUES('[dbo].[usp_HISTestIntegration] ','DATA PURGING FOR TEST REQUISITION ROLLBACK - '+CAST(@@ERROR AS VARCHAR),getdate())
			ROLLBACK TRANSACTION
			RETURN 0
		END CATCH
		INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) 
		VALUES('[dbo].[usp_HISTestIntegration] ','DATA PURGING FOR TEST REQUISITION COMMITED',getdate())

		COMMIT TRANSACTION
END



