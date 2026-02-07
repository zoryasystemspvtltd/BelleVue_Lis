USE [LISStaging]
GO
/****** Object:  StoredProcedure [dbo].[usp_SyncTestResult]    Script Date: 5/26/2024 11:23:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usp_SyncTestResult]
as
begin
INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_SyncTestResult]','LIS Test Result sync begins',getdate())

IF OBJECT_ID(N'tempdb..#MAPPING') IS NOT NULL
	BEGIN
		DROP TABLE #MAPPING
	END

	CREATE TABLE #MAPPING(
	HISTestCode VARCHAR(255),
	SpecimenCode VARCHAR(255),
	SpecimenName VARCHAR(500),
	LISTestCode VARCHAR(255),
	EquipmentId Int,
	EquipmentName VARCHAR(500)
	)

	INSERT INTO #MAPPING
	SELECT DISTINCT m.HISTestCode,s.SpecimenCode,s.SpecimenName
	,m.LISTestCode,m.EquipmentId,em.Name
	FROM [BeckmanLIS].[dbo].[TestMappingMaster] m 
	CROSS APPLY (SELECT TOP 1 SpecimenCode,SpecimenName
	 FROM [BeckmanLIS].[dbo].[TestMappingMaster] c WHERE c.IsActive=1 
	 and m.HISTestCode = c.HISTestCode
	 and m.SpecimenCode = c.SpecimenCode
	) s	
	inner join [BeckmanLIS].[dbo].[EquipmentMaster] em on em.Id = m.EquipmentId
	WHERE m.IsActive=1

	IF OBJECT_ID('tempdb..#Results') IS NOT NULL
    DROP TABLE #Results

	select distinct
	req.HISRequestId as REQID
	,req.HISRequestNo as REQNO
	,req.IPNO
	,req.MRNO
	,req.HISTestCode as TESTID
	,req.HISTestName as TESTNM
	,req.SpecimenCode as SPECIMENCODE
	,req.SampleNo as BARCODE
	,coalesce(hm.HISParamCode, rd.LISParamCode,req.HISTestName) as PARAMCODE
	,isnull(hm.HISParamDescription,hpm.name) AS PARAMNAME
	,rd.LISParamValue as PARAMVALUE
	,rd.LISParamUnit as PARAMUNIT
	,pt.Name as PATIENTNAME
	,req.SampleCollectionDate as REQDT
	,res.ResultDate AS RESULTDT
	,getdate() AS CREATEDON
	,0 as SENTTOHIS
	, case when hm.HISParamCODE is null then 0 else 1 end as HASPARAM
	,pm.EquipmentId,pm.EquipmentName
	into #Results
	 from BeckmanLIS.dbo.TestRequestDetails req
	 inner join BeckmanLIS.dbo.TestResults res on res.TestRequestId = req.Id and req.SampleNo = res.SampleNo
	 inner join BeckmanLIS.dbo.TestResultDetails rd on res.Id = rd.TestResultId
	 inner join BeckmanLIS.dbo.PatientDetails pt on pt.id = req.PatientId
	 inner join #MAPPING pm on res.HISTestCode = pm.HISTestCode 
	 and res.LISTestCode = pm.LISTestCode and res.SpecimenCode = pm.SpecimenCode and res.EquipmentId = pm.EquipmentId
	 left join(select distinct LisParamCode, HisParamCode, HisParamDescription,HisTestCode 
	 from BeckmanLIS.dbo.HISParameterMaster) hm on hm.LISParamCode = rd.LISParamCode  and hm.HISTestCode=res.HistestCode
	 left join HisParamMaster hpm on hpm.code=rd.LisParamcode
	 where req.ReportStatus = 2 AND RES.CreatedOn >= convert(date, dateadd(day, -2, getdate())) -- retrive last two days data

	if Exists( Select 1 from #Results where TESTID='T0000184')
	begin 
		Delete from #Results where RIGHT(BARCODE,1)='F' and PARAMCODE ='SCMP0830'
		Delete from #Results where RIGHT(BARCODE,1)='P' and PARAMCODE ='SCMP0829'
	end


insert into Staging_TestResult (
	REQID
	,REQNO
	,IPNO
	,MRNO
	,TESTID
	,TESTNM
	,SPECIMENCODE
	,BARCODE
	,PARAMCODE
	,PARAMNAME
	,PARAMVALUE
	,PARAMUNIT
	,PATIENTNAME
	,REQDT
	,RESULTDT
	,CREATEDON
	,SENTTOHIS
	,HASPARAM
	,EquipmentId
	,Equipment)
select x.* from (	
	Select * from #Results
) x 
left join dbo.Staging_TestResult st on st.REQNO = x.REQNO and st.TESTID=x.TESTID and st.BARCODE=x.BARCODE
where st.id is null

	update st
	set st.PARAMUNIT = pm.Unit
	from Staging_TestResult st 
	inner join [dbo].[Staging_Testparameter] pm 
		on pm.TestId = st.TESTID and pm.ParameterCode = st.PARAMCODE and isnull(pm.Unit,'') <> ''
	where isnull(PARAMUNIT,'') = ''

	update st
	set st.PARAMUNIT = pm.Unit
	from Staging_TestResult st 
	inner join (
		select distinct ParameterCode,Unit from [dbo].[Staging_Testparameter]  
		where isnull(Unit,'') <> ''
	) pm on pm.ParameterCode = st.PARAMCODE
	where isnull(PARAMUNIT,'') = ''

	UPDATE  st 
	set st.PARAMVALUE=x.PARAMVALUE,
		st.SENTTOHIS = 7 -- for Oracle data update
	from dbo.Staging_TestResult as st
	 INNER JOIN #Results AS x 
	on st.REQNO = x.REQNO 
	and st.TESTID=x.TESTID 
	and st.BARCODE=x.BARCODE
	and st.PARAMCODE = x.PARAMCODE
	and st.SPECIMENCODE = x.SPECIMENCODE
	where st.PARAMVALUE <> x.PARAMVALUE
	
	IF OBJECT_ID(N'tempdb..#RESULT') IS NOT NULL
	BEGIN
		DROP TABLE #RESULT
	END

	CREATE TABLE #RESULT (
		ID bigint
	)

	insert into #RESULT
	select ID from dbo.Staging_TestResult where SENTTOHIS = 0
	
	insert into [NEOSOFT]..[LIS_ZO].LIS_TESTRESULT(
		   [Id]
		  ,[REQID]
		  ,[REQNO]
		  ,[IPNO]
		  ,[MRNO]
		  ,[TESTID]
		  ,[TESTNM]
		  ,[PATIENTNAME]
		  ,[SPECIMENCODE]
		  ,[BARCODE]
		  ,[PARAMCODE]
		  ,[PARAMNAME]
		  ,[PARAMVALUE]
		  ,[PARAMUNIT]
		  ,[REQDT]
		  ,[RESULTDT]
		  ,[CREATEDON]
		  ,[EQUIPMENTID]
		  ,[EQUIPMENT])
	SELECT st.[Id]
		  ,[REQID]
		  ,[REQNO]
		  ,[IPNO]
		  ,[MRNO]
		  ,[TESTID]
		  ,[TESTNM]
		  ,[PATIENTNAME]
		  ,[SPECIMENCODE]
		  ,[BARCODE]
		  ,[PARAMCODE]
		  ,[PARAMNAME]
		  ,[PARAMVALUE]
		  ,[PARAMUNIT]
		  ,[REQDT]
		  ,[RESULTDT]
		  ,[CREATEDON]
		  ,[EQUIPMENTID]
		  ,[EQUIPMENT]
	  FROM [dbo].[Staging_TestResult] st
	  inner join #RESULT t on t.ID = st.Id

	  update st
	  set st.SENTTOHIS = 1
	  FROM [dbo].[Staging_TestResult] st
	  inner join #RESULT t on t.ID = st.Id

	 insert into  [NEOSOFT]..[LIS_ZO].RESULT_UPDATED(ID,PARAMVALUE)
	 select ID,PARAMVALUE from dbo.Staging_TestResult where SENTTOHIS = 7


	 UPDATE st
	  set st.SENTTOHIS = 1
	  FROM [dbo].[Staging_TestResult] st
	  where SENTTOHIS = 7

	INSERT INTO [dbo].[ExecutionLog] (ProcudereName,Description,ExecutedOn) VALUES('[dbo].[usp_SyncTestResult]','LIS Test Result sync ends',getdate())
end