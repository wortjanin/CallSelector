using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenPop.Pop3;
using System.Diagnostics;
using OpenPop.Mime;
using System.Text.RegularExpressions;
using System.IO;

namespace CallSelectorLib.Impl
{
    /// <summary>
    /// Non-threadsafe
    /// </summary>
    public class CallDataProviderImplPop3 : CallDataProvider
    {
        private Pop3Client pop3Client;

        private int index = 0;
        private int count = 0;
        private ICallMessage NULL_MESSAGE;

        public CallDataProviderImplPop3(ISelectorConfig config)
            : base(config)
        {
            this.pop3Client = new Pop3Client();
            pop3Client.Connect(HostName, Port, useSSL);
            pop3Client.Authenticate(Login, Password);
            NULL_MESSAGE = new CallMessageImpl(pop3Client, 0, this);
            
        }

        public override bool HasMessages()
        {
            if (!pop3Client.Connected)
            {
                pop3Client.Dispose();
                pop3Client = new Pop3Client();
                pop3Client.Connect(HostName, Port, useSSL);
                pop3Client.Authenticate(Login, Password);
            }

            if (0 >= index)
            {
                index = count = pop3Client.GetMessageCount();
                if (0 == index)
                {
                    pop3Client.Disconnect();
                    pop3Client.Dispose();
                    pop3Client = new Pop3Client();
                    pop3Client.Connect(HostName, Port, useSSL);
                    pop3Client.Authenticate(Login, Password);
                }
                //for (int i = count; i >= 1; i -= 1)
            }
            return 0 < index;
        }

        
        public override ICallMessage NextMessage()
        {
            ICallMessage callMessage = NULL_MESSAGE;
            if (0 >= index) return callMessage;
            //create message
            try
            {
                callMessage = new CallMessageImpl(pop3Client, index, this);
                return callMessage;
            }
            finally
            {
                index--;
            }

        }

        #region  ICallMessage implementation

        private class CallMessageImpl : ICallMessage
        {
            private readonly CallDataProviderImplPop3 parent;
            private readonly Pop3Client pop3Client;
            private readonly int index;

            private string sender = "";
            private string oper = "";
            private DateTime dateTimeStart = DateTime.MinValue;
            private TimeSpan dateTimeInterval = TimeSpan.MaxValue;
            private string abonent = "";
            private string audioFileName = "";
            private string plainText = "";
            private string text = "";
            
            public bool isValid(){
                if ("".Equals(sender) ||
                    //"".Equals(oper) ||                              <---- these fields could be empty 
                    //DateTime.MinValue.Equals(dateTimeStart) ||      <---- because of incorrect mail template (regex),
                    //DateTime.MinValue.Equals(dateTimeInterval) ||   <---- they must be checked in the CallDataConsumer
                    //"".Equals(abonent) ||                           <---- and mail should be saved in special table
                    "".Equals(audioFileName) ||
                    "".Equals(plainText))
                   return false;
                return true;
            }

            public CallMessageImpl(Pop3Client pop3Client, int index, CallDataProviderImplPop3 parent)
            {
                this.parent = parent;
                this.pop3Client = pop3Client;
                this.index = index;

                if (0 >= index || null == pop3Client) 
                    return;
                 
                Message message = pop3Client.GetMessage(index);
                if (null == message)
                    return;

                if (null != message.Headers &&
                   null != message.Headers.From &&
                   null != message.Headers.From.Address)
                    this.sender = message.Headers.From.Address;
                
                Visit(message.FindFirstPlainTextVersion(), true);
                Visit(message.MessagePart, false);
                if(String.IsNullOrEmpty(this.plainText) && !String.IsNullOrEmpty(this.text))
                {
                    var txt = Regex.Replace(this.text, @"\s*<br\s*/*>\s*", @" ");
                    fillMsgData(txt, parent.MessageRegex);
                    this.plainText = this.text;
                }
            }


            #region CallMessage Members

            public string Sender()
            {
                return sender;
            }

            
            public string Operator()
            {
                return oper;
            }

            
            public DateTime DateTimeStart()
            {
                return dateTimeStart;
            }

            
            public TimeSpan DateTimeInterval()
            {
                return dateTimeInterval;
            }

            
            public string Abonent()
            {
                return abonent;
            }

            
            public string AudioFileName()
            {
                return audioFileName;
            }

            
            public string PlainText()
            {
                return plainText;
            }

            
            public string Text()
            {
                return text;
            }


            public bool SaveAudioFile(System.IO.FileInfo AudioFile)
            {
                if (null == AudioFile || null == fileMessagePart) 
                    return false;

                if (AudioFile.Exists)
                    AudioFile.Delete();
                fileMessagePart.Save(AudioFile);
                return true;
            }

            public void Delete()
            {
                if (0 >= index || null == pop3Client) return;
                pop3Client.DeleteMessage(index);
            }

            private MessagePart fileMessagePart = null;

            private void fillMsgData(string txt, string regex)
            {
                Match match = Regex.Match(txt, regex);
                if (match.Success)
                {
                    dateTimeStart = Utils.getDate(match.Groups[parent.RegexIndexDateStart].Value.Replace("/", "."), parent.FormatVariantsForDateStart);
                    abonent = match.Groups[parent.RegexIndexAbonent].Value;
                    oper = match.Groups[parent.RegexIndexOperator].Value;
                    dateTimeInterval = Utils.getDate(match.Groups[parent.RegexIndexDateInterval].Value.Replace("/", "."), parent.FormatVariantsForDateInterval).TimeOfDay;
                }
            }

            private void Visit(MessagePart MessagePart, bool isPlainText)
            {
                if (null == MessagePart) return;

                if (MessagePart.IsText)
                {
                    if (isPlainText)
                    {
                        plainText = Utils.formatText(MessagePart.GetBodyAsText());
                        fillMsgData(plainText, parent.MessageRegex);
                    }
                    else
                    {
                        text = Utils.formatText(MessagePart.GetBodyAsText());
                    }
                }
                else if (MessagePart.IsMultiPart)
                {
                    //log.Debug("MessagePart.IsMultiPart");
                    for (int i = 0; i < MessagePart.MessageParts.Count; i++)
                    {
                        Visit(MessagePart.MessageParts[i], false);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(MessagePart.FileName)
                        && MessagePart.FileName.EndsWith("." + parent.AudioFileExtension))
                    {
                        fileMessagePart = MessagePart;

                        audioFileName = (null == fileMessagePart) ? "" : fileMessagePart.FileName;

                        //log.Debug("FileName: " + MessagePart.FileName);
                        //string fileName = parent.DirectoryForAudioFiles + Path.DirectorySeparatorChar + MessagePart.FileName;
                        //if (File.Exists(fileName))
                        //    File.Delete(fileName);
                        //MessagePart.Save(new FileInfo(parent.DirectoryForAudioFiles + Path.DirectorySeparatorChar + MessagePart.FileName));
                    }
                }
            }


            #endregion
        }

        #endregion  ~ICallMessage implementation

        #region IDisposable Members

        public override void Dispose()
        {
            try
            {
                if (pop3Client.Connected)
                {
                    pop3Client.Disconnect();
                    pop3Client.Dispose();
                }
            }
            catch { }
        }

        #endregion

    }
}
