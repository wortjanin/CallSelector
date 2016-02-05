using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using CallSelectorLib;
using System.IO;

namespace CallSelectorWeb.Common
{
    public class Const
    {
        private static readonly IWebConfig iwebConfig;
        static Const()
        {
            string fileName = HostingEnvironment.MapPath("~/App_Data/CallSelectorConfigServer.xml");
            iwebConfig = CallSelectorFactory.loadIWebConfig(new FileInfo(fileName));
        }

        public static string DBServerString
        {
            get
            {
                return iwebConfig.DBConnectionString();
            }
        }

        public static class DateFormat
        {
            public static string Value
            {
                get
                {
                    return "dd.MM.yyyy";
                }
            }

            public static string Error
            {
                get
                {
                    return "31.12.2014";
                }
            }

            public static string Regex
            {
                get
                {
                    return "(([012][1-9])|([123]0)|(31))[.]((0[1-9])|(1[12]))[.][0-9][0-9][0-9][0-9]";
                }
            }

            public static string jFormat
            {
                get
                {
                    return "dd.mm.yy";
                }
            }
        }

        public readonly static string TimeFormatError = "Правильный формат: ЧЧ:мм:cc";
    }
}
