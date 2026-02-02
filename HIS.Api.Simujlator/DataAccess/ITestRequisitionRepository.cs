using HIS.Api.Simujlator.Models.DTO;
using LIS.DtoModel.Models.ExternalApi;
using System.Collections.Generic;

namespace HIS.Api.Simujlator.DataAccess
{
    public interface ITestRequisitionRepository
    {
        IEnumerable<TestDetail> GetTestRequisitions();
        bool SaveAcknowledgement(IEnumerable<TestRequisitionAcknowledgement> testRequisitions);

        bool SaveTestResult(ResultDto result);

    }
}
