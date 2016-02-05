using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

namespace CallSelectorLib
{
    public class Utils
    {
        public static string formatText(string text)
        {
            if (null == text) return "";
            string[] strings = text.Split(new char[] { '\n', });
            text = "";
            for (int i = 0; i < strings.Length; i++)
            {
                String s = strings[i].Trim();
                if ("".Equals(s)) continue;
                text += s + " ";
            }
            return text.Trim();
        }

        public static string formatDirectoryName(string directoryForAudioFiles)
        {
            string dicName = String.IsNullOrEmpty(directoryForAudioFiles) ? "." : directoryForAudioFiles;
            dicName = dicName.Replace("/", "" + Path.DirectorySeparatorChar);
            while (dicName.EndsWith("" + Path.DirectorySeparatorChar) && dicName.Length > 1)
                dicName = dicName.Substring(0, dicName.Length - 1);
            return dicName;
        }

        public static string formatFileExtension(string audioFileExtension)
        {
            string res = audioFileExtension.Replace(".", "");
            if (String.IsNullOrEmpty(res))
                throw new InvalidDataException("AudioFileExtension is null or empty");
            return res;
        }

        public static DateTime getDate(string stringDate, string[] variants)
        {
            DateTime temp = DateTime.MinValue;
            if (getDateNoEx(stringDate, variants, out temp))
                return temp;
            throw new InvalidDataException("Incorrect input data format");
        }

        public static bool getDateNoEx(string stringDate, string[] variants, out DateTime dt)
        {
            return (DateTime.TryParseExact(
               stringDate, variants, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt));
       }


        public static DataTable ExecReaderSqlServer(String queryString, SqlParameter[] parameters, SqlConnection connection)
        {
            DataTable table = new DataTable();
            using (SqlCommand myCommand = new SqlCommand(queryString, connection))
            {
                if (null != parameters)
                    myCommand.Parameters.AddRange(parameters);
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                    table.Load(myReader);
            }
            return table;
        }

        public static int ExecNonQuerySqlServer(String query, SqlParameter[] parameters, SqlConnection connection)
        {
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                if (null != parameters)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }


        public static object ExecScalarSqlServer(String query, SqlParameter[] parameters, SqlConnection connection)
        {
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                if (null != parameters)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

    }
}
