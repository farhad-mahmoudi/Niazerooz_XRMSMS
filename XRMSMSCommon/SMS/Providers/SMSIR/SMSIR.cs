using SmsIrRestful;
using System;
using System.Collections.Generic;
using XRMSMS.SMS.Common;
using System.Configuration;
using XRMSMSCommon.SMS;
using XRMSMSCommon.SMS.Common;

namespace XRMSMS.SMS.Providers.SMSIR
{
    public class Smsir : INrSms
    {

        #region Token

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <returns></returns>
        private string GenerateSmsIrToken(SmsirEntity sMsirEntity)
        {
            var smsDotIrUserApiKey = sMsirEntity.SmsirUserApiKey; // ConfigurationManager.AppSettings["SMSIRUserApiKey"];
            var smsDotIrSecretKey = sMsirEntity.SmsirSecretKey;   // ConfigurationManager.AppSettings["SMSIRSecretKey"];

            return new Token().GetToken(smsDotIrUserApiKey, smsDotIrSecretKey);
        }

        #endregion

        #region Send SMS

        /// <summary>
        /// Send SMS
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="sentSms"></param>
        /// <returns></returns>
        public SentSmsResult SendSms(Message msg, SentSmsResult sentSms, SmsirEntity sMsirEntity)
        {
            try
            {
                string str = GenerateSmsIrToken(sMsirEntity);
                MessageSendObject obj2 = SmsIrMessageSend(msg, sMsirEntity);
                MessageSendResponseObject obj3 = new MessageSend().Send(str, obj2);
                if (obj3.IsSuccessful)
                {
                    sentSms.MsgID = obj3.Ids[0].ID.ToString();

                }
                else if (obj3.Message == "IP شما معتبر نمی باشد . Token را دوباره درخواست کنید")
                {
                    str = GenerateSmsIrToken(sMsirEntity);
                    obj2 = SmsIrMessageSend(msg, sMsirEntity);
                    if (new MessageSend().Send(str, obj2).IsSuccessful)
                    {

                        sentSms.MsgID = obj3.Ids[0].ID.ToString();

                    }
                    else sentSms.Status = obj3.Message;
                }
                else
                    sentSms.Status = obj3.Message;
            }
            catch (Exception e)
            {
                sentSms.Status = e.Message;
            }
            return sentSms;
        }

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static MessageSendObject SmsIrMessageSend(Message msg, SmsirEntity sMsirEntity)
        {
            List<string> list1 = new List<string>();
            list1.Add(msg.Body);
            MessageSendObject obj1 = new MessageSendObject();
            obj1.Messages = list1.ToArray();
            List<string> list2 = new List<string>();
            list2.Add(msg.To);
            obj1.MobileNumbers = list2.ToArray();

            var smsDotIrLineNumber = sMsirEntity.SmsirSourceNumber;

            obj1.LineNumber = smsDotIrLineNumber;
            obj1.SendDateTime = new DateTime?(DateTime.Now);
            obj1.CanContinueInCaseOfError = true;
            return obj1;
        }

        public SentSmsResult SendSms(Message msg, AsanakEntity asanakEntity)
        {
            throw new NotImplementedException();
        }

        #endregion

        public ResponseSent SentResponse(int msgID, ResponseSent responseSent, SmsirEntity smsirEntity)
        {
            try
            {

                string tokenString = GenerateSmsIrToken(smsirEntity);
                SentMessageResponseById response = new MessageSend().GetById(tokenString, msgID);
                string status = response.Messages.DeliveryStateID.ToString();
                switch (status)
                {
                    case "1":
                        responseSent.StatusCode = 100000015;
                        responseSent.Detail = "منتظر ارسال ";
                        responseSent.State = StateEnum.Open;
                        break;
                    case "2":
                        responseSent.StatusCode = 100000010;
                        responseSent.Detail = "اطالعات دقیقی دریافت نشد";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case "3":
                        responseSent.StatusCode = 100000004;
                        responseSent.Detail = "رسیده به مخابرات";
                        responseSent.State = StateEnum.Open;
                        break;
                    case "4":
                        responseSent.StatusCode = 100000014;
                        responseSent.Detail = "نرسیده به مخابرات";
                        responseSent.State = StateEnum.Completed;

                        break;
                    case "5":
                        responseSent.StatusCode = 100000012;
                        responseSent.Detail = "رسیده به گوشی";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case "6":
                        responseSent.StatusCode = 100000013;
                        responseSent.Detail = "نرسیده به گوشی";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case "8":
                        responseSent.StatusCode = 100000013;
                        responseSent.Detail = "بروزخطا در دلیوری";
                        responseSent.State = StateEnum.Completed;
                        break;
                    case "7":
                        responseSent.StatusCode = 100000001;
                        responseSent.Detail = "در صف ارسال به مخابرات";
                        responseSent.State = StateEnum.Open;
                        break;
                    case "9":
                        responseSent.StatusCode = 100000011;
                        responseSent.Detail = "لیست سیاه مخابراتی";
                        responseSent.State = StateEnum.Completed;
                        break;
                    default:
                        responseSent.StatusCode = 100000015;
                        responseSent.Detail = "هیچ گزارش تحویل به مقصدی از مخابرات دریافت نشد";
                        responseSent.State = StateEnum.Open;
                        break;
                }
                return responseSent;
            }

            catch (Exception ex)
            {

                return (ResponseSent) null;
            }
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

}

