using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallSelectorLib;
using log4net;
using System.IO;
using CallSelectorLib.Impl;

namespace CallSelectorLibTest
{
    class SelectorImplTest : Selector
    {
        protected static readonly log4net.ILog log = LogManager.GetLogger(typeof(SelectorImplTest));

        #region Selector Members

        //private SelectorConfig config = null;
        private readonly string DirectoryForAudioFiles;
        public SelectorImplTest(ISelectorConfig config) : 
            base(config)
        {
            this.DirectoryForAudioFiles = config.DirectoryForAudioFiles();
        }

        public override bool Run()
        {
            log.Debug("Starting...");

            int count = 4;
            while (base.callDataProvider.HasMessages())
            {
                ICallMessage message = base.callDataProvider.NextMessage();

                if (message.isValid())
                {
                    log.Debug("Sender: " + message.Sender());
                    log.Debug("message: ");
                    log.Debug(message.PlainText());

                    log.Debug("Operator: " + message.Operator());
                    log.Debug("Abonent: " + message.Abonent());
                    log.Debug("DateStart: " + message.DateTimeStart().ToString("HH:mm:ss dd.MM.yyyy"));
                    log.Debug("DateInterval: " + message.DateTimeInterval().ToString());

                    string fileName = DirectoryForAudioFiles + Path.DirectorySeparatorChar + message.AudioFileName();
                    log.Debug("FileName: " + fileName);
                    message.SaveAudioFile(new FileInfo(fileName));
                }
                count--;
                if (0 == count) break;// because we do not delete messages

            }
            base.callDataProvider.Dispose();

            //using (Pop3Client pop3Client = new Pop3Client())
            //{
            //    pop3Client.Connect(HostName, Port, useSSL);


            //    pop3Client.Authenticate(Login, Password);
            //    int count = pop3Client.GetMessageCount();

            //    log.Debug(String.Format("count == {0}", count));

            //    for (int i = count; i >= 1; i -= 1)
            //    {
            //        try
            //        {
            //            Message message = pop3Client.GetMessage(i);

            //            if (null == message || 
            //                null == message.Headers || 
            //                null == message.Headers.From || 
            //                null == message.Headers.From.Address ||
            //                !message.Headers.From.Address.Equals(this.Sender)) continue;

            //            log.Debug("message");
            //            log.Debug(String.Format("Headers.Subject == {0}", message.Headers.Subject));
            //            log.Debug(String.Format("Headers.Date == {0}", message.Headers.Date));

            //            log.Debug(String.Format("message.Headers.From == {0}", message.Headers.From));
            //            log.Debug(String.Format("message.Headers.From.MailAddress == {0}", message.Headers.From.MailAddress));
            //            log.Debug(String.Format("message.Headers.From.Address == {0}", message.Headers.From.Address));

            //            Visit(message.FindFirstPlainTextVersion(), true);
            //            Visit(message.MessagePart, false);
            //        }
            //        catch (Exception e)
            //        {
            //            log.Debug(
            //                "TestForm: Message fetching failed: " + e.Message + "\r\n" +
            //                "Exception details:\r\n",
            //                e);
            //        }
            //    }



            //    pop3Client.Disconnect();
            //}



            log.Debug("Finished!");

            System.Console.Read();

            return true;
        }

        private string formatText(string text)
        {
            return Utils.formatText(text);
        }

        private DateTime getDate(string stringDate, string[] variants)
        {
            return Utils.getDate(stringDate, variants);
        }

        //private void Visit(MessagePart MessagePart, bool printText)
        //{
        //    if (null == MessagePart) return;

        //    if (MessagePart.IsText && printText)
        //    {
        //        log.Debug("MessagePart");
        //        log.Debug("ContentType.MediaType: " + MessagePart.ContentType.MediaType);
        //        log.Debug("MessagePart.GetBodyAsText");

        //        string text = formatText(MessagePart.GetBodyAsText());

        //        Match match = Regex.Match(text, base.MessageRegex);
        //        if (match.Success)
        //        {
        //            log.Debug("DATE == " + getDate(match.Groups[base.RegexIndexDateStart].Value.Replace("/", "."), base.FormatVariantsForDateStart).ToString("HH:mm:ss dd.MM.yyyy"));//match.Groups[1].Value);//getDate(match.Groups[1].Value, dateStartVariants).ToString("HH:mm:ss"));
        //            log.Debug("PHONE == " + match.Groups[base.RegexIndexAbonent].Value);
        //            log.Debug("OPERATOR == " + match.Groups[base.RegexIndexOperator].Value);
        //            log.Debug("INTERVAL == " + getDate(match.Groups[base.RegexIndexDateInterval].Value.Replace("/", "."), base.FormatVariantsForDateInterval).ToString("HH:mm:ss"));
        //        }

        //        log.Debug(text); 


        //    }
        //    else if (MessagePart.IsMultiPart)
        //    {
        //        //log.Debug("MessagePart.IsMultiPart");
        //        for (int i = 0; i < MessagePart.MessageParts.Count; i++)
        //        {
        //            Visit(MessagePart.MessageParts[i], false);
        //        }
        //    }
        //    else
        //    {
        //        if (!String.IsNullOrEmpty(MessagePart.FileName)
        //            && MessagePart.FileName.EndsWith("." + AudioFileExtension))
        //        {
        //            log.Debug("FileName: " + MessagePart.FileName);
        //            string fileName = DirectoryForAudioFiles + Path.DirectorySeparatorChar + MessagePart.FileName;
        //            if (File.Exists(fileName))
        //                File.Delete(fileName);
        //            MessagePart.Save(new FileInfo(DirectoryForAudioFiles + Path.DirectorySeparatorChar + MessagePart.FileName));
        //        }
        //    }
        //}

        public override void Stop()
        {

        }

        public override void setIErrorHandler(IErrorHandler errorHandler)
        {

        }

        #endregion

    }
}
