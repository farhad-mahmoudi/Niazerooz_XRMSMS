using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using XRMSMS.SMS.Common;
using XRMSMS.SMS.Providers.ASANAK;
using XRMSMS.SMS.Providers.SMSIR;
using System;
using System.Activities;
using XRMSMSCommon.SMS;
using System.Configuration;

namespace NR_XRM_SendSMS
{
    public class SmsSenderWorkflow : CodeActivity
    {
        [RequiredArgument]
        [Input("SMSBody")]
        public InArgument<string> SmsBody { get; set; }

        [RequiredArgument]
        [Input("SMSNumber")]
        public InArgument<string> SmsNumber { get; set; }

        [RequiredArgument]
        [Input("SMSProvider")]
        public InArgument<string> SmsProvider { get; set; }


        [Output("SentStatus")]
        public OutArgument<string> SentStatus { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var tracer = executionContext.GetExtension<ITracingService>();
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            try 
            {
                switch (SmsProvider.Get(executionContext))
                {
                    case "SMSIR":

                        SmsirEntity smsIrEntity = new SmsirEntity();

                        smsIrEntity.SmsirSourceNumber = ConfigurationManager.AppSettings["SMSIRSourceNumber"];
                        smsIrEntity.SmsirSecretKey = ConfigurationManager.AppSettings["SMSIRSecretKey"];
                        smsIrEntity.SmsirUserApiKey = ConfigurationManager.AppSettings["SMSIRUserApiKey"];

                        SendNewSms(SmsBody.Get(executionContext), SmsNumber.Get(executionContext), SmsProviderType.Smsir, executionContext, smsIrEntity);

                        break;
                    case "ASANAK":

                        AsanakEntity asanakEntity = new AsanakEntity();
                        asanakEntity.AsanakSourceNumber = ConfigurationManager.AppSettings["AsanakSourceNumber"];
                        asanakEntity.AsanakServiceUrl = ConfigurationManager.AppSettings["AsanakServiceURL"];
                        asanakEntity.AsanakUserName = ConfigurationManager.AppSettings["AsanakUserName"];
                        asanakEntity.AsanakPassword = ConfigurationManager.AppSettings["AsanakPassword"];

                        SendNewSms(SmsBody.Get(executionContext), SmsNumber.Get(executionContext), SmsProviderType.Asanak, executionContext, asanakEntity);

                        break;

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SendNewSms(string body, string to, SmsProviderType provider, CodeActivityContext context, AsanakEntity asanakEntity)
        {

            Message msg = new Message()
            {
                Body = body,
                To = to,
            };
            SentSmsResult sMsResult = new SentSmsResult();

            INrSms asanakSms = new Asanaksms();
            asanakSms.SendSms(msg, asanakEntity);

           // SentStatus.Set(context, sMsResult.SendResult);

        }
        public void SendNewSms(string body, string to, SmsProviderType provider, CodeActivityContext context, SmsirEntity sMsirEntity)
        {

            Message msg = new Message()
            {
                Body = body,
                To = to,
            };

            INrSms smsIr = new Smsir();
            SentSmsResult sMsResult = new SentSmsResult();

            smsIr.SendSms(msg, sMsResult, sMsirEntity);

        }
    }
}