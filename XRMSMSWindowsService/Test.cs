using System;
using System.Configuration;
using System.ServiceModel.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using XRMSMS.SMS.Common;
using XRMSMS.SMS.Providers.ASANAK;
using XRMSMSCommon.SMS;
using XRMSMSWindowsService;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        public static void SmsSender(string smsBody, string smsNumber)
        {
            var asanakEntity = new AsanakEntity
            {
                AsanakSourceNumber = ConfigurationManager.AppSettings["AsanakSourceNumber"],
                AsanakServiceUrl = ConfigurationManager.AppSettings["AsanakServiceURL"],
                AsanakUserName = ConfigurationManager.AppSettings["AsanakUserName"],
                AsanakPassword = ConfigurationManager.AppSettings["AsanakPassword"]
            };

            SenNewSms(smsBody, smsNumber, SmsProviderType.Asanak, asanakEntity);
        }

        public static void SenNewSms(string body, string to, SmsProviderType provider, AsanakEntity asanakEntity)
        {
            var msg = new Message()
            {
                Body = body,
                To = to,
            };
            var smsResult = new SentSmsResult();

            INrSms asanakSms = new Asanaksms();
            asanakSms.SendSms(msg,  asanakEntity);


        }
        //public UnitTest1()
        //{
        //    var CrmDomain = "niazerooz";
        //    var CrmPassword = "Fm_0550063633";
        //    var CrmServiceUrl = "https://xrm.niazerooz.com/root/XRMServices/2011/Organization.svc";
        //    var CrmUsername = "f.mahmoudi";


        //    ClientCredentials credentials = new System.ServiceModel.Description.ClientCredentials();
        //    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(CrmUsername, CrmPassword, CrmDomain);

        //    _orgService = new OrganizationServiceProxy(new Uri(CrmServiceUrl), null, credentials, null);
        //    _orgService.Timeout = new TimeSpan(0, 2, 0);

        //    _appService = new App(_orgService);
        //}

        [TestMethod]
        public void TestMethod1()
        {

            EntityReference bulksms = new EntityReference("bmsd_bulksms", new Guid("F535ABA4-5253-EC11-B493-00155D655039"));
            SendSmsService s = new SendSmsService();
            SmsSender("سلام", "09357004085");
        }
    }
}