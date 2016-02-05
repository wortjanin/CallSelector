using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using CallSelectorWeb.Common;
using System.Text.RegularExpressions;

namespace CallSelectorWeb.Common.Models
{

    public class MyDateTime
    {
        private bool isValid = true;
        public MyDateTime() { }
        public MyDateTime(DateTime? dt) { DateTime = dt; }
        public MyDateTime(ValueProviderResult valueProviderResult)
        {
            Value = valueProviderResult.AttemptedValue;
        }
        private DateTime? _value = null;

        public string Value
        {
            get { return _value.HasValue ? _value.Value.ToString(Const.DateFormat.Value) : null; }
            set
            {
                DateTime dt;
                if (String.IsNullOrEmpty(value))
                {
                    isValid = true;
                    _value = null;
                }
                else if (System.DateTime.TryParseExact(
                    value, 
                    Const.DateFormat.Value, 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, 
                    out dt))
                {
                    isValid = true;
                    _value = dt;
                }
                else
                {
                    isValid = false;
                    _value = null;
                }
            }
        }
        public bool IsValid { get { return isValid; } }

        public DateTime? DateTime
        {
            get { return _value; }
            set { _value = value; }
        }

        public string jDateFormat { get { return Const.DateFormat.jFormat; } }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class DateFormatAttribute : ValidationAttribute
    {
        public DateFormatAttribute() { }

        public override bool IsValid(object value)
        {
            return null == value || ((MyDateTime)value).IsValid;
        }
    }


    public class DateFormatAttributeAdapter : DataAnnotationsModelValidator<DateFormatAttribute>
    {
        public DateFormatAttributeAdapter(
            ModelMetadata metadata, ControllerContext context, DateFormatAttribute attribute) :
            base(metadata, context, attribute)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            ModelClientValidationRule rule = new ModelClientValidationRegexRule(
                String.Format(Const.DateFormat.Error, Metadata.DisplayName),
                Const.DateFormat.Regex
            );
            return new ModelClientValidationRule[] { rule };
        }
    }


}
