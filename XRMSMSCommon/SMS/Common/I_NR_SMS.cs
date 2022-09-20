using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsIrRestful;
using XRMSMSCommon.SMS;
using XRMSMSCommon.SMS.Common;

namespace XRMSMS.SMS.Common
{
    public interface INrSms
    {
        SentSmsResult SendSms(Message msg, AsanakEntity asanakEntity);
        SentSmsResult SendSms(Message msg, SentSmsResult sentSms, SmsirEntity sMsirEntity);
        ReceiveSMSEntity ReceiveSMS(SmsirEntity sMsirEntity);
        ReceiveSMSEntity ReceiveSMS(AsanakEntity asanakEntity);
    }
}
