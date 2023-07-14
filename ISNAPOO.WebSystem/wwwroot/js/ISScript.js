
if (parseInt(localStorage.getItem("Accessibility_COLOR")) != 0) {
    var accessibility_Color = localStorage.getItem("Accessibility_COLOR");
    switch (parseInt(accessibility_Color)) {
        case 1: setContrast(true);break;
        case 2: setWhite(true);break;
        case 3: setGrayscale(true);break;
    }
}
if (localStorage.getItem("Accessibility_UNDERLINE_LINKS") === 'true') {
    var accessibility_Underline_links = localStorage.getItem("Accessibility_UNDERLINE_LINKS");
    underlineLinks(true);
}


isPreRendering = () => {
    return false;
};


function saveAsFile(filename, bytesBase64) {
    if (navigator.msSaveBlob) {
        //Download document in Edge browser
        var data = window.atob(bytesBase64);
        var bytes = new Uint8Array(data.length);
        for (var i = 0; i < data.length; i++) {
            bytes[i] = data.charCodeAt(i);
        }
        var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
        navigator.msSaveBlob(blob, filename);
    }
    else {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
    }
}

function Redirect(url) {
    location.href = url;
}

function generateUUID() { // Public Domain/MIT
    var key = localStorage.getItem("LOCAL_LOGIN_KEY");
    console.log(key);
    return key;
}

async function TEST() {
    var request = { selector: {}, showValidCerts: true }

    await postData('https://localhost:53952/getsigner', request)
        .then(data => {
            console.log(data.chain[0]); // JSON data parsed by `data.json()` call
            return data.chain[0];
        });
}

function chooseSignerCertificate() {
    var request = { selector: {}, showValidCerts: true };
    console.log(request);

    var payload = JSON.stringify(request);

    var url = "https://localhost:53952/getsigner";

    $.ajax(
        {
            url: url,
            type: "POST",
            crossDomain: true,
            data: payload,
            contentType: "application/json",
            dataType: "json"
        })
        .done(function (response) {
            console.log("done");
            console.log(response);
            if (response.status == "ok") {
                console.log(response.chain[0]);
            }
            else {
                console.log(response.reasonCode);
                console.log(response.reasonText);
            }
        })
        .fail(function (error) {
            console.log("failed");
            console.log(error);
        });
}
function chunk_split(body, chunklen, end) {
    chunklen = parseInt(chunklen, 10) || 76;
    end = end || '\r\n';
    if (chunklen < 1) {
        return false;
    }
    return body.match(new RegExp(".{0," + chunklen + "}", "g")).join(end);
}


