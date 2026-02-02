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
    public class PingController : ApiController
    {

        private ILogger logger;
        public PingController(ILogger logger)
        {
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                logger.LogInfo($"Ping API Request");
                return Ok(true);
            }
            catch (Exception e)
            {
                logger.LogException(e);
                return Ok(false);
            }
        }

    }

}
