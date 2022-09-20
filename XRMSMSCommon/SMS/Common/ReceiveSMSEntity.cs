using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRMSMSCommon.SMS.Common
{
    public class ReceiveSMSEntity
    {
        public bool new_type { get; set; }
        public string subject { get; set; }
        public Guid fromId { get; set; }
        public string description { get; set; }
        public int new_provider { get; set; }
        public string new_mobile{ get; set; }
        public string senton { get; set; }
        public string destination { get; set; }

    }
}