function sign1(contentToBeSign)
{
    var os = 900;
     

    var msg = this.messages;
    var info = new Info(true, msg);
    var b = new BissSigning(info, true, false, 2.26);

    $('#cover').show();
    b.checkAndSign(contentToBeSign, function (res) {
        if (typeof (b.getCert()) != 'undefined') {
            $('#cert_data').val('-----BEGIN CERTIFICATE-----\r\n' + chunk_split(b.getCert()) + '-----END CERTIFICATE-----');
        }

        if (res.status == 40)
            $('#data_textarea').val($('#data_textarea').val() + "BISSSIGN=1");
        else {
            if (res.status == -10)
                $('#data_textarea').val($('#data_textarea').val() + "BISSSIGN=-3000");
            else if (res.status !== -5)
                $('#data_textarea').val($('#data_textarea').val() + "BISSSIGN=-3001");
        }
        if (b.version !== 'undefined')
            $('#data_textarea').val($('#data_textarea').val() + "\nBISSVERSION=" + b.version);

        if (res.status == -10) {
            $('#signing-wait').hide();
            $('#BISS-Loading').hide();
            $('#no-biss').show();
            $('#signing-close').show().removeAttr('disabled');
        } else if (res.status == -5) {
            $('#cover').hide();
            //document.forms[0].submit();
        } else {

            //For Linux, Mac and Other OS scip chains check
            if (os == 300 || os == 400 || os == 500) {
                $('#data_textarea').val($('#data_textarea').val() + "\nBISSCHAINS=1\n");
                $('#cover').hide();
                //document.forms[0].submit();
            } else {
                b.checkChains(function (result) {
                    if (result.status == "ok" && result.chainFound) {
                        $('#data_textarea').val($('#data_textarea').val() + "\nBISSCHAINS=1\n");
                    } else {
                        $('#data_textarea').val($('#data_textarea').val() + "\nBISSCHAINS=-3003\n");
                    }

                    $('#cover').hide();
                    //document.forms[0].submit();
                });
            }
        }

    }, '');

   
}
function sign(digestValue, signedDigestValue, signedDigestValueCert, showConfirmText) {
    var request = { selector: {}, showValidCerts: true };
    //console.log(request);

    var payload = JSON.stringify(request);

    var urlGetSigner = "https://localhost:53952/getsigner";

    var signerCertificate = "";

    var xhr = new XMLHttpRequest();
    xhr.open("POST", urlGetSigner, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send(payload);
    xhr.onload = function () {
        var data = JSON.parse(this.responseText);
        signerCertificate = data.chain[0];
        //console.log("Start sign");
        var jsonObject = {
            version: "3.10",
            signatureType: "signature",
            contentType: "digest",
            hashAlgorithm: "SHA256",
            contents: [digestValue],
            signedContents: [signedDigestValue],
            signedContentsCert: [signedDigestValueCert],
            signerCertificateB64: signerCertificate,
            confirmText: [showConfirmText]
        };
        //console.log(jsonObject);

        var urlSign = "https://localhost:53952/sign";
        var payloadSign = JSON.stringify(jsonObject);

        var xhrSign = new XMLHttpRequest();
        xhrSign.open("POST", urlSign, true);
        xhrSign.setRequestHeader('Content-Type', 'application/json');
        xhrSign.send(payloadSign);
        xhrSign.onload = function () {
            var data1 = JSON.parse(this.responseText);
            // console.log(data1);
            var ss = data1.signatures[0];
            //console.log(ss);

            DotNet.invokeMethodAsync('ISNAPOO.WebSystem', 'GetCertificatePublicDate', signerCertificate).then(data => {
                $("#divCertSubject").html(data.subject);
                $("#divSerialNumber").html(data.serialNumber);
                $("#divThumbprint").html(data.thumbprint);
                $("#divIssuer").html(data.issuer);
            });

            $('#btnCertificatePublicData').show();
        }
    }
}

// Example POST method implementation:
async function postData(url = '', data = {}) {
    // Default options are marked with *
    const response = await fetch(url, {
        method: 'POST', // *GET, POST, PUT, DELETE, etc.
        mode: 'cors', // no-cors, *cors, same-origin
        cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
        credentials: 'same-origin', // include, *same-origin, omit
        headers: {
            'Content-Type': 'application/json'
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: 'follow', // manual, *follow, error
        referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
}


function setGrayscale(set) {
   
    if (Boolean(set)) {
        document.body.style.filter = 'grayscale(100%)';
        setGrayscale = true;
        localStorage.setItem("Accessibility_COLOR", 3);
    } 
}
function setContrast(set) {
    if (Boolean(set)) {
        localStorage.setItem("Accessibility_COLOR", 1);
        document.body.style.filter = 'invert(100%)';
        const ele = document.querySelectorAll('.pcoded-navbar');
        ele.forEach(ele => {
            ele.setAttribute('navbar-theme', 'theme2');
        });
        const elements = document.querySelectorAll('*');
        elements.forEach(element => {
            element.style.setProperty('visibility', 'visible', 'important');
            element.style.setProperty('color', '#2b00ff', 'important');
            element.style.setProperty('background-color ', '#ffffff', 'important');
        });
    }
}

function setWhite(set) {
    
    if (Boolean(set)) {
       
        document.body.style.filter = 'invert(0%)';
        const ele = document.querySelectorAll('.pcoded-navbar');
        ele.forEach(ele => {
            ele.setAttribute('navbar-theme', 'theme2');
        });
        const elements = document.querySelectorAll('*');
        elements.forEach(element => {
            element.style.setProperty('visibility', 'visible', 'important');
            element.style.setProperty('color', '#2b00ff', 'important');
            element.style.setProperty('background-color ', '#ffffff', 'important');
        });
        localStorage.setItem("Accessibility_COLOR", 2);
    } 
}
function underlineLinks(set) {
    if (Boolean(set)) {
        setUnderlineLinks = true;
        const elements = document.querySelectorAll('*');
        elements.forEach(element => {
            element.style.setProperty('text-decoration', 'underline', 'important');
        });
        localStorage.setItem("Accessibility_UNDERLINE_LINKS", true);
    } 
}
function reset() {

    localStorage.setItem("Accessibility_FONT", 0);
    localStorage.setItem("Accessibility_COLOR", 0);
    localStorage.setItem("Accessibility_UNDERLINE_LINKS", false);
    
    reloadPage();


}

function changeStylesheetRule(stylesheet, selector, property, value) {

    selector = selector.toLowerCase();
    property = property.toLowerCase();
    value = value.toLowerCase();


    for (var i = 0; i < stylesheet.cssRules.length; i++) {
        var rule = stylesheet.cssRules[i];
        if (rule.selectorText === selector) {
            if (rule.style[property] === null || rule.style[property] === "") {
                stylesheet.insertRule(selector + " { font-size: 14px; !important;}", 0);
            }
            rule.style[property] = parseInt(rule.style[property], 10) + parseInt(value) + 'px';

            return;
        }
    }

    stylesheet.insertRule(selector + " { font-size: 14px !important; }", 0);
}

function getSetStyleRule(sheetName, selector, rule) {
    var stylesheet = document.querySelector('link[href*=' + sheetName + ']');

    if (stylesheet) {
        stylesheet = stylesheet.sheet;
        stylesheet.insertRule(selector + '{ ' + rule + '}', stylesheet.cssRules.length);
    }

    return stylesheet
}


function reloadPage() {
    location.reload();
}



var cont = document.getElementById("pcoded");

function changeSizeByBtn(size) {

    // Set value of the parameter as fontSize
    cont.style.fontSize = size;
}

// Get the root element
var r = document.querySelector(':root');
var localStorageFontMain = 0;

if (parseInt(localStorage.getItem("Accessibility_FONT")) != 0 && !isNaN(parseInt(localStorage.getItem("Accessibility_FONT")))) {
     
    localStorageFontMain = parseInt(localStorage.getItem("Accessibility_FONT"));
    var rs = getComputedStyle(r);
    var intFontMain = parseInt(rs.getPropertyValue('--rootFontSize'));
    var fontMain = '';

    fontMain = intFontMain + localStorageFontMain + 'px';

    r.style.setProperty('--rootFontSize', fontMain);
    
}

function IncreaseDecreaseFont(type) {

   
    // Set the value of variable --blue to another value (in this case "lightblue")
    var rs = getComputedStyle(r);
    var intFontMain = parseInt(rs.getPropertyValue('--rootFontSize'));
    var fontMain = '';

    if (type == "increase")
    {
        intFontMain = intFontMain + 1;
        fontMain = intFontMain + 'px';

        localStorageFontMain = localStorageFontMain + 1;
        

    }
    else {

        intFontMain = intFontMain - 1;
        fontMain = intFontMain + 'px';
        localStorageFontMain = localStorageFontMain - 1;
    }

    localStorage.setItem("Accessibility_FONT", localStorageFontMain);
   
    r.style.setProperty('--rootFontSize', fontMain);
 }
 
   

 





