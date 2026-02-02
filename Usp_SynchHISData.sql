USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[Usp_SynchHISData]    Script Date: 5/26/2024 11:22:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Usp_SynchHISData]
AS

BEGIN

	IF OBJECT_ID(N'tempdb..#PATIENT') IS NOT NULL
	BEGIN
		DROP TABLE #PATIENT
	END
	IF OBJECT_ID(N'tempdb..#MAPPING') IS NOT NULL
	BEGIN
		DROP TABLE #MAPPING
	END

	CREATE TABLE #MAPPING(
	HISTestCode VARCHAR(255),
	SpecimenCode VARCHAR(255),
	SpecimenName VARCHAR(500),
	GroupTag VARCHAR(1)
	)

	INSERT INTO #MAPPING
	SELECT DISTINCT m.HISTestCode,s.SpecimenCode,s.SpecimenName
	,CASE WHEN m.GroupName = 'CITR.PLAS' THEN 'C'
	 WHEN m.GroupName = 'EDTA' THEN 'E'
	 WHEN m.GroupName = 'F' THEN 'F'
	 WHEN m.GroupName = 'PLAIN' THEN 'G'
	 WHEN m.GroupName = 'PP' THEN 'P'
	 WHEN m.GroupName = 'RAN' THEN 'R'
	 WHEN m.GroupName = 'URINE' THEN 'U'
	 ELSE  'G' END
	FROM [BeckmanLIS].[dbo].[TestMappingMaster] m 
	CROSS APPLY (SELECT TOP 1 SpecimenCode,SpecimenName
	 FROM [BeckmanLIS].[dbo].[TestMappingMaster] c WHERE c.IsActive=1 
	 and m.HISTestCode = c.HISTestCode
	 and m.SpecimenCode = c.SpecimenCode
	) s
	WHERE m.IsActive=1
	

	-- Patient details update
	UPDATE d SET 
		d.Name = u.PATIENTNM,
		d.Age = u.AGE,
		d.Gender = CASE WHEN u.SX='M' THEN 'MALE' ELSE 'FEMALE' END
		FROM [BeckmanLIS].[dbo].[PatientDetails] d 
		INNER JOIN [dbo].[Staging_TestReq] u
			ON HISPATIENTID = COALESCE(IPNO,MRNO,REQNO) AND u.Modified = 1
		INNER JOIN #MAPPING AS M ON u.TESTID=M.HISTestCode 
			AND CAST(u.REQDTTM AS DATE) = CAST(GETDATE() AS DATE) 

	-- All requition deleted for a Patient
	-- Few requition deleted for a Patient
	-- Requition modified = 1 Old req deleted + new req inserted

	-- Deleted record from HIS will be deliting from LIS
	-- Change Report Status 16
	UPDATE TRD SET TRD.ReportStatus = 16
	FROM [LISStaging].[dbo].[Staging_TestReq]  AS ST
	INNER JOIN #MAPPING AS M ON ST.TESTID=M.HISTestCode 
			AND CAST(ST.REQDTTM AS DATE) = CAST(GETDATE() AS DATE) 
			AND ST.Acknowledged = 1 
	INNER JOIN [BeckmanLIS].[dbo].[TestRequestDetails] AS TRD
	ON ST.REQNO=TRD.HISRequestNo AND ST.TESTID=TRD.HISTestCode
	WHERE Modified=2 AND TRD.ReportStatus = 0

	UPDATE u Set Modified = 3 
	FROM [dbo].[Staging_TestReq] u
	INNER JOIN #MAPPING AS M ON u.TESTID=M.HISTestCode 
			AND CAST(u.REQDTTM AS DATE) = CAST(GETDATE() AS DATE) 
	WHERE Modified = 1 OR Modified = 2
	

	CREATE TABLE #PATIENT(
		HISPATIENTID NVARCHAR(30),
		REQNO INT,
		PATIENTNM VARCHAR(255),
		AGE VARCHAR(10),
		DOB DATETIME,
		SX VARCHAR(1),
		IPNO INT,
		MRNO INT,
		ISPROCESSED BIT DEFAULT 0
	) 

	INSERT INTO #PATIENT
	SELECT DISTINCT 
	COALESCE(IPNO,MRNO,REQNO) AS HISPATIENTID,
	REQNO,PATIENTNM,AGE,GETDATE(),SX,IPNO,MRNO,0
	FROM [LISStaging].[dbo].[Staging_TestReq] R
	INNER JOIN #MAPPING M ON M.HISTestCode = R.TESTID 
	WHERE R.Acknowledged = 0 AND R.CANCELLED_HDR IS NULL AND R.CANCELLED_DTL IS NULL
	

	WHILE((SELECT COUNT(*)  FROM #PATIENT WHERE ISPROCESSED = 0 ) > 0)
	BEGIN
		DECLARE @REQNO INT,
		@AGE VARCHAR(10),
		@AGEYEAR INT,
		@AGEMONTH INT,
		@DOB DATETIME,
		@HISPATIENTID NVARCHAR(30),
		@HasError bit = 0

		SELECT TOP 1 @REQNO=REQNO,@AGE= AGE,@HISPATIENTID = HISPATIENTID FROM #PATIENT WHERE ISPROCESSED = 0 

		SELECT @AGEYEAR=CAST(CAST(@AGE AS DECIMAL)AS INT)*-1
	
		SELECT @DOB = GETDATE()
		IF(@AGEYEAR <= 0)
		BEGIN
			SELECT @DOB = DATEADD(YEAR,@AGEYEAR,@DOB)
		END
	
		UPDATE #PATIENT SET DOB = @DOB WHERE REQNO = @REQNO
	
		DECLARE @PATIENTID INT 

		BEGIN TRANSACTION

		IF NOT EXISTS(SELECT 1 FROM [BeckmanLIS].[dbo].[PatientDetails] WHERE HISPATIENTID = @HISPATIENTID)
		BEGIN
			INSERT INTO [BeckmanLIS].[dbo].[PatientDetails] 
			(Name,Age,Gender,Phone,IsActive,DateOfBirth,CreatedBy,CreatedOn,HisPatientId)
			SELECT PATIENTNM,CAST(AGE AS decimal),CASE WHEN SX='M' THEN 'MALE' ELSE 'FEMALE' END, 
			'',1,DOB,'JOB',GETDATE(),HISPATIENTID
			 FROM #PATIENT WHERE REQNO=@REQNO

			 SET @PATIENTID=@@IDENTITY

			IF(@@ERROR>0) 
			BEGIN
				SET @HasError = 1
				GOTO ERROR
			END
		 END
		ELSE
		BEGIN
			SELECT @PATIENTID=ID FROM [BeckmanLIS].[dbo].[PatientDetails] WHERE HisPatientId = @HISPATIENTID

			UPDATE B
			SET B.NAME=A.PATIENTNM,B.AGE=A.AGE,B.Gender = A.GENDER
			FROM [BeckmanLIS].[dbo].[PatientDetails] B 
			INNER JOIN (
				SELECT PATIENTNM,CAST(AGE AS decimal) AGE,CASE WHEN SX='M' THEN 'MALE' ELSE 'FEMALE' END AS GENDER, 
				'' AS PHONE,1 AS ISACTIVE,DOB,'JOB' AS CreatedBy,GETDATE() AS CreatedOn,HISPATIENTID
				 FROM #PATIENT WHERE REQNO=@REQNO
			 ) A ON B.HisPatientId = A.HISPATIENTID
			WHERE B.HISPATIENTID = @HISPATIENTID

			IF(@@ERROR>0) 
			BEGIN
				SET @HasError = 1
				GOTO ERROR
			END
		 END		 

		INSERT INTO [BeckmanLIS].[dbo].[TestRequestDetails]
		(SampleNo,HISTestCode,HISTestName,SampleCollectionDate,SampleReceivedDate,
		SpecimenCode,SpecimenName,CreatedBy,CreatedOn,ReportStatus,PatientId,
		IPNo,BedNo,MRNo,HISRequestId,HISRequestNo,DepartmentId,Department)
		SELECT DISTINCT (ST.REQNO+M.GroupTag) AS SAMPLENO,ST.TESTID,ST.TESTNM,
		ST.REQDTTM,ST.REQDTTM,m.SpecimenCode,m.SpecimenName,'JOB',GETDATE(),
		0,@PATIENTID,ST.IPNO,ST.BEDNO,ST.MRNO,ST.REQID,ST.REQNO
		,ST.DEPTID,ST.DEPTNM 
		FROM [LISStaging].[dbo].[Staging_TestReq]  AS ST
		LEFT JOIN [BeckmanLIS].[dbo].[TestRequestDetails] AS TRD
		ON ST.REQNO=TRD.HISRequestNo AND ST.TESTID=TRD.HISTestCode
		INNER JOIN #MAPPING AS M ON ST.TESTID=M.HISTestCode
		WHERE ST.Acknowledged = 0 AND TRD.Id IS NULL AND REQNO=@REQNO 
		AND ST.CANCELLED_HDR IS NULL AND ST.CANCELLED_DTL IS NULL

		IF(@@ERROR>0) 
		BEGIN
			SET @HasError = 1
			GOTO ERROR
		END

		INSERT INTO [BeckmanLIS].[dbo].[TestParameters]
		(HISParamCode,HISParamName,HISTestCode,CreatedBy,CreatedOn,TestRequestDetailsId)
		SELECT DISTINCT STP.ParameterCode,STP.Parameter,STP.TestId,
		'JOB',GETDATE(),TRD.Id 
		FROM [LISStaging].[dbo].[Staging_Testparameter] STP
		INNER JOIN [BeckmanLIS].[dbo].[TestRequestDetails] AS TRD ON
		STP.TestId=TRD.HISTestCode AND TRD.HISRequestNo=@REQNO 
	 
		IF(@@ERROR>0) 
		BEGIN
			SET @HasError = 1
			GOTO ERROR
		END

		UPDATE ST SET ST.Acknowledged=1 
		FROM [LISStaging].[dbo].[Staging_TestReq]  AS ST
		INNER JOIN [BeckmanLIS].[dbo].[TestRequestDetails] AS TRD
		ON ST.REQNO=TRD.HISRequestNo AND ST.TESTID=TRD.HISTestCode
		WHERE REQNO=@REQNO AND ST.CANCELLED_HDR IS NULL AND ST.CANCELLED_DTL IS NULL

		
		ERROR:   
		IF @HasError = 1
		BEGIN
			ROLLBACK TRANSACTION
		END
		ELSE 
		BEGIN
			COMMIT TRANSACTION
		END

		UPDATE #PATIENT SET ISPROCESSED = 1 WHERE REQNO = @REQNO

	END	

END


