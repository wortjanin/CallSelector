using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace CallSelectorLib.Impl
{
    public class CallDataConsumerImplSqlServer : ICallDataConsumer
    {
        private SqlConnection connection = null;
        private string directoryForAudioFiles;
        public CallDataConsumerImplSqlServer(ISelectorConfig config)
        {
            this.connection = new SqlConnection(Utils.formatText(config.DBConnectionString()));
            this.connection.Open();
            this.directoryForAudioFiles = config.DirectoryForAudioFiles();
        }

        #region CallDataConsumer Members


        private class OperatorTimeChecker
        {
            private object id_operator = -1;

            private readonly CallDataConsumerImplSqlServer parent;
            internal OperatorTimeChecker(ICallMessage callMessage, CallDataConsumerImplSqlServer parent)
            {// constructed to hold id_operator number, to save time
                this.parent = parent;

                string oper = callMessage.Operator();
                if (!setIdOperator(oper, ref id_operator))
                {
                    //insert, get id
                    string query = "INSERT INTO operator(mail) VALUES (@mail);SELECT Scope_Identity()";
                    id_operator = parent.ExecScalar(query, new SqlParameter[] {new SqlParameter("@mail", oper)});
                }
            }

            private bool setIdOperator(string oper, ref object id_operator)
            {
                DataTable table = parent.ExecReader(
                    "SELECT id_operator, mail FROM operator WHERE mail = @mail",
                    new SqlParameter[] { new SqlParameter("@mail", oper) });

                foreach (DataRow row in table.Rows)
                { // <=1 because of unique constraint in the DB
                    id_operator = row["id_operator"];
                    break;
                }
                return 1 == table.Rows.Count;
            }

            
            internal bool isValid(ICallMessage callMessage)
            {
                if (callMessage.DateTimeInterval().Equals(TimeSpan.Zero)) 
                    return false;

                DataTable table = parent.ExecReader(
                    @" SELECT TOP 1 id_phone_call  
                        FROM phone_call
                        WHERE id_operator = @id_operator
                         AND NOT (
                                ((date_start + date_interval) <= @date_start  AND (date_start + date_interval) < @date_finish)
                             OR (  @date_start < date_start AND @date_finish <= date_start  )
                                 )",
                    new SqlParameter[] { 
                        new SqlParameter("@id_operator", id_operator),
                        new SqlParameter("@date_start", callMessage.DateTimeStart()),
                        new SqlParameter("@date_finish", callMessage.DateTimeStart().Add(callMessage.DateTimeInterval()))});
                //if we found one row here, 
                //that means, we have duplicates or overlaying dates for the same operator

               return (0 == table.Rows.Count);

            }

            internal object getIdOperator()
            {
                return id_operator;
            }
        }

        public void HandleMessage(ICallMessage callMessage)
        {
            if (null == callMessage || !callMessage.isValid()) return;

            if ("".Equals(callMessage.Operator()) ||                      //   <---- these fields could be empty 
               DateTime.MinValue.Equals(callMessage.DateTimeStart()) ||   //   <---- because of incorrect mail template (regex),
               DateTime.MaxValue.Equals(callMessage.DateTimeInterval()) ||//   <---- they must be checked in the CallDataConsumer
               "".Equals(callMessage.Abonent()))                          //   <---- and mail should be saved in special table
            {
                String description = "Possibly the regex is incorrect. Check the MessageRegex element in the MessageStructure section of the config class. ";
                saveFailedMessage(callMessage, description);
                return;
            }

            OperatorTimeChecker ot;
            if (!operatorOperatorTime.TryGetValue(callMessage.Operator(), out ot))
            {
                ot = new OperatorTimeChecker(callMessage, this);
                operatorOperatorTime.Add(callMessage.Operator(), ot);
            }
            checkDateAndSave(callMessage, ot);
        }


        private Dictionary<string, OperatorTimeChecker> operatorOperatorTime = new Dictionary<string, OperatorTimeChecker>();

        private void checkDateAndSave(ICallMessage callMessage, OperatorTimeChecker ot)
        {
            if (ot.isValid(callMessage))
            {
                //save to database normally
                saveNormalMessage(callMessage, ot);
            }
            else
                saveFailedMessage(callMessage, "Invalid time intervals");
        }

        private void saveNormalMessage(ICallMessage callMessage, OperatorTimeChecker ot)
        {
            string fileName = saveFileGetNewName(callMessage);
            try
            {
                string query =
                   @"INSERT INTO phone_call( id_operator,  phone,  date_start,  date_interval,  sender_mail,  file_name)
                                    VALUES (@id_operator, @phone, @date_start, @date_interval, @sender_mail, @file_name)";
                ExecNonQuery(query, new SqlParameter[] { 
                    new SqlParameter("@id_operator", ot.getIdOperator()),
                    new SqlParameter("@phone", callMessage.Abonent()),
                    new SqlParameter("@date_start", callMessage.DateTimeStart()),
                    new SqlParameter("@date_interval", callMessage.DateTimeInterval()),
                    new SqlParameter("@sender_mail", callMessage.Sender()),
                    new SqlParameter("@file_name", fileName)
                });
            }
            catch(Exception e)
            {
                saveFailedMessage(callMessage, "Cannot save message to phone_call table: " + e.Message, fileName);
            }
        }

        private void saveFailedMessage(ICallMessage callMessage, string description)
        {
            string fileName = saveFileGetNewName(callMessage);
            saveFailedMessage(callMessage, description, fileName);
        }

        private void saveFailedMessage(ICallMessage callMessage, string description, string fileName)
        {
            string query =
                @" INSERT INTO failed_message( sender_mail,  file_name,  message_plain_text,  description)
                   VALUES                    (@sender_mail, @file_name, @message_plain_text, @description)";
            ExecNonQuery(query, new SqlParameter[] { 
                    new SqlParameter("@sender_mail", callMessage.Sender()),
                    new SqlParameter("@file_name", fileName),
                    new SqlParameter("@message_plain_text", callMessage.PlainText()),
                    new SqlParameter("@description", description)
                });

        }

        private string saveFileGetNewName(ICallMessage callMessage)
        {
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff_", CultureInfo.InvariantCulture) + Regex.Replace(callMessage.AudioFileName(), "[ \n\r\t]", "_");
            callMessage.SaveAudioFile(new FileInfo(directoryForAudioFiles + Path.DirectorySeparatorChar + newFileName));
            return newFileName;
        }

        private DataTable ExecReader(String queryString, SqlParameter[] parameters)
        {
            return Utils.ExecReaderSqlServer(queryString, parameters, connection);
        }

        private int ExecNonQuery(String query, SqlParameter[] parameters)
        {
            return Utils.ExecNonQuerySqlServer(query, parameters, connection);
        }


        private object ExecScalar(String query, SqlParameter[] parameters)
        {
            return Utils.ExecScalarSqlServer(query, parameters, connection);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (null != connection) connection.Dispose();
            }
            catch { }
        }

        #endregion

    }
}
