using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

namespace CallSelectorLib.Impl
{
    public class SelectorConfigImpl : ISelectorConfig
    {
        private MailProtocol protocol;
        private DBType dbType;
        private string hostName;
        private int port;
        private bool useSSL;
        private string login;
        private string password;
        private string sender;
        private string dbConnectionString;
        private int hostRequestDelayMilliseconds;
        private bool logDebug;
        private string directoryForAudioFiles;
        private string audioFileExtension;
        private string messageRegex;
        private int regexIndexOperator;
        private int regexIndexAbonent;
        private int regexIndexDateStart;
        private int regexIndexDateInterval;
        private string[] formatVariantsForDateStart;
        private string[] formatVariantsForDateInterval;

      
        /// <summary>
        /// Sample file: CallSelectorConfig.xml
        /// -----------------------------------------------------------
        /// <code>
        /// <?xml version="1.0" encoding="utf-8"?>
        ///<CallSelectorConfig xmlns="http://chernoivanov.org/SmsDeliveryTest/CallSelectorConfig.xsd">
        ///  <!--
        ///      Use only one service per one mailbox.
        ///      It is also necessary to allow only one sender, sending only callMessages, per mailbox,
        ///      since messages from all other senders or messages in different format (see MessageRegex element) 
        ///      will be treated as a spam and deleted permanently.
        ///    -->
        ///  <MailServer Protocol="Pop3">
        ///    <Host>pop.mail.ru</Host>
        ///    <Port>995</Port>
        ///    <UseSSL>true</UseSSL>
        ///    <Login>mynameich</Login>
        ///    <Password>auX_dR72gF</Password>
        ///  </MailServer>
        ///
        ///  <HostRequestDelayMilliseconds>1000</HostRequestDelayMilliseconds>
        ///  <Sender>achernoivanov@gmail.com</Sender>
        ///  <DirectoryForAudioFiles>MangoTelecomAudioFiles</DirectoryForAudioFiles>
        ///  <AudioFileExtension>mp3</AudioFileExtension>
        ///
        ///  <MessageStructure>
        ///    <!--  example:
        ///          Уважаемый клиент Манго Телеком!
        ///          В 11:15:18 06/02/2012 была произведена запись разговора с абонентом
        ///          79879433512, вызванным с номера sip:asmirnov@vendosoft.mangosip.ru.
        ///          Продолжительность общения: 16 сек.
        ///          Открыв присоединенный к письму файл, Вы можете прослушать записанный разговор
        ///          Благодарим Вас за пользование услугами Манго Телеком.    
        ///    -->
        ///    <MessageRegex>
        ///      Уважаемый клиент Манго Телеком!
        ///      В (\\d\\d:\\d\\d:\\d\\d \\d\\d/\\d\\d/\\d\\d\\d\\d) была произведена запись разговора с абонентом
        ///      (\\d\\d\\d\\d\\d\\d\\d\\d\\d\\d\\d), вызванным с номера ([^ ]+).
        ///      Продолжительность общения: ((\\d?\\d ч )?(\\d?\\d мин )?\\d?\\d сек).
        ///      Открыв присоединенный к письму файл, Вы можете прослушать записанный разговор
        ///      Благодарим Вас за пользование услугами Манго Телеком.
        ///    </MessageRegex>
        ///
        ///    <!-- The groups must be unique -->
        ///    <RegexGroups>
        ///      <Operator number="3"/>
        ///      <Abonent number="2"/>
        ///      <DateStart number="1"/>
        ///      <TimeInterval number="4"/>
        ///    </RegexGroups>
        ///
        ///    <!-- Never place slash, i.e. symbol '/'? into the FormatDateStart or FormatDateInterval
        ///            even if it is present in regex, use dot instead, which is symbol '.'
        ///            These elements are used to convert string in all possible cases of date format to C#'s DateTime object 
        ///           -->
        ///    <FormatDateStart>
        ///      <Case>HH:mm:ss dd.MM.yyyy</Case>
        ///    </FormatDateStart>
        ///    <FormatDateInterval>
        ///      <Case>s сек</Case>
        ///      <Case>m мин s сек</Case>
        ///      <Case>H ч s сек</Case>
        ///      <Case>H ч m мин s сек</Case>
        ///    </FormatDateInterval>
        ///  </MessageStructure>
        ///</CallSelectorConfig>
        /// </code>
        /// -----------------------------------------------------------
        /// </summary>
        /// <param name="configXml"></param>
        public SelectorConfigImpl(FileInfo configXml)
        {
            Debug.Assert(null != configXml);
            if (!configXml.Exists) throw new InvalidDataException("configXml file does not exists.");
            
            string xsd = "http://chernoivanov.org/SmsDeliveryTest";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configXml.FullName);
            XmlNamespaceManager names = new XmlNamespaceManager(xDoc.NameTable);
            names.AddNamespace("a", xsd);

