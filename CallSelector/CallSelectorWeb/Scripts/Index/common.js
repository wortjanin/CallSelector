
function loadImage(count) {
    var docWidth = Math.floor( $(document).width() * 0.85);
    var arr = $('tr#FilterInput input[type=text]');
    var str = "OperatorMail=" + $('#OperatorMail').val() + "&";
    for(var i = 0; i < arr.length; i++) {
        var elem = $($('tr#FilterInput input[type=text]')[i]);
        str += elem.attr('id') + "=" + elem.val() + "&";
    }
   
    var showImg = $('#HomeCallTable_ShowImage');
    str += showImg.attr('name') + "=" + showImg.val() + "&X-Requested-With=XMLHttpRequest";
    var img = $("<img />").attr('src', 'Home/Image?count=' + count + '&docWidth=' + docWidth + '&' + str)
     .load(function() {
        if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
          alert('broken image!');
        } else {
            $("#home_controller_image").append(img);
        }
   });
    // $('input:hidden[name=RequestForm]').val())
}

var var_g_Home_Image_count = 1;
function setClicks() {

    setClick($('#home_controller_toggle'), function() {
        if ($("#home_controller_toggle").hasClass('toggled')) {

            $("#home_controller_image").empty();
            $("#home_controller_toggle").removeClass('toggled');
            $("#home_controller_toggle").addClass('untoggled');
            $('input:hidden[name=HomeCallTable.ShowImage]').val("0");
            $("#home_controller_toggle").innerHTML = "+";

        } else {
            var_g_Home_Image_count++;
            loadImage(var_g_Home_Image_count);
            $("#home_controller_toggle").removeClass('untoggled');
            $("#home_controller_toggle").addClass('toggled');
            $('input:hidden[name=HomeCallTable.ShowImage]').val("1");
            $("#home_controller_toggle").innerHTML = "-";
        }
    });

    if ("1" == $('input:hidden[name=HomeCallTable.ShowImage]').val()) {
        var_g_Home_Image_count++;
        loadImage(var_g_Home_Image_count);
    }

    var hook;
    setClick($('.player_link'), hook = function(obj) {
        var repl = null;
        $('div')
           .filter(function() {
               var match = this.id.match( /objx\d+/ );
               if(match)
                   repl = $(this).parent().parent().find("input[type='hidden'][name='div_player_link']").val();
               return match;
           }).parent().html(repl);
        setClick($('.player_link'), hook);
        $(this).parent().html($(this).parent().parent().find("input[type='hidden'][name='div_player']").val());
    });

    setClick('#ButtonClear', function() {
        var arr = $('tr#FilterInput input[type=text]');
		for(var i = 0; i < arr.length; i++){
		    $(arr[i]).val('');
        }
        $('#OperatorMail').val('');
    });

    setClick('.filterInline', function (obj) {
        var parent = $(this).parent();
        if($(parent).hasClass('home_operator_mail')) {
            $('#OperatorMail').val($(this).text().trim());
            $('#ButtonFilter').click();
        }
        else if($(parent).hasClass('home_phone_number')) {
            $('#ButtonClear').click();
            $('input#PhoneNumber').val($(this).text().trim());
            $('#ButtonFilter').click();
        }
    });
}

function OnComplete(ajaxContext) {
    var url = ajaxContext._request._body;
    var image_url = "Home/Image/" + url;
}

function success(cntx) {
    setClicks();
    Sys.Mvc.FormContext.OnSuccessEnableClientValidation(cntx);
}

function success_image(cntx) {

}


function setupDatepicker(fmt) {
    if (undefined == fmt) iniDatePicker();
    $(".datePicker").attachDatepicker({
        showOn: "button",
        buttonImage: "/Content/ThirdParty/images/calendar_icon.jpg",
        buttonText: "",
        buttonImageOnly: true,
        dateFormat: $("#MyDateTime_ascx_DateTimeFormat").val()//"dd.mm.yy"
    });
}


function failure() { alert("Server error"); }