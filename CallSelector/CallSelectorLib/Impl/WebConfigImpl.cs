using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace CallSelectorLib.Impl
{
    public class WebConfigImpl : IWebConfig
    {
        private readonly DBType dbType;
        private readonly string dbConnectionString;
        private readonly Dictionary<string, IWebFileConfig> senderConfigDictionary = new Dictionary<string, IWebFileConfig>();

        public WebConfigImpl(FileInfo fileInfo)
        {
            Debug.Assert(null != fileInfo);
            if (!fileInfo.Exists) throw new InvalidDataException("fileInfo file does not exists.");

            string xsd = "http://chernoivanov.org/SmsDeliveryTest";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(fileInfo.FullName);
            XmlNamespaceManager names = new XmlNamespaceManager(xDoc.NameTable);
            names.AddNamespace("a", xsd);

            XmlNode node = xDoc.SelectSingleNode("//a:DB", names);
            this.dbType = (DBType)Enum.Parse(typeof(DBType), node.Attributes["type"].Value, true);
            this.dbConnectionString = Utils.formatText(node.SelectSingleNode("a:ConnectionString/text()", names).Value).Replace("; ", ";");

            XmlNodeList nodes = xDoc.SelectNodes("//a:AudioFiles/*", names);

            if (0 == nodes.Count)
                throw new InvalidDataException("No nodes in AudioFiles");
            foreach (XmlNode n in xDoc.SelectNodes("//a:AudioFiles/*", names))
            {
                WebFileConfigImpl fileCfg = new WebFileConfigImpl(
                    n.Attributes["DirectoryForAudioFiles"].Value, 
                    n.Attributes["AudioFileExtension"].Value);
                senderConfigDictionary.Add(n.Attributes["MailSender"].Value, fileCfg);
            }
        }

        private class WebFileConfigImpl : IWebFileConfig
        {
            private readonly string directoryForAudioFiles;
            private readonly string audioFileExtension;

            public WebFileConfigImpl(string directoryForAudioFiles, string audioFileExtension)
            {
                this.directoryForAudioFiles = Utils.formatDirectoryName(directoryForAudioFiles);
                this.audioFileExtension = Utils.formatFileExtension(audioFileExtension);
            }

            public string DirectoryForAudioFiles()
            {
                return directoryForAudioFiles;
            }

            public string AudioFileExtension()
            {
                return audioFileExtension;
            }


        }


        #region IWebConfig Members

        public DBType DbType()
        {
            return dbType;
        }

        public string DBConnectionString()
        {
            return dbConnectionString;
        }

        public Dictionary<string, IWebFileConfig> SenderConfigDictionary()
        {
            return senderConfigDictionary;
        }

        #endregion  ~IWebConfig Members
    }
}
