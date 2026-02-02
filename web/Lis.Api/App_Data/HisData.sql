/*
delete from [dbo].[HISParameterRangMaster]
delete from [dbo].[HISParameterMaster]
delete from [dbo].[HISTestMaster]

DBCC CHECKIDENT ('HISParameterRangMaster', RESEED, 0)
DBCC CHECKIDENT ('HISParameterMaster', RESEED, 0)
DBCC CHECKIDENT ('HISTestMaster', RESEED, 0)

*/


INSERT INTO [dbo].[HISTestMaster]([HISTestCode],[HISTestCodeDescription],[HISSpecimenCode],[HISSpecimenName],[CreatedOn]) 
select distinct headid,headname,sampleTypeId,SampleName,getdate() from HisData

insert into [dbo].[HISParameterMaster](HISTestCode,HISParamCode,HISParamDescription,HisTestId,CreatedOn)
select distinct d.headid,d.paramid,d.parametername,t.id,getdate() from [dbo].[HISTestMaster] t
join HisData d on t.HISTestCode = d.headid
where paramid <> 'NULL'

insert into [dbo].[HISParameterRangMaster] (HISRangeCode,HISRangeValue,HisParameterId,CreatedOn,AgeFrom,AgeTo,MinValue,MaxValue)
select distinct RangeId,RangeName,t.Id,getdate(),0,0,0,0 from [dbo].[HISParameterMaster] t
join HisData d on t.HISTestCode = d.headid and t.HISParamCode = d.paramid
where RangeId <> 'NULL'

select * from [dbo].[HISTestMaster] t
left join [dbo].[HISParameterMaster] p on p.HisTestId = t.Id
left join [dbo].[HISParameterRangMaster] r on r.HisParameterId = p.Id
