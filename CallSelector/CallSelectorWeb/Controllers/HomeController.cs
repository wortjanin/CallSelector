using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Hosting;
using CallSelectorLib;
using System.Data.SqlClient;
using System.Data;
using System.Web.Routing;
using CallSelectorWeb.Models;
using System.Drawing;
using CallSelectorWeb.Common.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using CallSelectorWeb.Common;
using System.Security.Permissions;

namespace CallSelectorWeb.Controllers
{
    [Authorize]
    [HandleError]
    public class HomeController : Controller
    {

        #region PUBLIC

        
        public ActionResult Index()
        {
            var filter = new HomeIndexFilter();
            defaultFilter(filter);
            filter.HomeCallTable = new HomeCallTable
            {
                Table = CreateDataTable(filter),
                dicSenderConfig = iwebConfig.SenderConfigDictionary(),
                Operators = SelectOperators()
            };
            return View(filter);
        }

        [HttpPost]
        public ActionResult CallTable(HomeIndexFilter filter)
        {
            HomeCallTable table;
            if (false == ModelState.IsValid)
                table = new HomeCallTable
                {
                    Table = TopPhoneCalls(0),
                    dicSenderConfig = iwebConfig.SenderConfigDictionary(),
                    Operators = SelectOperators()
                };
            else
            {
                //defaultFilter(filter);
                table = ((null == filter.HomeCallTable) ? new HomeCallTable() : filter.HomeCallTable);
                table.Table = CreateDataTable(filter);
                table.dicSenderConfig = iwebConfig.SenderConfigDictionary();
                table.RequestForm = Request.Form.ToString(); // To make an appropriate Image
                table.Operators = SelectOperators();
            }
            return PartialView("CallTable", table);
        }

        //[HttpPost]
        public ActionResult Image(string count, int docWidth, HomeIndexFilter filter)
        {
            if (false == ModelState.IsValid || docWidth <= 20 * mrg) return File(ConstructImageEmpty(), "image/png");
            byte[] imageData = ConstructImageFromDbData(docWidth, filter);
            return File(imageData, "image/png");
        }

        public ActionResult GetFile(string fileName)
        {
            string pathFile = HostingEnvironment.MapPath(fileName);
            if (ModelState.IsValid && System.IO.File.Exists(pathFile))
            {
                return File(pathFile, "audio/mp3");
            }
            return null;
        }

        public ActionResult About()
        {
            return View();
        }

        #endregion PUBLIC

        private static readonly int width = 1000;
        private static readonly int height = 600;

        private static readonly IWebConfig iwebConfig;

        static HomeController()
        {
            string fileName = HostingEnvironment.MapPath("~/App_Data/CallSelectorConfigServer.xml");
            iwebConfig = CallSelectorFactory.loadIWebConfig(new FileInfo(fileName));
        }

        private static int topNumber = 150; // to shortent the time

        private DataTable TopPhoneCalls(int max)
        {
            using (SqlConnection connection = new SqlConnection(iwebConfig.DBConnectionString()))
            {
                connection.Open();
                int TopNumber = Math.Min(topNumber, Math.Abs(max));
                string query = @" 
                  SELECT TOP (@TopNumber)
                     O.mail AS operator_mail,
                     PC.phone,
                     PC.date_start,
                     PC.date_interval,
                     PC.sender_mail, 
                     PC.file_name
                    FROM phone_call PC
                    JOIN operator O ON(PC.id_operator = O.id_operator)
                    ORDER BY (PC.date_start) DESC";/* sender_mail is used to get the correct file directory, if there are different directories for different senders */
                DataTable table = Utils.ExecReaderSqlServer(
                    query,
                    new SqlParameter[] { new SqlParameter("@TopNumber", TopNumber) }, 
                    connection);
                return table;
            }
        }

        private DataTable SelectOperators()
        {
            using (SqlConnection connection = new SqlConnection(iwebConfig.DBConnectionString()))
            {
                connection.Open();
                const string query = "SELECT mail FROM operator";
                var table = Utils.ExecReaderSqlServer(
                    query,
                    null,
                    connection);
                return table;
            }
        }

        private bool DateIsValid(MyDateTime date){
            return date.IsValid && date.DateTime.HasValue;
        }

