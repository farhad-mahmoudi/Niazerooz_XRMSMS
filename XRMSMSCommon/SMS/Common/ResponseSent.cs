using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMSMSCommon.SMS.Common
{
    public class ResponseSent
    {
        public int StatusCode { get; set; }
        public string Detail { get; set; }
        public StateEnum State { get; set; }
    }
}
