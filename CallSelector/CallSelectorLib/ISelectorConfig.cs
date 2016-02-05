using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallSelectorLib
{
    public enum MailProtocol
    {
        Pop3
    }

    public enum DBType
    {
        SQLServer
    }


    public interface ISelectorConfig
    {
        #region Main Properties


        MailProtocol Protocol();
        string HostName();
        int Port();
        bool UseSSL();

        string Login();
        string Password();

        DBType DbType();
        string DBConnectionString();

        string Sender();

        int HostRequestDelayMilliseconds();
        bool LogDebug();

        string DirectoryForAudioFiles();
        string AudioFileExtension();

        

        #endregion ~Main Properties


        #region Message Regular Expression and Formats

        string MessageRegex();

        int RegexIndexOperator();
        int RegexIndexAbonent();
        int RegexIndexDateStart();
        int RegexIndexDateInterval();

        string[] FormatVariantsForDateStart();
        string[] FormatVariantsForDateInterval();

        #endregion ~Message Regular Expression and Formats

    }
}
