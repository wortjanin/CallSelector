using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CallSelectorLib.Impl
{
    /// <summary>
    /// One provider per one mailbox is allowed. 
    /// Multiple providers on one mailbox will result in errors.
    /// One sender per one mailbox, all others will be deleted as a spam
    /// </summary>
    public abstract class CallDataProvider : ICallDataProvider 
    {
        #region Main Parameters

        protected string HostName;
        protected int Port;
        protected bool useSSL;

        protected string Login;
        protected string Password;

        protected string Sender;

        protected int HostRequestDelayMilliseconds;

        protected string DirectoryForAudioFiles;
        protected string AudioFileExtension;

        #endregion ~Main Parameters


        #region Message Regular Expression and Formats

        protected string MessageRegex;

        protected int RegexIndexOperator;
        protected int RegexIndexAbonent;
        protected int RegexIndexDateStart;
        protected int RegexIndexDateInterval;

        protected string[] FormatVariantsForDateStart;
        protected string[] FormatVariantsForDateInterval;

        #endregion ~Message Regular Expression and Formats


        public CallDataProvider(ISelectorConfig config)
        {
            Debug.Assert(null != config);
            //this.config = config;

            this.HostName = config.HostName();
            this.Port = config.Port();
            this.useSSL = config.UseSSL();

            this.Login = config.Login();
            this.Password = config.Password();

            this.Sender = config.Sender();

            this.HostRequestDelayMilliseconds = config.HostRequestDelayMilliseconds();

            this.DirectoryForAudioFiles = config.DirectoryForAudioFiles();
            this.AudioFileExtension = config.AudioFileExtension();


            this.MessageRegex = config.MessageRegex();

            this.RegexIndexOperator = config.RegexIndexOperator();
            this.RegexIndexAbonent = config.RegexIndexAbonent();
            this.RegexIndexDateStart = config.RegexIndexDateStart();
            this.RegexIndexDateInterval = config.RegexIndexDateInterval();

            this.FormatVariantsForDateStart = config.FormatVariantsForDateStart();
            this.FormatVariantsForDateInterval = config.FormatVariantsForDateInterval();
        }

        public abstract bool HasMessages();
        public abstract ICallMessage NextMessage();

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
