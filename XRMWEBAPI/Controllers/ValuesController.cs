using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XRMSMSCommon.SMS.Common;

namespace XRMWEBAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        public void getsms(int id)
        {
        }

        [HttpGet]
        [Route("getsms")]
        public Guid GetSMS(string destination, string source, string receiveTime, string msgBody)


        {

            ReceiveSMSEntity receiveSmsEntity = new ReceiveSMSEntity()
            {

                description = msgBody,
                new_mobile = source,
                new_provider = 0,
                subject = $"{source} پیامک دریافتی از شماره ",
                senton = receiveTime,
                new_type = true,
                destination = destination

            };

            return ReceiveSMS.InsertSMS(receiveSmsEntity);

        }
    }
}
