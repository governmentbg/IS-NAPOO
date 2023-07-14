;
;
;
var Info = /** @class */ (function () {
    function Info(debug, messages) {
        if (debug === void 0) { debug = false; }
        this.doc = document;
        this.logMessages = [];
        this.debug = debug;
        this.messages = messages;
    }
    Info.prototype.dialogCallback = function (data, infoObj) { };
    ;
    Info.prototype.resultCallback = function (response, infoObj) { };
    ;
    Info.prototype.waitCallback = function (data, infoObj) { };
    ;
    Info.prototype.endWaitCallback = function (data, infoObj) { };
    ;
    Info.prototype.statusCallback = function (data, infoObj) { };
    ;
    Info.prototype.dialog = function (header, description, confirmText, confirmCallback, cancelText, cancelCallback, response, noclose) {
        if (noclose === void 0) { noclose = false; }
        var data = {
            header: this.getMsg(header),
            description: this.getMsg(description),
            confirmText: confirmText,
            confirmCallback: confirmCallback,
            cancelText: (cancelText) ? this.getMsg(cancelText) : this.getMsg('CANCEL_BTN_TXT'),
            cancelCallback: cancelCallback,
            response: response,
            noclose: noclose
        };
        this.log('Извиква диалог');
        this.log(data);
        this.dialogCallback(data, this);
    };
    /**
     * Визуализира съобщение, което представлява край на действие
     * @param response - обект от тип responseType
     */
    Info.prototype.result = function (response) {
        this.endWait();
        this.log('Info result:');
        this.log(response);
        this.resultCallback(response, this);
    };
    Info.prototype.wait = function (msg) {
        this.log('Process start');
        msg = msg || 'WAIT';
        this.waitCallback(this.getMsg(msg), this);
    };
    Info.prototype.endWait = function () {
        this.log('Process end');
        this.endWaitCallback(false, this);
    };
    Info.prototype.getLog = function () {
        this.msg('SHOW_LOG');
        return this.logMessages;
    };
    Info.prototype.response = function (callback, status, responseCode, responseData) {
        var responseText = this.getMsg(responseCode);
        var response = { status: status, responseText: this.getMsg(responseCode), responseCode: responseCode, responseData: responseData };
        if (this.isFunction(callback))
            callback(response);
        return response;
    };
    Info.prototype.msg = function (key, endStr) {
        if (endStr === void 0) { endStr = ''; }
        var msg = this.getMsg(key, endStr);
        this.dialog('SIGNING', msg);
        this.log(msg);
        return msg;
    };
    Info.prototype.getMsg = function (key, endStr) {
        if (endStr === void 0) { endStr = ''; }
        key = key || this.getMsg('WAIT');
        return (this.messages.hasOwnProperty(key)) ? this.messages[key] + endStr : key + endStr;
    };
    Info.prototype.log = function (msg) {
        msg = (this.messages.hasOwnProperty(msg)) ? this.messages[msg] : msg;
        this.logMessages.push(msg);
        if (this.debug)
            console.log(msg);
        return msg;
    };
    Info.prototype.isFunction = function (obj) {
        return !!(obj && obj.constructor && obj.call && obj.apply);
    };
    Info.prototype.success = function (msg, code) {
        var errCode = msg || 'CONTENT_SUCCESS_SIGNED';
        var errNum = code || 100;
        var response = {
            status: errNum,
            responseCode: errCode,
            responseText: this.getMsg(errCode),
            responseData: ''
        };
        this.resultCallback(response, this);
    };
    /**
     * Форматира responce със съобщение за грешка
     * @param responseCode - код на грешката
     * @param status - номер на грешката
     */
    Info.prototype.error = function (responseCode, status) {
        if (responseCode === void 0) { responseCode = 'SIGN_ERR'; }
        if (status === void 0) { status = -100; }
        var response = {
            status: status,
            responseCode: responseCode,
            responseText: this.getMsg(responseCode),
            responseData: ''
        };
        this.endWait();
        this.resultCallback(response, this);
    };
    return Info;
}());
