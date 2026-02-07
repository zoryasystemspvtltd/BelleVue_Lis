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
    public class AcknowledgeController : ApiController
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

        [AllowAnonymous]
        public HttpResponseMessage Post([FromBody]Acknowledge[] acknowledges)
        {
            var responseStrign = JsonConvert.SerializeObject(acknowledges);

            Logger.LogInstance.LogInfo($"Acknowledge Request {responseStrign}");

            //IsAuthenticate();
            foreach(var ack in acknowledges)
            {
                foreach(var o in ack.Orders)
                {
                    if (string.IsNullOrEmpty(o.TestCode))
                    {
                        Logger.LogInstance.LogInfo($"PreconditionFailed TestCode");
                        return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                    }

                    if (string.IsNullOrEmpty(o.BarcodeNo))
                    {
                        Logger.LogInstance.LogInfo($"PreconditionFailed BarcodeNo");
                        return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                    }

                    if (o.Status == null)
                    {
                        Logger.LogInstance.LogInfo($"PreconditionFailed Status");
                        return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                    }

                    foreach(var p in o.TestParameter)
                    {
                        if (string.IsNullOrEmpty(p.ParameterCode))
                        {
                            Logger.LogInstance.LogInfo($"PreconditionFailed ParameterCode");
                            return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                        }

                        if (p.Status == null)
                        {
                            Logger.LogInstance.LogInfo($"PreconditionFailed Status");
                            return Request.CreateResponse(HttpStatusCode.PreconditionFailed);
                        }
                    }
                }
            }

            Logger.LogInstance.LogInfo($"Acknowledge OK");
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }

    public class Acknowledge
    {
        public string PatientID { get; set; }
        public string SiteID { get; set; }
        public AcknowledgeOrders[] Orders { get; set; }
    }

    public class AcknowledgeOrders
    {
        public string TestCode { get; set; }
        public string TestName { get; set; }
        public string BarcodeNo { get; set; }
        public int? Status { get; set; }
        public AcknowledgeTestParameter[] TestParameter { get; set; }
    }

    public class AcknowledgeTestParameter
    {
        public string ParameterCode { get; set; }
        public string Parameter { get; set; }

        public int? Status { get; set; }
    }
}
