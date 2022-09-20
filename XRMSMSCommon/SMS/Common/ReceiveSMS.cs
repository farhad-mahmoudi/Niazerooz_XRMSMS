using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using XRMSMSCommon.SMS.Common;

namespace XRMSMSCommon.SMS.Common
{
    public static class ReceiveSMS
    {
        public static Guid InsertSMS(ReceiveSMSEntity receiveSmsEntity)
        {
            #region connection
            var connectionString = @"AuthType=AD;Url=https://xrm-dev.niazerooz.com/Develop;Domain=dev;UserName=waterzolal;Password=A6rtYiIIW5f3jfysIYm";
            CrmServiceClient conn = new CrmServiceClient(connectionString);

            IOrganizationService service;
            service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            #endregion

            Entity mySMS = new Entity("new_sms");
            mySMS["description"] = receiveSmsEntity.description;
            mySMS["new_mobile"] = receiveSmsEntity.new_mobile;
            mySMS["new_provider"] = new OptionSetValue(receiveSmsEntity.new_provider);
            mySMS["subject"] = receiveSmsEntity.subject;
       mySMS["new_senton"] = UnixTimeStampToDateTime(Convert.ToDouble(receiveSmsEntity.senton));
            mySMS["new_type"] = receiveSmsEntity.new_type;
            mySMS["new_destination"] = receiveSmsEntity.destination;

            return service.Create(mySMS);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
