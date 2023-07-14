;
;
;
;
;
;
;
;
;
var BissSigning = /** @class */ (function () {
    function BissSigning(info, debug, test, version) {
        if (debug === void 0) { debug = false; }
        if (test === void 0) { test = false; }
        this.contentType = 'digest';
        this.ports = [53952, 53953, 53954];
        this.signerMethod = 'BISS';
        this.BISSRetryCnt = 2;
        this.lastVersion = version;
        /**
         * съдържа алгоритъма, който трябва да бъде използван за подписване. Подържаните стойности са “SHA1”, “SHA256” и “SHA512”.
         * Да се обърне внимание, че ако стойността на параметъра “contentType” е “digest” то параметъра трябва да съдържа хеш, който е направен с хеш алгоритъма посочен в полето “hashAlgorithm”.
         */
        this.hashAlgorithm = 'SHA256';
        this.checkStatusTimeout = 1000; //Времето в милисекунди за заявка за статус на подписването
        this.scheme = 'https';
        this.timeout = 1000000; // Време за избор на сертификат (в милисекунди)
        this.hashAlgorithms = ['SHA1', 'SHA256', 'SHA512'];
        this.info = info;
        this.info.signing = this;
        this.debug = debug;
        this.test = test;
        this.stop = false;
        if (test) {
            this.scheme = 'http';
            this.ports = [53951];
        }
    }



    /**
     * Прави проверка за достъпност на BISS. Извиква callback на който подава резултата в обект от тип responseType
     * responseType.status: 10 успех || -10 неуспех
     *  @param (out) callback(responseType)
     */
    BissSigning.prototype.check = function (callback) {
        var _this = this;
        try {
            var checkBISS = function (portIndex) {
                _this.BISSPort = _this.ports[portIndex];
                _this.info.log('Checking port: ' + _this.BISSPort);
                jQuery.ajax({
                    type: "GET",
                    url: _this.scheme + '://localhost:' + _this.BISSPort + '/version',
                    crossDomain: true,
                    dataType: "json",
                    success: function (result) {
                        _this.info.log('Service available at port: ' + _this.BISSPort);
                        _this.info.log(result);
                        if (Number(result.version) < Number(_this.lastVersion)) {
                            _this.version = result.version;
                            _this.response(callback, -5, 'OLD_BISS_VERSION', result.version);
                        }
                        else if (Number(result.version) >= 1) {
                            _this.version = result.version;
                            _this.response(callback, 10, 'BISS_VERSION', result.version);
                        }
                        else {
                            _this.response(callback, -10, 'NO_CONNECTION');
                        }
                    },
                    error: function (response) {
                        portIndex++;
                        _this.info.log('Service not available at port: ' + _this.BISSPort + response);
                        if (typeof _this.ports[portIndex] !== 'undefined') {
                            checkBISS(portIndex);
                        }
                        else {
                            _this.response(callback, -10, 'NO_CONNECTION');
                        }
                    }
                });
            };
            checkBISS(0);

        }
        catch (error) {
            this.info.log('check error');
            this.info.log(error);
            this.response(callback, -10, 'NO_CONNECTION');
        }
    };

    BissSigning.prototype.checkChains = function (callback) {
        var _this = this;
        try {
            var Chains = function (keyId, store, cb) {
                var chainsFound = false;
                var xhr = jQuery.ajax({
                    type: "POST",
                    method: "POST",
                    url: _this.scheme + "://localhost:" + _this.BISSPort + "/chainFound",
                    crossDomain: true,
                    cache: false,
                    data: JSON.stringify({
                        keyIdentifier: keyId,
                        store: store
                    }),
                    dataType: "json",
                    contentType: "application/json",
                    success: function (result) {
                        if (result.chainFound == false) {
                            callback(result);
                        } else {
                            cb(result);
                        }
                    },
                    error: function (res) {
                        callback(res);
                    }

                });
            };

            Chains("27cf084304f0c583376781174dfc05e6db658bb0", "CA", function (res) {// B-Trust Operational Qualified CA
                Chains("07dcaa307698b7854b6d0318c8e3cda77b3682ef", "CA", function (res1) {// B-Trust Operational Advanced CA
                    Chains("88db42ed8905320c72270c461be1c6095eecc921", "root", function (res2) { // B-Trust Root Advanced CA
                        Chains("f284ee2e35fef0fad85050b09c4889ea5a2fd9ab", "root", function (res3) {// B-Trust Root Qualified CA
                            callback(res3);
                        });
                    });
                });
            });

        }
        catch (error) {
            callback({ status: "error", chainFound: "false" });
        }
    };

    /**
     * Извършва подписване на подаденото съдържание (content).
     *  1 Ако е подадено getHashUrl се обръща към услугата за генериране на хеш, ако не е, очаква подаденото съдържание да е от тип signedHashes
     *  2 Избира сертификат (ако все още не е избран)
     *  3 Подписва
     *  4 Извиква callback на който подава резултата в обект от тип responseType
     *    responseType.status: 40 успех || -40;-41;-42;-43 неуспех (възможно е да върне и статуси от проверка за достъпност, генериране на хеш или избор на сертификат сертификат)
     *  @param contentToSign - съдържанието, което да се подпише
     *  @param (out) callback(responseType)
     *  @param signImediately (опционален) - ако е подаден, подписва директно подаденото съдържание без каквато и да е обработка
     */
    BissSigning.prototype.sign = function (contentToSign, callback, signImediately) {
        var _this = this;
        this.info.log('Check if cert available (sign): ' + contentToSign);
        var prepareAndSign = function (signedHashes) {
            _this.info.log('signedHashes: ');
            _this.info.log(signedHashes);
            var mapping = [], signedContents = [], contents = [], signedContentsCert = [], cryptoData = {
                signedContents: signedContents,
                contents: contents,
                signedContentsCert: signedContentsCert,
                version: _this.version,
                contentType: _this.contentType,
                hashAlgorithm: _this.hashAlgorithm,
                signatureType: "signature",
                signerCertificateB64: _this.signerCertificate,
                confirmText: (typeof _this.confirmText == 'string' && _this.confirmText.length > 0) ? [_this.confirmText] : null
            };
            for (var key in signedHashes.toSignArr) {
                if (typeof signedHashes.toSignArr[key].signedContents == 'string') {
                    cryptoData.signedContents.push(signedHashes.toSignArr[key].signedContents);
                    cryptoData.contents.push(signedHashes.toSignArr[key].contents);
                    mapping.push(key);
                }
            }
            cryptoData.signedContentsCert.push(signedHashes.signedContentsCert);
            _this.info.log('Final signing object for BISS: ');
            _this.info.log(cryptoData);
            cryptoData.contents = ["MYIBXTAYBgkqhkiG9w0BCQMxCwYJKoZIhvcNAQcBMBwGCSqGSIb3DQEJBTEPFw0yMjA3MjAwNzE5NDRaMC0GCSqGSIb3DQEJNDEgMB4wDQYJYIZIAWUDBAIBBQChDQYJKoZIhvcNAQELBQAwLwYJKoZIhvcNAQkEMSIEIFopttmygR4kDnNWVq5VI+FKHbm8rhsSvSyEraXoWu+CMIHCBgsqhkiG9w0BCRACLzGBsjCBrzCBrDCBqQQgR0Zm1NAtgCFnZyK9P2jf3D/2ppjsYfrUIJWOmzmkFogwgYQwfKR6MHgxCzAJBgNVBAYTAkJHMRgwFgYDVQRhEw9OVFJCRy0yMDEyMzA0MjYxEjAQBgNVBAoTCUJPUklDQSBBRDEQMA4GA1UECxMHQi1UcnVzdDEpMCcGA1UEAxMgQi1UcnVzdCBPcGVyYXRpb25hbCBRdWFsaWZpZWQgQ0ECBCPE9OY="];
            cryptoData.signedContents = ["ZSPUSgKQvETf4jDPwmh8ZIwDjezAajEXRXZ8y8XoF2oTepce08fTbOIbfT/TqdKAJO2bWlv3JT01mGRzslFJnZ1/ejUrJEeZWRIcRDKXlItDW2/8qvS8dUZp8qB1bJwournZoMme9eMbQ7HWMXpLmXH/8Xo3Df6epOtTbjHpZEiOjhRDgMD6v+0emWnWRsN8E/AM9eYUNoZd1DHshLaMhgw9CcVqhP4lszFbOmnnDUQP9SRv/ssCs5aDc/96fnDqs4vVuZoxvudddLqYEYmqOPEIgLLnmprabl2ViGlUIGcz5On3hQyRk5yfwebvgjJWMJpp0tAR+Uwf8F5UY8olQA=="];
            cryptoData.signedContentsCert = ["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"];
            if (typeof cryptoData.contents == 'undefined' || cryptoData.contents.length < 1) {
                _this.response(callback, -40, 'NO_SIGNING_CONTENT_ERR');
                return;
            }
            console.log(cryptoData);
            sign(JSON.stringify(cryptoData));
        };
        var sign = function (data) {
            var ready = false;
            var checkStatus = function () {
                if (ready || _this.stop)
                    return;
                _this.info.log('checkStatus');
                jQuery.ajax({
                    type: "GET",
                    method: "GET",
                    url: _this.scheme + "://localhost:" + _this.BISSPort + "/status",
                    crossDomain: true,
                    cache: false,
                    dataType: "json",
                    complete: function (result) {
                        _this.info.log("BISS status: ");
                        _this.info.log(result);
                        var res = result.responseJSON;

                        if (res.status > 0) {
                            //Започнала обработка
                            _this.info.statusCallback(res, _this.info);
                        }
                        else if (res.status = 0) {
                            //Не е започнала обработка
                        }
                        else {
                            _this.info.log('status error');
                            _this.info.log(res);
                            // this.response(callback, -42, 'SIGNING_FAILED');//TODO - да се помисли за съобщение, че не може да бъде върнат статус
                        }
                        if (!ready) {
                            setTimeout(function () { checkStatus(); }, _this.checkStatusTimeout);
                        }
                    }
                });
            };
            try {
                _this.info.msg('SIGNING_DESCRIPTION');
                var xhr = jQuery.ajax({
                    type: "POST",
                    method: "POST",
                    url: _this.scheme + "://localhost:" + _this.BISSPort + "/sign",
                    crossDomain: true,
                    data: data,
                    contentType: "application/json",
                    dataType: "json",
                    complete: function (result) {
                        _this.info.log("BISS result: ");
                        _this.info.log(result);

                        var res = result.responseJSON || null;
                        if (res && res.status && res.status == 'ok') {
                            _this.info.log('signedContent = ' + res.signatures);
                            _this.info.log('execute callback - ' + callback);
                            _this.response(callback, 40, 'CONTENT_SUCCESS_SIGNED', res.signatures);
                        }
                        else {
                            _this.info.log('Sign not ok');
                            _this.info.log(result);
                            _this.response(callback, -41, 'SIGNING_FAILED', res.signatures);
                        }
                        ready = true;
                    }
                });
                setTimeout(function () { checkStatus(); }, _this.checkStatusTimeout);
            }
            catch (e) {
                _this.info.log(e);
                _this.info.log('No hash 2');
                _this.stopSigning(function (callback) { return _this.response(callback, -43, 'SIGNING_FAILED'); });
                return;
            }
        };
        var checkCertAndSign = function (signedHashes) {
            if (!_this.selectNewCert && _this.signerCertificate) {
                _this.info.log('Cert available calling check and signing');
                _this.check(function (res) {
                    _this.info.log('Check result: ' + JSON.stringify(res));
                    if (res.status > 0) {
                        _this.info.log('BISSSigning without selecting cert');
                        if (signImediately) {
                            _this.info.log('signImediately1: ');
                            _this.info.log(signedHashes);
                            sign(signedHashes);
                        }
                        else {
                            _this.info.log('prepareAndSign: ' + signedHashes);
                            prepareAndSign(signedHashes);
                        }
                    }
                    else {
                        _this.info.log('Error at checkCertAndSign');
                        _this.response(callback, res.status, res.responseCode, res.responseData);
                    }
                });
            }
            else {
                _this.info.log('selectCert at sign');
                _this.selectCert(function (resCert) {
                    if (resCert.status > 0) {
                        _this.info.log('BISSSigning after selectCert');
                        if (signImediately) {
                            _this.info.log('signImediately signedHashes:');
                            _this.info.log(signedHashes);
                            sign(signedHashes);
                        }
                        else {
                            _this.info.log('prepareAndSign 2: ' + signedHashes);
                            prepareAndSign(signedHashes);
                        }
                    }
                    else {
                        _this.info.log('Error probably BISS not accessible: ' + resCert.responseText);
                        _this.response(callback, resCert.status, resCert.responseCode);
                    }
                });
            }
        };
        checkCertAndSign(contentToSign);
    };
    BissSigning.prototype.getHashAndSign = function (contentToSign, getHashUrl, callback) {
        var _this = this;
        this.info.log('Calling getHash');
        this.getHash(contentToSign, getHashUrl, function (res) {
            if (res.status > 0) {
                _this.info.log('Hash received calling checkCertAndSign');
                _this.checkAndSign(res.responseData, callback);
            }
            else {
                _this.response(callback, res.status, res.responseCode);
            }
        });
    };
    BissSigning.prototype.checkAndSign = function (content, callback) {
        var _this = this;
        this.stop = false;
        this.info.msg('CHECK');
        this.sign(content, function (res) {
            if (res.status === -10) {
                if (!_this.isStopped(callback)) {
                    _this.BISSOpen(jQuery('body', _this.info.doc), function (res) {
                        _this.info.msg('BISS_START');
                        var retryCnt = 0;
                        var chkRes = function (result) {
                            retryCnt++;
                            if (!_this.isStopped(callback)) {
                                if (result.status > -1) {
                                    _this.info.log('BISS started successfully ' + retryCnt);
                                    _this.checkAndSign(content, callback);
                                }
                                else if (retryCnt < _this.BISSRetryCnt) {
                                    _this.info.log('Checking BISS ' + retryCnt);
                                    _this.check(chkRes);
                                }
                                else {
                                    _this.info.log('Stop retrying at retry: ' + retryCnt);
                                    _this.info.result(result);
                                    _this.response(callback, result.status, result.responseCode);
                                }
                            }
                        };
                        chkRes({ status: -1 });
                    });
                }
            }
            else {
                _this.info.result(res);
                _this.response(callback, res.status, res.responseCode, res.responseData);
            }
        });
    };
    BissSigning.prototype.checkAndStart = function (callback) {
        var _this = this;
        this.stop = false;
        this.check(function (res) {
            if (res.status > 0) {
                _this.response(callback, res.status, res.responseCode, res.responseData);
            } else if (res.status == -5) {
                _this.response(callback, res.status, res.responseCode, res.responseData);
            }
            else {
                if (!_this.isStopped(callback)) {
                    _this.BISSOpen(jQuery('body', _this.info.doc), function (res) {
                        _this.info.msg('BISS_START');
                        var retryCnt = 0;
                        var chkRes = function (result) {
                            retryCnt++;
                            if (!_this.isStopped(callback)) {
                                if (result.status > -1) {
                                    _this.info.log('BISS started at retry: ' + retryCnt);
                                    _this.checkAndStart(callback);
                                }
                                else if (retryCnt < _this.BISSRetryCnt) {
                                    _this.info.log('Checking BISS ' + retryCnt);
                                    _this.check(chkRes);
                                }
                                else {
                                    _this.info.log('Stop retrying: ' + retryCnt);
                                    _this.response(callback, result.status, result.responseCode, result.responseData);
                                }
                            }
                        };
                        chkRes({ status: -1 });
                    });
                }
            }
        });
    };
    /**
     * Извършва тестово подписване, като не се обръща към услугата за генериране на хеш, а използва заложения тестови
     */
    BissSigning.prototype.testSign = function (content, callback, getHashUrl) {
        this.stop = false;
        this.test = true;
        this.checkAndSign(content, callback);
    };
    BissSigning.prototype.stopSigning = function (callback) {
        this.stop = true;
        var res = this.response(null, -100, 'STOPPED');
        this.info.result(res);
        callback(res);
    };
    /**
     * Обръща се към услуга за криптиране на хеша на съдържанието, което трябва да се подпише.
     * При успех извиква callback, на който подава съдържанието в responseData на отговора във формат: cryptoData
     *  @param contentToSign - съдържанието, което да бъде подписано
     *  @param getHashUrl - Адресът на услугата, която ще генерира Хеш на съдържанието
     *  @param (out) callback(responseType)
     */
    BissSigning.prototype.getHash = function (contentToSign, getHashUrl, callback) {
        var _this = this;
        jQuery.ajax({
            type: 'POST',
            method: "POST",
            url: getHashUrl,
            data: {
                contentToSign: contentToSign,
                HashAlgorithm: this.getHashAlgorithm()
            },
            dataType: 'json',
            crossDomain: true,
            accepts: {
                text: "application/json"
            },
            success: function (data) {
                _this.response(callback, 20, 'HASH_RECEIVED', data);
            },
            error: function (res) {
                _this.response(callback, -22, 'HASH_NOT_RECEIVED');
            }
        });
    };
    /**
     * Прави обръщение към BISS за избор на сертификат.
     * При успех връща избрания сертификат в responseData на отговора
     *  @param callback(responseType)
     *  @param showValidCerts - ако се подаде true - връща списък с всички валидни сертификати, ако се подаде false - връща списък с всички сертификати, включително изтеклите
     *  @param msg код на съобщението
     */
    BissSigning.prototype.selectCert = function (callback, showValidCerts, msg) {
        var _this = this;
        try {
            var selCert = function () {
                if (_this.stop)
                    return;
                msg = msg || 'SELECT_CERT';
                _this.info.msg(msg);
                _this.info.log('SELECT_CERT', ' port: ' + _this.BISSPort);
                var data = {
                    selector: {
                        issuers: ["CN=B-Trust TEST Operational Qualified CA", "CN=B-Trust Operational Qualified CA", "CN=B-Trust TEST Operational CA QES", "CN=B-Trust Operational CA QES",
                            "CN=B-Trust TEST Operational Advanced CA", "CN=B-Trust Operational Advanced CA"]
                    },
                    showValidCerts: true
                };
                jQuery.ajax({
                    type: "POST",
                    url: _this.scheme + "://localhost:" + _this.BISSPort + "/getsigner",
                    timeout: _this.timeout,
                    crossDomain: true,
                    data: JSON.stringify(data),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (result) {
                        if (result.reasonCode == 200) {
                            _this.signerCertificate = result.chain[0];
                            _this.info.log('Cert selected: ' + _this.signerCertificate);
                            _this.response(callback, 30, 'SELECT_CERT_SUCCESS', _this.signerCertificate);
                        }
                        else {
                            _this.info.log('success result' + result);
                            _this.info.log(callback);
                            _this.response(callback, -31, 'LBL_CERT_SEL_ERR');
                        }
                    },
                    error: function (err) {
                        _this.info.log(err);
                        _this.info.log(callback);
                        _this.response(callback, -32, 'LBL_CERT_SEL_ERR');
                    }
                });
            };
            try {
                this.info.log('Checking BISS at selectCert');
                this.checkAndStart(function (res) {
                    if (res.status > 0) {
                        _this.info.log('BISS active at selectCert');
                        selCert();
                    }
                    else if (res.status == -5) {
                        _this.info.log('Old BISS version at selectCert');
                        _this.response(callback, -5, res.responseCode);

                    }
                    else {
                        _this.info.log('No BISS at selectCert');
                        _this.response(callback, -10, res.responseCode);
                    }
                });
            }
            catch (err) {
                this.info.log(err);
                this.response(callback, -10, 'NO_CONNECTION');
            }
        }
        catch (error) {
            this.info.log(error);
            this.info.log(callback);
            this.response(callback, -33, 'LBL_CERT_SEL_ERR');
        }
    };


    /**
     * Прави опит за старитиране на приложението (на Windows), чрез обръщане към bissopen URI схема от динамично генериран iframe.
     * Обръщението се прихваща от HKEY_CLASSES_ROOT\bissOpen, който се опитва да стартира BISS.
     * Стартирането е възможно само, когато услугата е стартирана на операционна система Windows.
     *  @param DOMelement - елемент, към който да бъде прикрепен iframe-ът
     *  @param callback(responseType)
     *  @param check - дали да провери за активна услуга или направо да опита да я стартира
     */
    BissSigning.prototype.BISSOpen = function (DOMelement, callback, check) {
        var _this = this;
        if (check === void 0) { check = false; }
        try {
            DOMelement = (DOMelement instanceof HTMLElement) ? DOMelement : document.body;
            var open_1 = function () {
                if (jQuery('#BISSFrame'), DOMelement) {
                    _this.info.log('Removing iframe');
                    jQuery('#BISSFrame').remove();
                }
                _this.info.log('Adding iframe');
                var fr = "<iframe id=\"BISSFrame\"\n                                    style=\"visibility: hidden;\"\n                                    src=\"bissopen:start\">\n                        </iframe>";
                jQuery(DOMelement).append(jQuery(fr));
                setTimeout(function () {
                    _this.check(function (res) {
                        _this.info.log('check timeout res:');
                        _this.info.log(res);
                        if (res.status > 0) {
                            _this.response(callback, res.status, 'BISS_ACTIVE');
                        }
                        else {
                            _this.response(callback, res.status, res.responseCode);
                        }
                    });
                }, 5000);
            };
            if (check) {
                this.info.log('Calling check at BISSOpen');
                this.check(function (res) {
                    if (res.status < 1) {
                        open_1();
                    }
                    else {
                        _this.response(callback, 0, 'BISS_ACTIVE');
                    }
                });
            }
            else {
                this.info.log('Direct start at BISSOpen');
                open_1();
            }
        }
        catch (e) {
            this.info.log('BISSOpen error');
            this.info.log(e);
            this.info.error();
        }
    };
    BissSigning.prototype.close = function (callback) {
        var _this = this;
        this.info.log('BISS_CLOSING');
        try {
            this.check(function (res) {
                if (res.status > 0) {
                    jQuery.ajax({
                        type: "POST",
                        method: "POST",
                        url: _this.scheme + "://localhost:" + _this.BISSPort + "/close",
                        timeout: _this.timeout,
                        crossDomain: true,
                        dataType: "json",
                        success: function (res, status) {
                            _this.info.log('Close complete response: ');
                            _this.info.log(res);
                            _this.info.log('status: ' + status);
                            if (status == 'success') {
                                _this.info.log('BISS_CLOSED');
                                _this.response(callback, 110, 'BISS_CLOSED');
                            }
                            else {
                                _this.response(callback, -110, 'BISS_CLOSED_ERROR');
                            }
                        },
                        error: function (err) {
                            _this.info.log('Close error: ');
                            _this.info.log(err);
                            _this.response(callback, -111, 'BISS_CLOSED_ERROR');
                        }
                    });
                }
                else {
                    _this.response(callback, 111, 'BISS_CLOSED');
                }
            });
        }
        catch (error) {
            this.info.log('Close catch: ');
            this.info.log(error);
            this.response(callback, -112, 'BISS_CLOSED_ERROR');
        }
    };
    /**
     * Прави проверка, дали подадения алгоритъм за хеширане се поддържа
     */
    BissSigning.prototype.setHashAlgorithm = function (hashAlgorithm) {
        this.info.log('Setting HashAlgorithm: ' + hashAlgorithm);
        if (this.hashAlgorithms.indexOf(hashAlgorithm) > -1) {
            return this.hashAlgorithm = hashAlgorithm;
        }
        else {
            this.info.log('HashAlgorithm not supported: ' + this.hashAlgorithms);
            throw new Error(this.info.getMsg('ALGORITHM_ERR'));
        }
    };
    BissSigning.prototype.getHashAlgorithm = function () {
        return this.hashAlgorithm;
    };
    BissSigning.prototype.getCert = function () {
        return this.signerCertificate;
    };
    /**
     * Проверява, дали потребителя не е инициирал спиране на процеса и подава отговора на info.response
     * @param callback - функцията, на която ще се подаде форматирания отговор, като параметър
     * @param status - положителен - успех, отрицателен - неуспех
     * @param responseCode - кода на съобщението от messages.js
     * @param responseData - допълнителен параметър, който ще се върне на callback функцията и съдържа специфична информация, която ще се ползва от нея
     */
    BissSigning.prototype.response = function (callback, status, responseCode, responseData) {
        callback = (!this.stop) ? callback : null;
        return this.info.response(callback, status, responseCode, responseData);
    };
    /**
     * Конвертира отговора от формат на BISSResponseType, в този на responseType
     * и го връща през info. Всички кодове (BISSResponse.reasonText), които връща услугата BISS,
     * задължително трябва да имат съответния responseCode в съобщенията в messages.js
     * @param callback - функция, която да се извика и на която се подава форматирания отговор
     * @param BISSResponse - отговор от BISS
     * @param status - При неподаден BISSResponse.reasonCode се връща този статус
     * @param responseCode - При неподаден BISSResponse.reasonText се връща този код
     * @param responseData - Евентуални допълнителни параметри, които да се върнат на callback за последваща обработка
     */
    BissSigning.prototype.BISSResponse = function (callback, BISSResponse, status, responseCode, responseData) {
        callback = (!this.stop) ? callback : null;
        return this.info.response(callback, status, BISSResponse.reasonText || responseCode, responseData);
    };
    BissSigning.prototype.isStopped = function (callback) {
        if (this.stop) {
            this.response(callback, -100, 'STOPPED');
            return true;
        }
    };
    BissSigning.prototype.isFunction = function (obj) {
        return !!(obj && obj.constructor && obj.call && obj.apply);
    };
    return BissSigning;
}());