        private DataTable CreateDataTable(HomeIndexFilter filter)
        {
            if(filter.IsEmptyFilter) return TopPhoneCalls(50);
            using (SqlConnection connection = new SqlConnection(iwebConfig.DBConnectionString()))
            {
                bool isDate1 = DateIsValid(filter.DateMax);
                bool isDate2 = DateIsValid(filter.DateMin);
                DateTime min;
                DateTime max;
                string whereClause = " WHERE 1=1";
                List<SqlParameter> list = new List<SqlParameter>();
                if (isDate1 && isDate2)
                {
                    min = ((filter.DateMax.DateTime.Value > filter.DateMin.DateTime.Value) ? filter.DateMin.DateTime.Value : filter.DateMax.DateTime.Value);
                    max = ((filter.DateMax.DateTime.Value > filter.DateMin.DateTime.Value) ? filter.DateMax.DateTime.Value : filter.DateMin.DateTime.Value);
                    max = max.AddDays(1);
                    whereClause += " AND @date_start_min <= PC.date_start AND PC.date_start < @date_start_max";
                    list.Add(new SqlParameter("@date_start_min", min));
                    list.Add(new SqlParameter("@date_start_max", max));
                }
                else if (isDate1 || isDate2)
                {
                    min = ((isDate1) ? filter.DateMax.DateTime.Value : filter.DateMin.DateTime.Value);
                    whereClause += " AND @date_start <= PC.date_start AND PC.date_start < @date_start_next";
                    list.Add(new SqlParameter("@date_start", min));
                    list.Add(new SqlParameter("@date_start_next", min.AddDays(1)));
                }
                DateTime time1, time2;
                string timeFormat = "HH:mm:ss";
                if (Utils.getDateNoEx(filter.TimeMax, new string[] { timeFormat }, out time1))
                {
                    if (Utils.getDateNoEx(filter.TimeMin, new string[] { timeFormat }, out time2))
                    {
                        TimeSpan tsMin = (time1 > time2) ? time2.TimeOfDay : time1.TimeOfDay;
                        TimeSpan tsMax = (time1 > time2) ? time1.TimeOfDay : time2.TimeOfDay;
                        whereClause += " AND @time_min <= PC.date_interval AND PC.date_interval <= @time_max";
                        list.Add(new SqlParameter("@time_min", tsMin));
                        list.Add(new SqlParameter("@time_max", tsMax));
                    }
                    else
                    {
                        whereClause += " AND @time_min <= PC.date_interval AND PC.date_interval < @time_min_next";
                        list.Add(new SqlParameter("@time_min", time1.TimeOfDay));
                        list.Add(new SqlParameter("@time_min_next", time1.TimeOfDay.Add(new TimeSpan(TimeSpan.TicksPerHour))));
                    }

                }
                else if (Utils.getDateNoEx(filter.TimeMin, new string[] { timeFormat }, out time2))
                {
                    whereClause += " AND @time_min <= PC.date_interval AND PC.date_interval < @time_min_next";
                    list.Add(new SqlParameter("@time_min", time2.TimeOfDay));
                    list.Add(new SqlParameter("@time_min_next", time2.TimeOfDay.Add(new TimeSpan(TimeSpan.TicksPerHour))));
                }
                if (!String.IsNullOrEmpty(filter.OperatorMail))
                {
                    string equality = filter.OperatorMail.Contains("%") ? " LIKE " : " = ";
                    whereClause +=" AND O.mail " + equality +  " @operator_mail ";
                    list.Add(new SqlParameter("@operator_mail", filter.OperatorMail));
                }
                if (!String.IsNullOrEmpty(filter.PhoneNumber) && Regex.Match(filter.PhoneNumber, @"^\d{11,11}$").Success)
                {
                    string equality = filter.PhoneNumber.Contains("%") ? " LIKE " : " = ";
                    whereClause += " AND PC.phone " + equality + " @phone ";
                    list.Add(new SqlParameter("@phone", filter.PhoneNumber));
                }

                string query = @" 
                  SELECT TOP (@TopNumber)
                     O.mail AS operator_mail,
                     PC.phone,
                     PC.date_start,
                     PC.date_interval,
                     PC.sender_mail, 
                     PC.file_name
                    FROM phone_call PC
                    JOIN operator O ON(PC.id_operator = O.id_operator)
                    " + whereClause + @"
                    ORDER BY (PC.date_start) DESC";
                list.Add( new SqlParameter("@TopNumber", topNumber) );
                connection.Open();
                return Utils.ExecReaderSqlServer(query, list.ToArray(), connection);
            }
        }

        private void defaultFilter(HomeIndexFilter filter)
        {
            if (filter.IsEmptyFilter)
            {
                filter.DateMin.DateTime = DateTime.Now.Date;
            }
        }

