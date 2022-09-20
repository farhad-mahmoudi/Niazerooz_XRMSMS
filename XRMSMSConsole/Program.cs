using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using XRMSMS.SMS.Common;
using XRMSMS.SMS.Providers.ASANAK;
using XRMSMS.SMS.Providers.SMSIR;
using XRMSMSCommon.SMS;
using XRMSMSCommon.SMS.Common;
using Message = XRMSMS.SMS.Common.Message;

namespace XRMSMSConsole
{
    internal class Program
    {

        private System.Timers.Timer _timer;

        static void Main(string[] args)
        {
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(15000D);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

            File.AppendAllText("D:\\log.txt", "moosa");
            timer.Start();
            Console.ReadKey();
        }


        public int Counter { get; set; }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {



            #region connection
            var connectionString = @"AuthType=AD;Url=https://xrm-dev.niazerooz.com/Develop;Domain=dev;UserName=waterzolal;Password=A6rtYiIIW5f3jfysIYm";
            CrmServiceClient conn = new CrmServiceClient(connectionString);

            IOrganizationService service;
            service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            #endregion

            GetSMS(service);



        }

        private static void GetSMS(IOrganizationService service)
        {
            #region ارسال پیامک 
            var dateNow = DateTime.UtcNow;

            QueryExpression qe = new QueryExpression("new_sms");
            qe.Criteria.AddCondition("scheduledstart", ConditionOperator.OnOrBefore, dateNow);
            qe.Criteria.AddCondition("scheduledend", ConditionOperator.OnOrAfter, dateNow);
            qe.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 100000000);
            qe.Criteria.AddCondition("new_trigger", ConditionOperator.Equal, true);
            qe.Criteria.AddCondition("new_messageid", ConditionOperator.Null);
            qe.ColumnSet = new ColumnSet(true);
            EntityCollection smses = service.RetrieveMultiple(qe);

            if (smses.Entities.Count > 0)
            {
                foreach (var item in smses.Entities)
                {
                    SentSmsResult sendResult = new SentSmsResult();

                    switch (((OptionSetValue)item["new_provider"]).Value)
                    {
                        case 1:
                            sendResult = SmsSender("SMSIR", (string)item["description"], (string)item["new_mobile"]);
                            break;
                        case 0:
                            sendResult = SmsSender("ASANAK", (string)item["description"], (string)item["new_mobile"]);
                            break;

                    }

                    //  item.cre06_sms_status = 1;
                    Entity myEntity = new Entity(item.LogicalName, item.Id);
                    if (sendResult.MsgID != null)
                    {
                        myEntity["new_messageid"] = sendResult.MsgID;
                        myEntity["statuscode"] = new OptionSetValue(100000015);
                        myEntity["new_senton"] = DateTime.Now;

                    }
                    if (sendResult.Status != null)
                    {
                        myEntity["new_sendstatus"] = sendResult.Status;
                        myEntity["statuscode"] = new OptionSetValue(100000009);
                    }
                    service.Update(myEntity);
                }

                // crmContext.SaveChanges();

            }
            #endregion

            #region وضعیت پیامک ارسالی

