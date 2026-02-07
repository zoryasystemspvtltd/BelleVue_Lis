using HIS.Api.Simujlator.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HIS.Api.Simujlator.Models.DTO
{
    public interface IResult
    {
        TestResult TestResult { get; set; }
        IEnumerable<TestResultDetail> ResultDetails { get; set; }
    }
    public class Result : IResult
    {
        public TestResult TestResult { get; set; }
        public IEnumerable<TestResultDetail> ResultDetails { get; set; }
    }
}