        private byte[] ConstructImageEmpty()
        {
            using (Bitmap bm = new Bitmap(width, height))
            {
                using (System.Drawing.Graphics gr = Graphics.FromImage(bm))
                {
                    gr.FillRectangle(Brushes.White, 0, 0, bm.Width, bm.Height);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
            }
        }

        private int getX(DateTime date, DateTime minDate, DateTime maxDate, int width)
        {
            TimeSpan spanBig = maxDate.Subtract(minDate);
            TimeSpan span = date.Subtract(minDate);
            if (0 == spanBig.Ticks) return mrg;
            return Convert.ToInt32(((width - 2*mrg) * span.Ticks) / spanBig.Ticks) + mrg;
        }

        private int getY(int i, int maxCount, int height)
        {
            if (0 == maxCount) return mrg;
            return (((height - 2*mrg) * i) / maxCount) + mrg;//since Y points down in windows
        }

        int mrg = 8;
        private byte[] ConstructImageFromDbData(int docWidth, HomeIndexFilter filter)
        {//Caution, here we depend on the knowledge, that table is ordered (Max dates first)
            DataTable table = CreateDataTable(filter);
            int count = table.Rows.Count;
            if (count < 1) return ConstructImageEmpty();
            using (Bitmap bm = new Bitmap(docWidth, height))
            {
                //table.Columns["asda"].
                using (System.Drawing.Graphics gr = Graphics.FromImage(bm))
                {
                    gr.FillRectangle(Brushes.White, 0, 0, bm.Width, bm.Height);
                    gr.DrawLine(Pens.Black, new Point(0, height - mrg), new Point(docWidth, height - mrg));
                    gr.DrawLine(Pens.Black, new Point(mrg, 0), new Point(mrg, height));

                    //gr.DrawString(
                    //    " OperatorMail = " + filter.OperatorMail +
                    //    ", PhoneNumber = " + filter.PhoneNumber, new Font("Arial", 10F), new SolidBrush(Color.Black), new PointF(8, 24F));
                    //gr.DrawString(
                    //    ", DateMin = " + filter.DateMin.Value + 
                    //    ", DateMax = " + filter.DateMax.Value +
                    //    ", TimeMin = " + filter.TimeMin +
                    //    ", TimeMin = " + filter.TimeMax, new Font("Arial", 10F), new SolidBrush(Color.Black), new PointF(8, 48F));

                    DateTime minDate = table.Rows[count - 1].Field<DateTime>("date_start");
                    DateTime maxDate = table.Rows[0].Field<DateTime>("date_start");

                    for (int i = table.Rows.Count - 1; i >= 0; i--)
                    {//moving from min date to max
                        int y1 = getY(i, count, bm.Height); //Convert.ToInt32(bm.Height - (count * 1.5F * (i + 1)) - mrg);  //getY(i, count, bm.Height);
                        int x1 = getX(table.Rows[i].Field<DateTime>("date_start"), minDate, maxDate, bm.Width - 10 * mrg); //i * 50 + mrg;//getY(i, count, bm.Height);// getX(dr.Field<DateTime>("date_start"), minDate, maxDate, bm.Width);
                        int y2 = getY(i + 1, count, bm.Height); //Convert.ToInt32(bm.Height - (count * 1.5F * (i + 2)) - mrg); //getY(i + 1, count, bm.Height); //one call
                        int x2 = getX(table.Rows[i].Field<DateTime>("date_start").Add(table.Rows[i].Field<TimeSpan>("date_interval")), minDate, maxDate, bm.Width - 10 * mrg); //(i + 1) * 50 + mrg;//getY(i + 1, count, bm.Height);// getX(dr.Field<DateTime>("date_start").Add(dr.Field<TimeSpan>("date_interval")), minDate, maxDate, bm.Width);
                        //now we have a i-th rectangle, so let's drow it down
                        int dX = Math.Max(1, Math.Abs(x2 - x1));
                        gr.FillRectangle(Brushes.CornflowerBlue, x1, y1, Math.Max(1, Math.Abs(x2 - x1)), Math.Max(1, Math.Abs(y1 - y2)));
                        gr.DrawString("" + (table.Rows.Count - 1 - i + 1), new Font("Arial", 8F), Brushes.Black, (i <= table.Rows.Count - 10) ? (x1 - 20) : (x1 - 10), y1);//- ((i < (table.Rows.Count - 1) / 2) ? 15 : -5), y1);
                    }

                    bool printDate = !minDate.Date.Equals(maxDate.Date);
                    int counts = 10;
                    for (int i = 0; i < counts; i++)
                    {
                        DateTime date = minDate.Add(new TimeSpan(maxDate.Subtract(minDate).Ticks * i / counts));
                        int x1 = mrg + i * ((bm.Width - 10 * mrg) / counts);
                        gr.DrawLine(Pens.Black, x1, bm.Height - 5 * mrg, x1, bm.Height);
                        if (printDate) gr.DrawString(date.ToString(Const.DateFormat.Value), new Font("Arial", 8F), Brushes.Black, x1 + mrg, bm.Height - 5 * mrg);
                        gr.DrawString(date.ToString("HH:mm"), new Font("Arial", 8F), Brushes.Black, x1 + mrg, bm.Height - 7 * mrg);
                    }


                    using (MemoryStream stream = new MemoryStream())
                    {
                        bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        return stream.ToArray();
                    }
                }
            }
        }

    }  
}
