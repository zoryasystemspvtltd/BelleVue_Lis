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
using System.Web.Http.Description;

namespace HIS.Api.Simujlator.Controllers
{
    public class OrderController : ApiController
    {
        private bool IsAuthenticate()
        {
            try
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
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
                return false;
            }

            return true;
        }

        private ITestRequisitionRepository requisitionRepository;
        private ILogger logger;
        public OrderController(ITestRequisitionRepository requisitionRepository, ILogger logger)
        {
            this.requisitionRepository = requisitionRepository;
            this.logger = logger;
        }

        [AllowAnonymous]
        [ResponseType(typeof(TestDetail[]))]
        [HttpGet]
        public IHttpActionResult Get()
        {
            IEnumerable<TestDetail> data = null;

            try
            {

                data = requisitionRepository.GetTestRequisitions();
                var responseStrign = JsonConvert.SerializeObject(data);

                Logger.LogInstance.LogInfo($"Order Response {responseStrign}");
            }
            catch (Exception ex)
            {
                Logger.LogInstance.LogException(ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok(data.ToArray());
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage Post(TestAcknowledgement[] acknowledgements)
        {
            var responseStrign = JsonConvert.SerializeObject(acknowledgements);

            Logger.LogInstance.LogInfo($"Acknowledge Request {responseStrign}");


            var testRequisitions = new List<TestRequisitionAcknowledgement>();
            foreach (var acknowledgement in acknowledgements)
            {
                var req = new TestRequisitionAcknowledgement
                {
                    RequesitionId = acknowledgement.RequesitionId,
                    RequesitionNumber = acknowledgement.RequesitionNumber,
                    TestId = acknowledgement.TestId
                };
                testRequisitions.Add(req);
            }

            var result = this.requisitionRepository.SaveAcknowledgement(testRequisitions);

            if (result)
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