            //پیامک های ارسال شده 
            QueryExpression queryExpression = new QueryExpression("new_sms");
            queryExpression.Criteria.AddCondition("new_messageid", ConditionOperator.NotNull);
            FilterExpression fe = queryExpression.Criteria.AddFilter(LogicalOperator.Or);
            queryExpression.ColumnSet = new ColumnSet(true);
            fe.AddCondition("statuscode", ConditionOperator.Equal, 100000015);
            fe.AddCondition("statuscode", ConditionOperator.Equal, 100000001);
            fe.AddCondition("statuscode", ConditionOperator.Equal, 100000004);
            EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Count > 0)
            {
                foreach (var item in entityCollection.Entities)
                {
                    Entity myEntity = new Entity(item.LogicalName, item.Id);
                    ResponseSent sentOutput = new ResponseSent();
                    switch (((OptionSetValue)item["new_provider"]).Value)
                    {
                        case 1: //sms.ir
                            var smsIrEntity = new SmsirEntity
                            {
                                SmsirSourceNumber = ConfigurationManager.AppSettings["SMSIRSourceNumber"],
                                SmsirSecretKey = ConfigurationManager.AppSettings["SMSIRSecretKey"],
                                SmsirUserApiKey = ConfigurationManager.AppSettings["SMSIRUserApiKey"]
                            };

                            Smsir smsIR = new Smsir();
                            sentOutput = smsIR.SentResponse(Convert.ToInt32(item["new_messageid"]), new ResponseSent(), smsIrEntity);

                            break;
                        case 0: //Asanak
                            var asanakEntity = new AsanakEntity
                            {
                                AsanakSourceNumber = ConfigurationManager.AppSettings["AsanakSourceNumber"],
                                AsanakServiceUrl = ConfigurationManager.AppSettings["AsanakServiceURL"],
                                AsanakUserName = ConfigurationManager.AppSettings["AsanakUserName"],
                                AsanakPassword = ConfigurationManager.AppSettings["AsanakPassword"]
                            };

                            Asanaksms asanakSms = new Asanaksms();
                            sentOutput = asanakSms.SentResponse(Convert.ToDouble(item["new_messageid"]), new ResponseSent(), asanakEntity);


                            break;

                    }

                    if (((OptionSetValue)item["statecode"]).Value != (int)sentOutput.State)
                    {
                        var state = new SetStateRequest();
                        state.EntityMoniker = myEntity.ToEntityReference();
                        state.State = new OptionSetValue((int)sentOutput.State);
                        state.Status = new OptionSetValue((int)sentOutput.StatusCode);
                        var stateSet = (SetStateResponse)service.Execute(state);
                    }
                    else
                    {

                        myEntity["statuscode"] = new OptionSetValue(sentOutput.StatusCode);
                    }

                    myEntity["new_sendstatus"] = sentOutput.Detail;
                    service.Update(myEntity);

                }
            }
            #endregion
        }

        public static SentSmsResult SmsSender(string smsProvider, string smsBody, string smsNumber)
        {
            SentSmsResult result = new SentSmsResult();
            switch (smsProvider)
            {
                case "SMSIR":

                    var smsIrEntity = new SmsirEntity
                    {
                        SmsirSourceNumber = ConfigurationManager.AppSettings["SMSIRSourceNumber"],
                        SmsirSecretKey = ConfigurationManager.AppSettings["SMSIRSecretKey"],
                        SmsirUserApiKey = ConfigurationManager.AppSettings["SMSIRUserApiKey"]
                    };

                    result = SenNewSms(smsBody, smsNumber, SmsProviderType.Smsir, smsIrEntity);

                    break;

                case "ASANAK":

                    var asanakEntity = new AsanakEntity
                    {
                        AsanakSourceNumber = ConfigurationManager.AppSettings["AsanakSourceNumber"],
                        AsanakServiceUrl = ConfigurationManager.AppSettings["AsanakServiceURL"],
                        AsanakUserName = ConfigurationManager.AppSettings["AsanakUserName"],
                        AsanakPassword = ConfigurationManager.AppSettings["AsanakPassword"]
                    };

                    result = SenNewSms(smsBody, smsNumber, SmsProviderType.Asanak, asanakEntity);

                    break;
            }
            return result;

        }

        public static SentSmsResult SenNewSms(string body, string to, SmsProviderType provider, AsanakEntity asanakEntity)
        {
            var msg = new Message()
            {
                Body = body,
                To = to,
            };
            var smsResult = new SentSmsResult();

            INrSms asanakSms = new Asanaksms();
            smsResult = asanakSms.SendSms(msg, asanakEntity);
            return smsResult;

        }

        public static SentSmsResult SenNewSms(string body, string to, SmsProviderType sMsir, SmsirEntity smsIrEntity)
        {

            var msg = new Message()
            {
                Body = body,
                To = to,
            };

            INrSms smsIr = new Smsir();
            var smSResult = new SentSmsResult();

            smSResult = smsIr.SendSms(msg, smSResult, smsIrEntity);
            return smSResult;

        }

    }

}
