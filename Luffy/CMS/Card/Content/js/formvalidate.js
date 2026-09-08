function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(emailAddress);
}
function CheckAlpha(obj) {

    if (obj.match(/^[a-zA-Z0-9_]+$/)) {
        return true;
    }
    else {
        return false;
    }
}
function isValidDate(dateStr) {
    // Date validation function courtesty of 
    // Sandeep V. Tamhankar (stamhankar@hotmail.com) -->

    // Checks for the following valid date formats:
    // MM/DD/YY   MM/DD/YYYY   MM-DD-YY   MM-DD-YYYY

    var datePat = /^(\d{1,2})(\/|-)(\d{1,2})\2(\d{4})$/; // requires 4 digit year

    var matchArray = dateStr.match(datePat); // is the format ok?
    if (matchArray == null) {
       
        return false;
    }
    month = matchArray[1]; // parse date into variables
    day = matchArray[3];
    year = matchArray[4];
    if (month < 1 || month > 12) { // check month range

        return false;
    }
    if (day < 1 || day > 31) {

        return false;
    }
    if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31) {

        return false;
    }
    if (month == 2) { // check for february 29th
        var isleap = (year % 4 == 0 && (year % 100 != 0 || year % 400 == 0));
        if (day > 29 || (day == 29 && !isleap)) {

            return false;
        }
    }
    return true;
}
function isInteger(s) {

    var i;

    if (isEmpty(s))

        if (isInteger.arguments.length == 1)

            return 0;

        else

            return (isInteger.arguments[1] == true);

    for (i = 0; i < s.length; i++) {

        var c = s.charAt(i);

        if (!isDigit(c))

            return false;

    }

    return true;

}
function isEmpty(s) {

    return ((s == null) || (s.length == 0))

}

function isDigit(c) {

    return ((c >= 0) && (c <= 9))

}
function format_title(title) {
    title = title.toLowerCase();
    title = title.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    title = title.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    title = title.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    title = title.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    title = title.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    title = title.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    title = title.replace(/đ/g, "d");
    title = title.replace(/!|@@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
    title = title.replace(/-+-/g, "-"); //thay thế 2- thành 1-
    title = title.replace(/^\-+|\-+$/g, ""); //cắt bỏ ký tự - ở đầu và cuối chuỗi

    return title;
}

//lay query
function querySt(ji) {
    hu = window.location.search.substring(1);
    gy = hu.split("&");
    for (i = 0; i < gy.length; i++) {
        ft = gy[i].split("=");
        if (ft[0] == ji) {
            return ft[1];
        }
    }
    return '';
}



// Remove HTML
function removeAllHtmlTags(text) {
    var strInputCode = text;
    /*
    This line is optional, it replaces escaped brackets with real ones,
    i.e. < is replaced with < and > is replaced with >
    */
    strInputCode = strInputCode.replace(/&(lt|gt);/g, function (strMatch, p1) {
        return (p1 == "lt") ? "<" : ">";
    });

    var strTagStrippedText = strInputCode.replace(/<\/?[^>]+(>|$)/g, "");

    return strTagStrippedText.replace("&nbsp;", "").replace("&amp;nbsp;", "");
}

function ValidateControls(valid_message, valid_id, msgBox) {
    if (msgBox == undefined) {
        msgBox = '#msgBox';
    }
    $(msgBox).hide();
    var validText = $(valid_id).val();
   
    if (validText == null || typeof validText == 'undefined') {

        $(msgBox).html(valid_message).show();

        $(valid_id).focus();
        return false;
    }
    if (validText == "-1") {

        $(msgBox).html(valid_message).show();

        $(valid_id).focus();
        return false;
    }
    validText = $.trim(validText);

    // valid_text = valid_text.toUpperCase();
    if (validText == '') {

        $(msgBox).html(valid_message).show(); //.attr("class", "c2_box_ct");

        $(valid_id).focus();
        return false;
    }
    if (validText.length > 1) {
       

        var datePat = /^(?:(?!<[^>]*>).)*$/; // requires 4 digit year

        var matchArray = validText.match(datePat); // is the format ok?
        if (matchArray == null) {
            $(msgBox).html("Thông tin khai báo bao gồm từ khóa không hợp lệ").show();

            $(valid_id).focus();
            return false;
        }
    }
    return true;
}

function isFloat(n) {
    return n === +n && n !== (n | 0);
}
// Chặn tấn công CSRF (XSRF)
function addAntiForgeryToken(data) {
    if (!data) {
        data = {};
    }
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

function model_alert(message) {
    $('#modal-message').html(message);

    $('#modal-alert').modal('show');
}
function ShowProgress() {
    setTimeout(function () {

       
        $(".fullscreenFF").css({ height: $(document).height() });
        $(".TB_overlayBG").show();
        $('.container').css({ opacity: 0.4 });
    }, 100);
}
function HideProgress() {
    setTimeout(function () {
        $(".fullscreenFF").css({ height: 0 });
        $(".TB_overlayBG").hide();
      
        $('.container').css({ opacity: 1 });
    }, 100)

}