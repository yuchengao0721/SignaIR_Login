$(function () {
    window.Modal = function () {
        var reg = new RegExp("\\[([^\\[\\]]*?)\\]", 'igm');
        var alr = $("#ycf-alert");
        var ahtml = alr.html();

        //关闭时恢复 modal html 原样，供下次调用时 replace 用
        //var _init = function () {
        //	alr.on("hidden.bs.modal", function (e) {
        //		$(this).html(ahtml);
        //	});
        //}();

        /* html 复原不在 _init() 里面做了，重复调用时会有问题，直接在 _alert/_confirm 里面做 */

        var _hideAlertOk = function () {
            alr.find('.ok').click();
        };
        var _hideAlertCancel = function () {
            alr.find('.cancel').click();
        };
        var _alert = function (options) {
            alr.html(ahtml);	// 复原
            alr.find('.ok').removeClass('btn-success').addClass('btn-primary');
            alr.find('.cancel').hide();
            _dialog(options);

            return {
                on: function (callback) {
                    if (callback && callback instanceof Function) {
                        alr.find('.ok').click(function () { callback(true) });
                    }
                }
            };
        };

        var _confirm = function (options) {
            alr.html(ahtml); // 复原
            alr.find('.ok').removeClass('btn-primary').addClass('btn-success');
            alr.find('.cancel').show();
            _dialog(options);

            return {
                on: function (callback) {
                    if (callback && callback instanceof Function) {
                        alr.find('.ok').click(function () { callback(true) });
                        alr.find('.cancel').click(function () { callback(false) });
                    }
                }
            };
        };

        var _dialog = function (options) {
            var ops = {
                msg: "提示内容",
                title: "操作提示",
                btnok: "确定",
                btncl: "取消"
            };

            $.extend(ops, options);

            console.log(alr);
            if (ops.btnok==null)
                alr.find('.ok').hide();
            if (ops.btncl == null)
                alr.find('.cancel').hide();
            var html = alr.html().replace(reg, function (node, key) {
                return {
                    Title: ops.title,
                    Message: ops.msg,
                    BtnOk: ops.btnok,
                    BtnCancel: ops.btncl
                }[key];
            });

            alr.html(html);
            alr.modal({
                width: 500,
                backdrop: 'static'
            });
        }

        return {
            alert: _alert,
            confirm: _confirm,
            hideAlertOk: _hideAlertOk,
            hideAlertCancel: _hideAlertCancel,
            dialog: _dialog
        }

    }();
})