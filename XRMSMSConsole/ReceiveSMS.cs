using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using XRMSMSCommon.SMS.Common;

namespace XRMSMSConsole
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
            mySMS["senton"] = receiveSmsEntity.senton;
            mySMS["new_type"] = receiveSmsEntity.new_type;
            mySMS["destination"] = receiveSmsEntity.destination;

            return service.Create(mySMS);
        }
    }
}
