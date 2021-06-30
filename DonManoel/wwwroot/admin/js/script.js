$(document).ready(function () {

    $('.numeric').keyup(function () {
        $(this).val(this.value.replace(/\D/g, ''));
    });

    if ($('[data-toggle="tooltip"]').length > 0) {
        $('[data-toggle="tooltip"]').tooltip()
    }


    $(document).on('keyup', function (e) {
        //console.log("Tecla pressionada %s (%s)", e.key, getCharCode(e));
        if (getCharCode(e) === 27) {
            $(".close").click();
        }

    });
});


function hostSite() {
    var Path = location.host;
    var VirtualDirectory;
    if (Path.indexOf("localhost") >= 0 && Path.indexOf(":") >= 0) {
        VirtualDirectory = "";
    }
    else {
        var pathname = window.location.pathname;
        var VirtualDir = pathname.split('/');
        VirtualDirectory = VirtualDir[1];
        VirtualDirectory = '/' + VirtualDirectory;
        return location.protocol + "//" + location.host + "/";
    }

    return location.protocol + "//" + location.host + VirtualDirectory + "/";
}

function isNullOrEmpty(s) {
    return (s === undefined || s === null || s === "");
}

function getURLParameters(paramName) {
    var sURL = window.document.URL.toString();
    if (sURL.indexOf("?") > 0) {
        var arrParams = sURL.split("?");
        var arrURLParams = arrParams[1].split("&");
        var arrParamNames = new Array(arrURLParams.length);
        var arrParamValues = new Array(arrURLParams.length);

        var i = 0;
        for (i = 0; i < arrURLParams.length; i++) {
            var sParam = arrURLParams[i].split("=")[0];
            arrParamNames[i] = sParam;
            if (sParam[1] !== "")
                arrParamValues[i] = unescape(arrURLParams[i].substring(sParam.length + 1));
            else
                arrParamValues[i] = "";
        }

        for (i = 0; i < arrURLParams.length; i++) {
            if (arrParamNames[i] === paramName) {
                return arrParamValues[i];
            }
        }
        return "";
    }
    else {
        return "";
    }
}

//onKeyPress="return sCampoNumber(event);"
function sCampoNumber(e) {
    var key = window.event ? e.keyCode : e.which;
    var keychar = String.fromCharCode(key);
    var refStr = "0123456789";
    var vRet = true;
    if ((key !== 13) && (key !== 8)) { if (refStr.indexOf(keychar) === -1) { vRet = false; } else { vRet = true; } }
    else { if (key === 8) { vRet = true; } };
    return vRet;
};

//onKeyPress="return sAlphaNumeric(event);"
function sAlphaNumeric(e) {
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode === 8) return true;

    var keynum;
    var keychar;
    var charcheck = /[a-zA-Z0-9]/;
    if (window.event) // IE
    {
        keynum = e.keyCode;
    }
    else {
        if (e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which;
        }
        else return true;
    }

    keychar = String.fromCharCode(keynum);
    return charcheck.test(keychar);
}

function get_browser() {
    var ua = navigator.userAgent, tem, M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return { name: 'IE', version: (tem[1] || '') };
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\bOPR\/(\d+)/)
        if (tem !== null) { return { name: 'Opera', version: tem[1] }; }
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) !== null) { M.splice(1, 1, tem[1]); }
    return {
        name: M[0],
        version: M[1]
    };
}

function setValueAsDate(inputId, data) {
    var browser = get_browser();
    if (browser.name === "Chrome") {
        var ano = "";
        var mes = "";
        var dia = "";
        if (data !== null && data !== "") {
            ano = data.substring(6);
            mes = data.substring(3, 5);
            dia = data.substring(0, 2);
        }
        document.getElementById(inputId).valueAsDate = new Date(ano, parseInt(mes) - 1, dia);
    }
    else {
        document.getElementById(inputId).value = data;
    }
}

function formatMoney(amount, withSymble=true, decimalCount = 2, decimal = ",", thousands = ".") {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;
        if (withSymble)
            return "R$ " + negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
        else 
            return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
        console.log(e)
    }
};

function getCharCode(e) {
    e = (e) ? e : window.event, charCode = null;

    try {
        charCode = (e.which) ? e.which : e.keyCode;
        return charCode;
    } catch (err) {
        return charCode;
    }
}

