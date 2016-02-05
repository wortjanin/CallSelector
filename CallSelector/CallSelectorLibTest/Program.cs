using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallSelectorLib;
using System.IO;
using log4net;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Threading;
using CallSelectorLib.Impl;

namespace CallSelectorLibTest
{
    public class Program
    {
        protected static readonly log4net.ILog log = LogManager.GetLogger(typeof(Program));
        private static string cfgSelector;
        static Program()
        {
            String cfgLog = "log4netCallSelector.xml";
            Char l = Path.DirectorySeparatorChar;
            String dir2up = ".." + l + ".." + l + "Config" + l;
            if (File.Exists(dir2up + cfgLog))
                log4net.Config.XmlConfigurator.Configure(new FileInfo(dir2up + cfgLog));
            cfgSelector = dir2up + "CallSelectorConfig.xml";

        }

        public static void Main(string[] args)
        {
            testSelector();

            //System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfff_", CultureInfo.InvariantCulture));
            //Thread.Sleep(100);
            //System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfff_", CultureInfo.InvariantCulture));
            //testSQL("mymy211@gmail.com");

            //System.Console.ReadLine();
            //System.Console.ReadLine();

            //DateTime lastNow = DateTime.Now;
            //DateTime lastLst = new DateTime(2009, 11, 12);

            //TimeSpan span = lastNow.Subtract(lastLst);

            //System.Console.WriteLine("Ticks: " + span.Ticks);
            //System.Console.WriteLine("Days: " + span.Days);
            //System.Console.WriteLine("Hours: " + span.Hours);
            //System.Console.WriteLine("TotalHours: " + span.TotalHours);

            //System.Console.Read();

        }


        private static void testSelector()
        {
            ISelectorConfig config = CallSelectorFactory.loadISelectorConfig(new FileInfo(cfgSelector));
            using(ISelector selector = CallSelectorFactory.createISelector(config))
                selector.Run();
        }

        private static void testSQL(string newMail)
        {
            System.Console.WriteLine("----------------------------------------------------------------");
            System.Console.WriteLine("");

            using (SqlConnection myConnection = new SqlConnection(
                           "user id=call_selector;" +
                           "password=call_selector_pass;server=WS_ITPARK\\SMSDELIVERY;" +
                           "Trusted_Connection=yes;" +
                           "database=db_call;" +
                           "connection timeout=30"))
            {
                myConnection.Open();
                //myConnection.FireInfoMessageEventOnUserErrors = true;
                System.Console.WriteLine("State: " + myConnection.State);


                System.Console.WriteLine("Rows affected: " + ExecNonQuery(
                    "INSERT INTO operator(mail) VALUES (@mail)",
                    new SqlParameter[] { new SqlParameter("@mail", newMail) },
                    myConnection));

                DataTable table = ExecReader(
                    "SELECT id_operator, mail FROM operator WHERE id_operator = @id_operator",
                    new SqlParameter[] { new SqlParameter("@id_operator", 99) },
                    myConnection);


                foreach (DataRow row in table.Rows)
                    System.Console.WriteLine(row["id_operator"] + "  " + row["mail"]);
                System.Console.Read();
                System.Console.Read();
            }
        }

        private static DataTable ExecReader(String queryString, SqlParameter[] parameters, SqlConnection myConnection)
        {
            DataTable table = new DataTable();
            List<string[]> list = new List<string[]>();

            using (SqlCommand myCommand = new SqlCommand(queryString, myConnection))
            {
                if (null != parameters)
                    myCommand.Parameters.AddRange(parameters);
                using (SqlDataReader myReader = myCommand.ExecuteReader())
                    table.Load(myReader);
            }
            return table;
        }

        private static int ExecNonQuery(String query, SqlParameter[] parameters, SqlConnection myConnection)
        {
            using (SqlCommand cmd = new SqlCommand(query, myConnection))
            {
                if (null != parameters)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