            XmlNode node = xDoc.SelectSingleNode("//a:MailServer", names);
            this.protocol = (MailProtocol)Enum.Parse(typeof(MailProtocol), node.Attributes["protocol"].Value, true);
            this.hostName = node.SelectSingleNode("a:Host/text()", names).Value;
            this.port = Convert.ToInt32(node.SelectSingleNode("a:Port/text()", names).Value);
            this.useSSL = Convert.ToBoolean(node.SelectSingleNode("a:UseSSL/text()", names).Value);
            this.login = node.SelectSingleNode("a:Login/text()", names).Value;
            this.password = node.SelectSingleNode("a:Password/text()", names).Value;

            node = xDoc.SelectSingleNode("//a:DB", names);
            this.dbType = (DBType)Enum.Parse(typeof(DBType), node.Attributes["type"].Value, true);
            this.dbConnectionString = Utils.formatText(node.SelectSingleNode("a:ConnectionString/text()", names).Value).Replace("; ", ";");

            this.hostRequestDelayMilliseconds = Convert.ToInt32(xDoc.SelectSingleNode("//a:HostRequestDelayMilliseconds/text()", names).Value);
            this.logDebug = Convert.ToBoolean(xDoc.SelectSingleNode("//a:LogDebug/text()", names).Value);
            if (5 > this.hostRequestDelayMilliseconds)
                throw new InvalidDataException("5 > hostRequestDelayMilliseconds");

            this.sender = xDoc.SelectSingleNode("//a:Sender/text()", names).Value;
            this.directoryForAudioFiles = xDoc.SelectSingleNode("//a:DirectoryForAudioFiles/text()", names).Value;
            this.audioFileExtension = xDoc.SelectSingleNode("//a:AudioFileExtension/text()", names).Value;

            node = xDoc.SelectSingleNode("//a:MessageStructure", names);
            this.messageRegex = Utils.formatText(node.SelectSingleNode("a:MessageRegex/text()", names).Value);
            XmlNode n = node.SelectSingleNode("a:RegexGroups", names);
            regexIndexOperator     = Convert.ToInt32(n.SelectSingleNode("a:Operator", names).Attributes["number"].Value);
            regexIndexAbonent      = Convert.ToInt32(n.SelectSingleNode("a:Abonent", names).Attributes["number"].Value);
            regexIndexDateStart    = Convert.ToInt32(n.SelectSingleNode("a:DateStart", names).Attributes["number"].Value);
            regexIndexDateInterval = Convert.ToInt32(n.SelectSingleNode("a:TimeInterval", names).Attributes["number"].Value);
            
            iniVariants(node.SelectNodes("a:FormatDateStart/*", names), names, out formatVariantsForDateStart);
            iniVariants(node.SelectNodes("a:FormatDateInterval/*", names), names, out formatVariantsForDateInterval);

            directoryForAudioFiles = Utils.formatDirectoryName(directoryForAudioFiles);     

            if (!Directory.Exists(this.DirectoryForAudioFiles()))
                throw new InvalidDataException("" + this.DirectoryForAudioFiles() + " does not exists.");

            audioFileExtension = Utils.formatFileExtension(audioFileExtension);
        }

        private void iniVariants(XmlNodeList nodes, XmlNamespaceManager names, out string[] outStrings)
        {
            outStrings = new string[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
                outStrings[i] = nodes.Item(i).SelectSingleNode("text()", names).Value;
        }


        #region SelectorConfig Members

        public DBType DbType()
        {
            return dbType;
        }

        public MailProtocol Protocol()
        {
            return this.protocol;
        }

        public string HostName()
        {
            return this.hostName;
        }

        public int Port()
        {
            return this.port;
        }

        public bool UseSSL()
        {
            return this.useSSL;
        }

        public string Login()
        {
            return this.login;
        }

        public string Password()
        {
            return this.password;
        }


        public string Sender()
        {
            return this.sender;
        }

        public string DBConnectionString()
        {
            return this.dbConnectionString;
        }

        public int HostRequestDelayMilliseconds()
        {
            return this.hostRequestDelayMilliseconds;
        }
        public bool LogDebug()
        {
            return logDebug;
        }

        public string DirectoryForAudioFiles()
        {
            return this.directoryForAudioFiles;
        }

        public string AudioFileExtension()
        {
            return this.audioFileExtension;
        }

        public string MessageRegex()
        {
            return this.messageRegex;
        }

        public int RegexIndexOperator()
        {
            return this.regexIndexOperator;
        }

        public int RegexIndexAbonent()
        {
            return this.regexIndexAbonent;
        }

        public int RegexIndexDateStart()
        {
            return this.regexIndexDateStart;
        }

        public int RegexIndexDateInterval()
        {
            return this.regexIndexDateInterval;
        }

        public string[] FormatVariantsForDateStart()
        {
            return this.formatVariantsForDateStart;
        }

        public string[] FormatVariantsForDateInterval()
        {
            return this.formatVariantsForDateInterval;
        }

        #endregion
    }
}
