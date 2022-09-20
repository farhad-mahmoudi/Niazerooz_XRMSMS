using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XRMSMS.SMS.Common;
using XRMSMS.SMS.Providers.ASANAK;
using XRMSMS.SMS.Providers.SMSIR;
using XRMSMSCommon.SMS;


namespace XRMSMSWindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {


            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new SendSmsService()
            };
            ServiceBase.Run(servicesToRun);


        }

    }
}
