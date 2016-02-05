using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallSelectorLib;

namespace CallSelectorLibTest
{
    class SelectorConfigImplTest : ISelectorConfig
    {
        #region SelectorConfig Members

        #region Main Properties

        public MailProtocol Protocol()
        {
            return MailProtocol.Pop3;
        }

        public DBType DbType()
        {
            return DBType.SQLServer;
        }
        public string DBConnectionString(){
            return "user id=call_selector;" +
                   "password=call_selector_pass;server=WS_ITPARK\\SMSDELIVERY;" +
                   "Trusted_Connection=yes;" +
                   "database=db_call;" +
                   "connection timeout=30";
        }

        public string HostName()
        {
            return "pop.mail.ru";
        }

        public int Port()
        {
            return 995;
        }

        public bool UseSSL()
        {
            return true;
        }

        public string Login()
        {
            return "mynameich";
        }

        public string Password()
        {
            return "auX_dR72gF";
        }

        public int HostRequestDelayMilliseconds()
        {
            return 1000;
        }

        public bool LogDebug()
        {
            return true;
        }

        public string Sender()
        {
            return "achernoivanov@gmail.com";
        }

        public string DirectoryForAudioFiles()
        {
            return "audioFiles";
        }

        public string AudioFileExtension()
        {
            return "mp3";
        }

        #endregion ~Main Properties


        #region Message Regular Expression and Formats


        //        String example = "Уважаемый клиент Манго Телеком!"
        //+ " В 11:15:18 06/02/2012 была произведена запись разговора с абонентом"
        //+ " 79879433512, вызванным с номера sip:asmirnov@vendosoft.mangosip.ru."
        //+ " Продолжительность общения: 16 сек."
        //+ " Открыв присоединенный к письму файл, Вы можете прослушать записанный разговор"
        //+ " Благодарим Вас за пользование услугами Манго Телеком.";

        public string MessageRegex()
        {
            return Utils.formatText("Уважаемый клиент Манго Телеком!"
                 + " В (\\d\\d:\\d\\d:\\d\\d \\d\\d/\\d\\d/\\d\\d\\d\\d) была произведена запись разговора с абонентом"
                 + " (\\d\\d\\d\\d\\d\\d\\d\\d\\d\\d\\d), вызванным с номера ([^ ]+)."
                 + " Продолжительность общения: ((\\d?\\d ч )?(\\d?\\d мин )?\\d?\\d сек)."
                 + " Открыв присоединенный к письму файл, Вы можете прослушать записанный разговор"
                 + " Благодарим Вас за пользование услугами Манго Телеком.");
        }

        public int RegexIndexOperator()
        {
            return 3;
        }
        public int RegexIndexAbonent()
        {
            return 2;
        }
        public int RegexIndexDateStart()
        {
            return 1;
        }
        public int RegexIndexDateInterval()
        {
            return 4;
        }

        public string[] FormatVariantsForDateStart()
        {
            return new string[] { "HH:mm:ss dd.MM.yyyy" };//NEVER SEPARATE Date BY SLASH '/', use dot '.' instead even if we have slash '/' in the regexp.
        }

        public string[] FormatVariantsForDateInterval()
        {
            return new string[] { "s сек", "m мин s сек", "H ч s сек", "H ч m мин s сек" };
        }

        #endregion ~Message Regular Expression and Formats


        #endregion
    }
}
