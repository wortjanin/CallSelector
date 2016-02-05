using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CallSelectorWeb.Common.Models;
using System.ComponentModel;
using System.Data;
using CallSelectorLib;
using System.ComponentModel.DataAnnotations;
using CallSelectorWeb.Common;

namespace CallSelectorWeb.Models
{
    public class HomeIndexFilter
    {
        public bool IsEmptyFilter
        {
            get
            {

                return (
                      
                    ( !DateMin.DateTime.HasValue ) &&
                    ( !DateMax.DateTime.HasValue ) &&
                      String.IsNullOrEmpty(OperatorMail) &&
                      String.IsNullOrEmpty(TimeMin) &&
                      String.IsNullOrEmpty(TimeMax) &&
                      String.IsNullOrEmpty(PhoneNumber)
                    );
            }
        }
        [DataType(DataType.Text)]
        [DisplayName("Оператор")]
        [StringLength(512, ErrorMessage=">512 символов")]
        public string OperatorMail { get; set; }


        [DateFormat]
        [DisplayName("Дата мин.")]
        public MyDateTime DateMin{get; set;}

        [DateFormat]
        [DisplayName("Дата макс.")]
        public MyDateTime DateMax { get; set; }

        [RegularExpression(@"^[012][0123]:[0-5]\d:[0-5]\d$", ErrorMessage = "23:59:59")]
        [DataType(DataType.Text)]
        [DisplayName("Время мин.")]
        public string TimeMin { get; set; }

        [RegularExpression(@"^[012][0123]:[0-5]\d:[0-5]\d$", ErrorMessage = "23:59:59")]
        [DataType(DataType.Text)]
        [DisplayName("Время макс.")]
        public string TimeMax { get; set; }


        [RegularExpression(@"^\d{11,11}$", ErrorMessage = "12345678901")]
        [DataType(DataType.Text)]
        [DisplayName("Телефонный номер")]
        public string PhoneNumber { get; set; }

        public HomeCallTable HomeCallTable { get; set; }

        public HomeIndexFilter()
        {
            DateMin = new MyDateTime();
            DateMax = new MyDateTime();
        }
    }

    public class HomeCallTable
    {
        public HomeCallTable()
        {
            ShowImage = "0";
        }
        public DataTable Table { get; set; }
        public Dictionary<string, IWebFileConfig> dicSenderConfig { get; set; }
        // !"0".equals => show
        public string ShowImage { get; set; }
        public string RequestForm { get; set; }
        public DataTable Operators { get; set; }

    }
}
