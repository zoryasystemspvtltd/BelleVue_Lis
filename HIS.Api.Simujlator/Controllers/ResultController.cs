using HIS.Api.Simujlator.DataAccess;
using HIS.Api.Simujlator.Models.DTO;
using LIS.DtoModel.Models.ExternalApi;
using LIS.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HIS.Api.Simujlator.Controllers
{
    public class ResultController : ApiController
    {
        private bool IsAuthenticate()
        {
            var userName = System.Web.HttpContext.Current.Request.Headers.GetValues("UserName");
            if (userName == null || userName.Count() == 0)
            {
                throw new KeyNotFoundException("Invalid UserName specified");
            }
            var password = System.Web.HttpContext.Current.Request.Headers.GetValues("Password");
            if (password == null || password.Count() == 0)
            {
                throw new KeyNotFoundException("Invalid Password specified");
            }

            Logger.LogInstance.LogInfo($"Authentication {userName[0]} {password[0]}");

            return true;
        }

        private ITestRequisitionRepository requisitionRepository;
        private ILogger logger;
        public ResultController(ITestRequisitionRepository requisitionRepository, ILogger logger)
        {
            this.requisitionRepository = requisitionRepository;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]ResultDto result)
        {
            var responseString = JsonConvert.SerializeObject(result);

            Logger.LogInstance.LogInfo($"Request Result {responseString}");
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.PreconditionFailed, ModelState.Keys);
            }
            var status = false;
            try
            {
                status = requisitionRepository.SaveTestResult(result);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            if (status)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }
        }
    }
}
