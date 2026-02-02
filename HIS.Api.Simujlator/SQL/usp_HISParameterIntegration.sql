USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[usp_HISParameterIntegration]    Script Date: 9/24/2021 12:37:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_HISParameterIntegration] 
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISParameterIntegration]','HIS Parameter Integration begins',getdate())

	CREATE TABLE #TestTmp
	(
		TestId varchar(8) NOT NULL,
		ParameterCode varchar(8) NOT NULL,
		Modified int NOT NULL Default 0
	)

	INSERT INTO  #TestTmp
	SELECT DISTINCT TestId,ParameterCode,0
	FROM [dbo].[vwHISTestParameters]	

	--insert test
	UPDATE #TestTmp
	SET Modified = 0
	FROM #TestTmp A
	LEFT JOIN [dbo].[Staging_Testparameter] B ON A.TestId = B.TestId AND A.ParameterCode = B.ParameterCode
	WHERE B.TestId IS NULL AND B.ParameterCode IS NULL

	--update test
	UPDATE #TestTmp
	SET Modified = 1
	FROM #TestTmp A
	INNER JOIN [dbo].[Staging_Testparameter] B ON A.TestId = B.TestId AND A.ParameterCode = B.ParameterCode


	INSERT INTO [dbo].[Staging_Testparameter]
           ([TestId]
           ,[TestAlias]
           ,[TestName]
           ,[ParameterCode]
           ,[Parameter]
           ,[MethodName]
           ,[Gender]
           ,[AgeFrom]
           ,[AgeTo]
           ,[AgeType]
           ,[MinValue]
           ,[MaxValue])
	SELECT
	 A.[TestId]
    ,ISNULL([TestAlias],'')
    ,ISNULL([TestName],'')
    ,ISNULL(A.[ParameterCode],'')
    ,ISNULL([Parameter],'')
    ,ISNULL([MethodName],'')
    ,ISNULL([Gender],'')
    ,CAST(ISNULL([AgeFrom],0) AS Decimal) AS [AgeFrom]
    ,CAST(ISNULL([AgeTo],0) AS Decimal) AS [AgeTo]
    ,ISNULL([AgeType],'')
    ,ISNULL([MinValue],'0')
    ,ISNULL([MaxValue],'0')
	FROM [dbo].[vwHISTestParameters] A
	INNER JOIN #TestTmp B	ON A.TestId = B.TestId AND A.ParameterCode = B.ParameterCode
	WHERE B.Modified = 0

	UPDATE [dbo].[Staging_Testparameter]
	 SET 
      [TestAlias]  = ISNULL(B.TestAlias,'')
      ,[TestName]  = ISNULL(B.TestName,'')     
      ,[Parameter]  = ISNULL(B.Parameter,'')
      ,[MethodName]  = ISNULL(B.MethodName,'')
      ,[Gender]  = ISNULL(B.Gender,'')
      ,[AgeFrom]  = CAST(ISNULL(B.AgeFrom,0) AS Decimal)
      ,[AgeTo]  = CAST(ISNULL(B.AgeTo,0) AS Decimal)
      ,[AgeType]  = ISNULL(B.AgeType,'')
      ,[MinValue]  = ISNULL(B.MinValue,'0')
      ,[MaxValue]  = ISNULL(B.MaxValue,'0')
	FROM [dbo].[Staging_Testparameter] A
	INNER JOIN 
	(   SELECT 
		 A.[TestId]
		,[TestAlias]
		,[TestName]
		,A.[ParameterCode]
		,[Parameter]
		,[MethodName]
		,[Gender]
		,[AgeFrom]
		,[AgeTo]
		,[AgeType]
		,[MinValue]
		,[MaxValue]
		FROM [dbo].[vwHISTestParameters] A
		INNER JOIN  #TestTmp B ON A.TestId = B.TestId AND A.ParameterCode = B.ParameterCode
		WHERE B.Modified = 1
	) B ON A.TestId = B.TestId AND A.ParameterCode = B.ParameterCode

	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_HISParameterIntegration]','HIS Parameter Integration ends',getdate())
END



GO