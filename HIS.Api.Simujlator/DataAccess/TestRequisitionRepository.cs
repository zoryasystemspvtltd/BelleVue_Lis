using HIS.Api.Simujlator.Models.DTO;
using HIS.Api.Simujlator.Models.Entity;
using LIS.DtoModel.Models.ExternalApi;
using LIS.Logger;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace HIS.Api.Simujlator.DataAccess
{
    internal class TestRequisitionRepository : ITestRequisitionRepository
    {
        private ILogger logger;
        public TestRequisitionRepository(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<TestDetail> GetTestRequisitions()
        {
            var testDetails = new List<TestDetail>();
            var parameters = new List<TestParameter>();
            var orders = new List<Order>();

            TestDetail test = new TestDetail();

            var testRequisitions = GenericRepository<StagingTestRequisition>
                .CreateInstance
                .Get(p => p.Acknowledged == 0);

            var distinctRequisitions = (from p in testRequisitions
                                        select new
                                        {
                                            p.RequesitionId,
                                            p.RequesitionNumber
                                        }
                                   ).Distinct().Select(x => new DistinctRequisition()
                                   {
                                       RequesitionId = x.RequesitionId,
                                       RequesitionNumber = x.RequesitionNumber

                                   })
           .ToList();

            if (testRequisitions != null)
            {
                foreach (var distinctRequisition in distinctRequisitions)
                {
                    var selectedRequisitions = testRequisitions
                        .Where(p => p.RequesitionId.Equals(distinctRequisition.RequesitionId, StringComparison.OrdinalIgnoreCase)
                                                   && p.RequesitionNumber.Equals(distinctRequisition.RequesitionNumber, StringComparison.OrdinalIgnoreCase));

                    var selectedRequisition = selectedRequisitions.FirstOrDefault();
                    var ymd = selectedRequisition.YMD;

                    var dob = ToDob(selectedRequisition.Age, selectedRequisition.YMD);
                    test = new TestDetail
                    {
                        PatientId = (!string.IsNullOrWhiteSpace(selectedRequisition.MRNumber) 
                                        ? selectedRequisition.MRNumber : 
                                    (!string.IsNullOrEmpty(selectedRequisition.IpNo) 
                                    ? selectedRequisition.IpNo: selectedRequisition.RequesitionNumber)),
                        PatientName = selectedRequisition.PatientName,
                        Gender = selectedRequisition.Sex,
                        RequisitionId = selectedRequisition.RequesitionId,
                        RequisitionNumber = selectedRequisition.RequesitionNumber,
                        SiteId = string.Empty,
                        DOB = dob,
                        BedNo = string.IsNullOrWhiteSpace(selectedRequisition.BedNumber) ? " " : selectedRequisition.BedNumber,
                        Department = selectedRequisition.DepartmentName,
                        DepartmentId = selectedRequisition.DepartmentId,
                        HISRequestId = selectedRequisition.RequesitionId,
                        HISRequestNo = selectedRequisition.RequesitionNumber,
                        IPNo = selectedRequisition.IpNo,
                        MRNo = selectedRequisition.MRNumber
                    };

                    orders = new List<Order>();
                    foreach (var requisition in selectedRequisitions)
                    {
                        var order = new Order
                        {
                            TestCode = requisition.TestId,
                            TestName = requisition.TestName,
                            BarcodeNo = requisition.RequesitionNumber,
                        };

                        var testParameters = GenericRepository<StagingTestparameter>
                            .CreateInstance
                            .Get(p => p.TestId.Equals(requisition.TestId, StringComparison.OrdinalIgnoreCase))
                            .Select(p => new { p.ParameterCode, p.Parameter })
                            .Distinct();

                        var lstParams = new List<TestParameter>();
                        foreach (var param in testParameters)
                        {
                            var parameter = new TestParameter
                            {
                                ParameterCode = param.ParameterCode,
                                Parameter = param.Parameter
                            };
                            lstParams.Add(parameter);
                        }
                        order.TestParameter = lstParams;

                        orders.Add(order);
                    }
                    test.Orders = orders;
                    testDetails.Add(test);
                }

            }
            return testDetails;
        }

        public bool SaveAcknowledgement(IEnumerable<TestRequisitionAcknowledgement> acknowledgements)
        {
            foreach (var acknowledgement in acknowledgements)
            {
                var requisition = GenericRepository<StagingTestRequisition>
                    .CreateInstance.Get(p => p.RequesitionId.Equals(acknowledgement.RequesitionId, StringComparison.OrdinalIgnoreCase)
                                        && p.RequesitionNumber.Equals(acknowledgement.RequesitionNumber, StringComparison.OrdinalIgnoreCase)
                                        && p.TestId.Equals(acknowledgement.TestId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                requisition.Acknowledged = 1;

                GenericRepository<StagingTestRequisition>.CreateInstance.Update(requisition);
            }

            return true;
        }

        public bool SaveTestResult(ResultDto result)
        {
            logger.LogDebug("SaveTestResult Started");
            try
            {
                var testResult = new Models.Entity.TestResult
                {
                    EquipmentId = result.TestResult.EquipmentId,
                    HISTestCode = result.TestResult.HISTestCode,
                    DoctorNote = result.TestResult.DoctorNote,
                    LISTestCode = result.TestResult.LISTestCode,
                    PatientId = result.TestResult.PatientId,
                    AuthorizationDate = result.TestResult.AuthorizationDate,
                    AuthorizedBy = result.TestResult.AuthorizedBy,
                    CreatedBy = result.TestResult.CreatedBy,
                    CreatedOn = result.TestResult.CreatedOn,
                    Id = result.TestResult.Id,
                    ResultDate = result.TestResult.ResultDate,
                    ReviewDate = result.TestResult.ReviewDate,
                    ReviewedBy = result.TestResult.ReviewedBy,
                    SampleCollectionDate = result.TestResult.SampleCollectionDate,
                    SampleNo = result.TestResult.SampleNo,
                    SampleReceivedDate = result.TestResult.SampleReceivedDate,
                    SpecimenCode = result.TestResult.SpecimenCode,
                    SpecimenName = result.TestResult.SpecimenName,
                    TechnicianNote = result.TestResult.TechnicianNote,
                    TestRequestId = result.TestResult.TestRequestId
                };

                var lstResultDetails = new List<Models.Entity.TestResultDetail>();
                foreach (var resultDetail in result.TestResultDetails)
                {
                    var testResultDetail = new Models.Entity.TestResultDetail
                    {
                        CreatedBy = resultDetail.CreatedBy,
                        CreatedOn = resultDetail.CreatedOn,
                        Id = resultDetail.Id,
                        LISParamCode = resultDetail.LISParamCode,
                        LISParamUnit = resultDetail.LISParamUnit,
                        LISParamValue = resultDetail.LISParamValue,
                        TestResultId = resultDetail.TestResultId
                    };

                    lstResultDetails.Add(testResultDetail);
                }

                GenericRepository<Models.Entity.TestResult>.CreateInstance.Insert(testResult);
                logger.LogDebug("SaveTestResult Saved");

                InsertTestResults(testResult);

                foreach (var resultDetail in lstResultDetails)
                {
                    logger.LogDebug("SaveTestResult Details Started {0}", resultDetail.TestResultId);
                    GenericRepository<TestResultDetail>.CreateInstance.Insert(resultDetail);

                    InsertTestResultDetails(resultDetail);

                    logger.LogDebug("SaveTestResult Details End");
                }


            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }

            logger.LogDebug("SaveTestResult End");
            return true;
        }

        /// <summary>
        /// This method save final approved test result in HIS Oraclr database
        /// </summary>
        /// <param name="testResult">Object of Models.Entity.TestResult</param>
        private void InsertTestResults(Models.Entity.TestResult testResult)
        {
            string constr = "User Id=LIS_ZO; Password=system32; Data Source=neosoft;";
            string ProviderName = "Oracle.ManagedDataAccess.Client";

            try
            {                
                DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

                using (DbConnection conn = factory.CreateConnection())
                {
                    if (conn is OracleConnection)
                    {
                        conn.ConnectionString = constr;
                        var objConn = conn as OracleConnection;

                        OracleCommand objCmd = new OracleCommand();

                        objCmd.Connection = objConn;
                        objCmd.CommandText = "InsertTESTRESULTS";
                        objCmd.CommandType = CommandType.StoredProcedure;

                        objCmd.Parameters.Add("P_ID", OracleDbType.Decimal).Value = testResult.Id;
                        objCmd.Parameters.Add("P_SAMPLENO", OracleDbType.Varchar2).Value = testResult.SampleNo;
                        objCmd.Parameters.Add("P_HISTESTCODE", OracleDbType.Varchar2).Value = testResult.HISTestCode;
                        objCmd.Parameters.Add("P_LISTESTCODE", OracleDbType.Varchar2).Value = testResult.LISTestCode;
                        objCmd.Parameters.Add("P_SPECIMENNAME", OracleDbType.Varchar2).Value = testResult.SpecimenName;
                        objCmd.Parameters.Add("P_RESULTDATE", OracleDbType.Date).Value = testResult.ResultDate.Date;
                        objCmd.Parameters.Add("P_SAMPLECOLLECTIONDATE", OracleDbType.Date).Value = testResult.SampleCollectionDate.Date;
                        objCmd.Parameters.Add("P_SAMPLERECEIVEDDATE", OracleDbType.Date).Value = testResult.SampleReceivedDate.Date;
                        objCmd.Parameters.Add("P_AUTHORIZATIONDATE", OracleDbType.Date).Value = testResult.AuthorizationDate.Value.Date;
                        objCmd.Parameters.Add("P_AUTHORIZEDBY", OracleDbType.Varchar2).Value = testResult.AuthorizedBy;
                        objCmd.Parameters.Add("P_REVIEWDATE", OracleDbType.Date).Value = testResult.ReviewDate.Value.Date;
                        objCmd.Parameters.Add("P_REVIEWEDBY", OracleDbType.Varchar2).Value = testResult.ReviewedBy;
                        objCmd.Parameters.Add("P_TECHNICIANNOTE", OracleDbType.Varchar2).Value = testResult.TechnicianNote;
                        objCmd.Parameters.Add("P_DOCTORNOTE", OracleDbType.Varchar2).Value = testResult.DoctorNote;
                        objCmd.Parameters.Add("P_CREATEDBY", OracleDbType.Varchar2).Value = testResult.CreatedBy;
                        objCmd.Parameters.Add("P_CREATEDON", OracleDbType.Date).Value = testResult.CreatedOn.Date;
                        objCmd.Parameters.Add("P_PATIENTID", OracleDbType.Decimal).Value = testResult.PatientId;
                        objCmd.Parameters.Add("P_TESTREQUESTID", OracleDbType.Decimal).Value = testResult.TestRequestId;
                        objCmd.Parameters.Add("P_EQUIPMENTID", OracleDbType.Decimal).Value = testResult.EquipmentId;

                        try
                        {
                            objConn.Open();
                            
                            objCmd.ExecuteNonQuery();
                            logger.LogDebug("TestResults data inserted.");
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                        }
                        objConn.Close();
                    }
                }                

            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
        /// <summary>
        /// This method save final approved test result in HIS Oraclr database
        /// </summary>
        /// <param name="testResultDetail">Object of Models.Entity.TestResult</param>
        private void InsertTestResultDetails(Models.Entity.TestResultDetail testResultDetail)
        {
            string constr = "User Id=LIS_ZO; Password=system32; Data Source=neosoft;";
            string ProviderName = "Oracle.ManagedDataAccess.Client";

            try
            {

                DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

                using (DbConnection conn = factory.CreateConnection())
                {
                    if (conn is OracleConnection)
                    {
                        conn.ConnectionString = constr;
                        var objConn = conn as OracleConnection;

                        OracleCommand objCmd = new OracleCommand();

                        objCmd.Connection = objConn;
                        objCmd.CommandText = "InsertTESTRESULTDETAILS";
                        objCmd.CommandType = CommandType.StoredProcedure;


                        objCmd.Parameters.Add("P_ID", OracleDbType.Decimal).Value = testResultDetail.Id;
                        objCmd.Parameters.Add("P_LISPARAMCODE", OracleDbType.Varchar2).Value = testResultDetail.LISParamCode;
                        objCmd.Parameters.Add("P_LISPARAMVALUE", OracleDbType.Varchar2).Value = testResultDetail.LISParamUnit;
                        objCmd.Parameters.Add("P_LISPARAMUNIT", OracleDbType.Varchar2).Value = testResultDetail.LISParamValue;
                        objCmd.Parameters.Add("P_CREATEDBY", OracleDbType.Varchar2).Value = testResultDetail.CreatedBy;
                        objCmd.Parameters.Add("P_CREATEDON", OracleDbType.Date).Value = testResultDetail.CreatedOn.Date;
                        objCmd.Parameters.Add("P_TESTRESULTID", OracleDbType.Decimal).Value = testResultDetail.TestResultId;

                        try
                        {
                            objConn.Open();
                           
                            objCmd.ExecuteNonQuery();
                            logger.LogDebug("TestResultDetails data inserted.");
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                        }
                        objConn.Close();
                    }
                }              

            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private string Age(DateTime dob)
        {
            DateTime now = DateTime.Today;
            int ageInMonth = (now.Year - dob.Year) * 12 + (now.Month - dob.Month);

            int ageYear = ageInMonth / 12;
            decimal ageMonth = ((decimal)(ageInMonth % 12)) / 100;

            decimal age = (decimal)ageYear + ageMonth;
            return age.ToString();
        }

        private string ToDob(decimal? age, string ymd)
        {
            DateTime dob = DateTime.Today;
            if (age == null)
            {
                age = 0;
            }
            var ageYmd = (int)age * -1;
            switch (ymd)
            {
                case "D":
                    dob = dob.AddDays(ageYmd);
                    break;
                case "M":
                    dob = dob.AddMonths(ageYmd);
                    break;
                default:
                    dob = dob.AddYears(ageYmd);
                    break;

            }
            return dob.ToString("dd/MM/yyyy");
        }
    }
}