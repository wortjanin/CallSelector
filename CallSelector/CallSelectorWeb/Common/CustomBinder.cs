using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CallSelectorWeb.Common.Models;

namespace CallSelectorWeb.Common
{
    public class CustomBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (typeof(MyDateTime) == propertyDescriptor.PropertyType)
                return new MyDateTime(bindingContext.ValueProvider.GetValue(bindingContext.ModelName));
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}
