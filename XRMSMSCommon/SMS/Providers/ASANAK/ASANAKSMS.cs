using XRMSMS.SMS.Common;
using RestSharp;
using System.Configuration;
using XRMSMSCommon.SMS;
using System.Collections.Generic;
using Newtonsoft.Json;
using XRMSMSCommon.SMS.Common;
using System;
using System.Linq;

namespace XRMSMS.SMS.Providers.ASANAK
{
    public class Asanaksms : INrSms
    {
        #region SendSMS

        /// <summary>
        /// Send SMS by ASANAK
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public SentSmsResult SendSms(Message msg, AsanakEntity asanakEntity)
        {
            SentSmsResult result = new SentSmsResult();

            var client1 = new RestClient(
                $"{asanakEntity.AsanakServiceUrl}" +
                $"/sendsms?username={asanakEntity.AsanakUserName}" +
                $"&password={asanakEntity.AsanakPassword}" +
                $"&source={asanakEntity.AsanakSourceNumber}" +
                $"&destination={msg.To}" +
                $"&message={msg.Body}"
            );

            client1.Timeout = -1;
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("Cookie", "asanak=htt128dc43evll7qit0vth4552");
            var response1 = client1.Execute(request1);
            result = ResponsSendHandeling(response1.Content);
            return result;
        }

        public SentSmsResult SendSms(Message msg, SentSmsResult sentSms, SmsirEntity sMsirEntity)
        {
            throw new System.NotImplementedException();
        }

        public SentSmsResult ResponsSendHandeling(string response)
        {
            SentSmsResult result = new SentSmsResult();
            if (response.Contains("["))
            {
                result.MsgID = response.Replace("[", "").Replace("]", "");
            }
            else
            {
                result = JsonConvert.DeserializeObject<SentSmsResult>(response);
            }
            return result;
        }

        #endregion

        private ResponseSent ResponseStatusHandeling(string response, ResponseSent responseSent)
        {
            ResponseObj obj;
            if (response.Contains("["))
            {
                response = response.Replace("[", "").Replace("]", "");
                obj = JsonConvert.DeserializeObject<ResponseObj>(response);
            }
            else
            {
                //obj = response;
            }

            return responseSent;
        }

        public ResponseSent SentResponse(double msgID, ResponseSent responseSent, AsanakEntity asanakEntity)
        {

            try
            {
                var client1 = new RestClient(
                    $"{asanakEntity.AsanakServiceUrl}" +
                    $"/msgstatus" +
                    $"?Username={asanakEntity.AsanakUserName}" +
                    $"&Password={asanakEntity.AsanakPassword}" +
                    $"&msgid={msgID}"
                    );
                client1.Timeout = -1;
                var request1 = new RestRequest(Method.POST);
                request1.AddHeader("Cookie", "asanak=htt128dc43evll7qit0vth4552");
                var response1 = client1.Execute(request1);
                
                var jsonObject = JsonConvert.DeserializeObject<List<ResponseObj>>(response1.Content)[0];

                switch (Convert.ToInt32(jsonObject.Status))
                {
                    case -1:
                        responseSent.StatusCode = 100000009;
                        responseSent.Detail = "پیام مورد نظر یافت نشد";
                        responseSent.State = StateEnum.Cancel;

                        break;
                    case 1:
                        responseSent.StatusCode = 100000001;
                        responseSent.Detail = "در صف ارسال به مخابرات";
                        responseSent.State = StateEnum.Open;
                        break;
                    case 4:
                        responseSent.StatusCode = 100000015;
                        responseSent.Detail = "در حال ارسال به مخابرات";
                        responseSent.State = StateEnum.Open;
                        break;
                    case 2:
                        responseSent.StatusCode = 100000015;
                        responseSent.Detail = "به مخابرات تحویل داده شد و شناسه پیام دریافت شد";
                        responseSent.State = StateEnum.Open;
                        break;
                    case 7:
                        responseSent.StatusCode = 100000010;
                        responseSent.Detail = "مخابرات پاسخی مبنی بر پذیرش پیام ارائه نداد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 8:
                        responseSent.StatusCode = 100000011;
                        responseSent.Detail = "مخابرات پیام را مردود اعالم کرد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 10:
                        responseSent.StatusCode = 100000012;
                        responseSent.Detail = "بخشی از پیام به مخابرات تحویل داده شد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 9:
                        responseSent.StatusCode = 100000012;
                        responseSent.Detail = "درحال تحویل دادن به مقصد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 6:
                        responseSent.StatusCode = 100000012;
                        responseSent.Detail = "ه مقصد تحویل داده شد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 5:
                        responseSent.StatusCode = 100000013;
                        responseSent.Detail = "ه مقصد تحویل داده نشد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 11:
                        responseSent.StatusCode = 100000014;
                        responseSent.Detail = "بخش هایی از پیام به مخابرات تحویل داده شده اما شناسه دریافت نکرده.";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 12:
                        responseSent.StatusCode = 100000012;
                        responseSent.Detail = "خشی از پیام به مقصد تحویل داده شد )این حالت مربوط به زمانی است\r\nکه گزارش تحویل برای برخی تکههای پیام از مخابرات دریافت نشده است و\r\nیا برخی ازتکههای پیام به مقصد تحویل داده نشدهاند(";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case 13:
                        responseSent.StatusCode = 100000010;
                        responseSent.Detail = "هیچ گزارش تحویل به مقصدی از مخابرات دریافت نشد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    default:
                        responseSent.StatusCode = 100000015;
                        responseSent.Detail = "هیچ گزارش تحویل به مقصدی از مخابرات دریافت نشد";
                        responseSent.State = StateEnum.Open;
                        break;

                }

            }
            catch (Exception ex)
            {
                responseSent.Detail = "کاربر گرامی سیستم با خطا مواجه شده است";
                responseSent.StatusCode = 100000016;
            }
            return responseSent;
        }

        public ReceiveSMSEntity ReceiveSMS(SmsirEntity sMsirEntity)
        {
            throw new NotImplementedException();
        }

        public ReceiveSMSEntity ReceiveSMS(AsanakEntity asanakEntity)
        {
            throw new NotImplementedException();
        }

    }
    public class ResponseObj
    {
        public string MsgID { get; set; }
        public string Status { get; set; }
    }
}
