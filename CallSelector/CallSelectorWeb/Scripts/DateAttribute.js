/// <reference path="../jquery-1.4.4-vsdoc.js" />
/// <reference path="../jquery.validate-vsdoc.js" />
/// <reference path="../jquery.validate.unobtrusive.js" />

jQuery.validator.addMethod("date", function (value, element, param) {
    if ($(element).attr("type") == "checkbox") {
        value = String($(element).attr("checked"));
        param = param.toLowerCase();
    }

    return (value == param);
});

jQuery.validator.unobtrusive.adapters.addSingleVal("equal", "valuetocompare");