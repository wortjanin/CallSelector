//Sys.Mvc.FormContext.OnSuccessEnableClientValidation = function (ajaxContext) { }

Sys.Mvc.FormContext.OnSuccessEnableClientValidation = function (ajaxContext) {
    //Getting the update target container
    var updateTarget = document.getElementById(ajaxContext.$4.id);
    //Getting all script elements in it (script elements injected with innerHtml are not executed)
    var mvcClientValidationMetadataOldScripts = updateTarget.getElementsByTagName('script');
    var mvcClientValidationMetadataNewScripts = [];
    //For every script element
    while (mvcClientValidationMetadataOldScripts.length > 0) {
        //Create a new one
        var mvcClientValidationMetadataNewScript = document.createElement('script');
        mvcClientValidationMetadataNewScript.type = 'text/javascript';
        mvcClientValidationMetadataNewScript.text = mvcClientValidationMetadataOldScripts[0].text;
        //Add it to collection
        mvcClientValidationMetadataNewScripts.push(mvcClientValidationMetadataNewScript);
        //And remove old one
        updateTarget.removeChild(mvcClientValidationMetadataOldScripts[0]);
    }
    //For every new script element
    while (mvcClientValidationMetadataNewScripts.length > 0) {
        //Append it to update target container, this way they will be executed and generate needed metadata
        updateTarget.appendChild(mvcClientValidationMetadataNewScripts.pop());
    }
    //Calling Microsoft validation initialization for new metadata
    Sys.Mvc.FormContext._Application_Load();
}

