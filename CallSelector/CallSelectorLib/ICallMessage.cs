using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CallSelectorLib
{
    /// <summary>
    /// * оператор (sip:asmirnov@vendosoft.mangosip.ru)
    /// * время начала разговора (06/02/2012 11:15:18)
    /// * продолжительность (00:00:16)
    /// * номер абонента (79879433512)
    /// И на диск сохранять приложенный MP3-файл.
    /// </summary>
    public interface ICallMessage
    {
        bool isValid();

        string Sender();
        string PlainText();
        string Text();

        string Operator();
        DateTime DateTimeStart();
        TimeSpan DateTimeInterval();
        string Abonent();

        string AudioFileName();
        bool SaveAudioFile(FileInfo AudioFile);

        /// <summary>
        /// You must delete CallMessage immidiately after reading its parameters and saving the file
        /// </summary>
        void Delete();
    }
}
