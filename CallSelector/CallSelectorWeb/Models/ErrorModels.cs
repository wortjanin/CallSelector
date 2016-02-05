using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CallSelectorWeb.Models
{
    public class GeneralErrorModel
    {
        [DataType(DataType.Text)]
        [DisplayName("Ошибка")]
        public string Message { get; set; }

        [DataType(DataType.Text)]
        [DisplayName("Описание")]
        public string Description { get; set; }
    }
}
