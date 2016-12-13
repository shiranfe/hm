
var callInProccess = new Array();




var rest = (function ($) {

    var get = function (ops) {
        //ops.url = ops.cntrl + '/Update';
        ops.type = "Get";
        ops.showUpdateGif = false;
        customAjax(ops);
    };

    var getContent = function (ops) {
       
        customAjax($.extend({}, ops, {
            type: "Get",
            showUpdateGif: false,
            url : ops.cntrl + "/IndexContent",
            htmlSuccessTo:"#pageContent"
        }));
    };

    var post = function (ops) {
        ops.type = "Post";

       customAjax(ops);
    };

    var update = function (ops) {
        ops.url = ops.cntrl + "/Update" ;
        ops.type = "Post";
     

        customAjax(ops);


    };

    var add = function (ops) {
        ops.url = ops.cntrl + "/Add";
        ops.type = "Post";

        customAjax(ops);


    };

    var copy = function (ops) {
        ops.url = ops.cntrl + "/Copy";
        ops.type = "Post";

        customAjax(ops);
    };

    var sort = function (ops) {
        ops.url = ops.cntrl + "/Sort";
        ops.type = "Post";

        customAjax(ops);
    };

    var remove = function (ops) {
        ops.url = ops.cntrl + "/Delete";
        ops.type = "Post";

        customAjax(ops);
    };

   

    /*****************     ******************/
    var customAjax = function(options) {
        // vars
        var ops = $.extend({}, defaults, options);
        var IsHtmlLoad = ops.htmlSuccessTo != null;// && ops.type=='Get';
        var showLongLoading = false;

        function Init() {

            startCall();

            $.ajax({
                type: ops.type,
                dataType: ops.dataType,
                contentType: ops.contentType,
                url: ops.url,
                data: ops.data,
                traditional: ops.traditional,
            })
                //.done(function (data) {
                //    finishCall();
                //    onDone(data);
                //})
                .success(function (data) {
                    
                    finishCall();
                    onSuccess(data);
                })
                .fail(function (xhr, textStatus, errorThrown) {
                   
                    finishCall();
                    OnFail(xhr);
                });
        };

        if (callInProccess[ops.url] != true)
            Init();


        function startCall() {
            callInProccess[ops.url] = true;

            showLongLoading = true;

            setLoadingGif();

            if (ops.btn != null)
                $(ops.btn).prop('disabled', true);


        };

        function finishCall() {
            callInProccess[ops.url] = false;

            showLongLoading = false;
            $("#jNotify").remove();


            if (ops.done != null)
                ops.done();


            hideLoading(ops.url);
        };



        //function onDone(data) {

        //    if (data.sts == "Success" || data.sts == undefined || !data.msg)
        //        onSuccess(data);
        //    else
        //        OnError(data);

        //    if (ops.done != null)
        //        ops.done(data);

        //};

        function onSuccess(data) {
            if (IsHtmlLoad)
                $(ops.htmlSuccessTo).html(data);

            if (ops.showSuccessNotification)
                $.SuccessAlert();

            if (ops.btn != null && ops.unDisableBtn) {
                $(ops.btn).prop('disabled', false);
            }

            if (ops.success == null)
                return;

            ops.success(data);

        };

        //logic error
        function OnError(data) {

            var errTxt = data.msg || "זה לא אתם זה אנחנו... נסו שוב מאוחר יותר";
            if (ops.showErrorNotification)
                $.BugAlert(errTxt);

            if (ops.htmlFailTo != null)
                $(ops.htmlFailTo).html(data);

            if (ops.btn != null) {
                $(ops.btn).prop('disabled', false);
            }

            if (ops.fail == null)
                return;

            ops.fail(data);


        };

        // technical fail
        function OnFail(data) {
          
            if(!data.msg)
                data.msg = (data.responseText.indexOf("<!DOCTYPE html>") > -1) ? data.statusText : data.responseText;

            try {
                console.log( JSON.parse(data.msg));
            } catch (e) {
                console.log(data.msg);
            }
            var errTxt = "זה לא אתם זה אנחנו... נסו שוב מאוחר יותר. ";

            //if (IsHtmlLoad)
            //    $(ops.htmlSuccessTo).html(errTxt);

            if (ops.showErrorNotification)
                $.BugAlert(errTxt);

            if (ops.fail != null)
                ops.fail(data);

            if (ops.btn != null) {
                $(ops.btn).prop('disabled', false);
            }

            if ( ops.showLoadingGif) {
                $(ops.htmlSuccessTo).html(errTxt);
            }
           
        };

        function setLoadingGif() {
            if (IsHtmlLoad && ops.showLoadingGif) {
                $(ops.htmlSuccessTo).html(ajax_load);
                return;
            }

            if (ops.showUpdateGif) {
                showLoading(ops.url);
            }
            //if (ops.showProccessNotification) {
            //    setTimeout(function () {
            //        ShowLongProccess();
            //    }, 4000);

            //}

        };

        function ShowLongProccess() {

            if (showLongLoading)
                $.NotifProccess("please wait...");
        };

    }

    var defaults = {
        type: "Post",
        url: null,
        //, cache: false,
        data: null,
        traditional: true,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",

        showErrorNotification: true,
        showSuccessNotification: false,
        showProccessNotification: true,
        showLoadingGif: true,/** html loading gif*/
        showUpdateGif:true, /** show bottom right loading gif*/
        // dataType: "json",
        htmlSuccessTo: null, // $(htmlSuccessTo).html(data);
        htmlFailTo: null, // $(htmlFailTo).html(data);
        btn: null,//button to disable or make 'loading gif'
        unDisableBtn: true, // if needs to redirect, use it to keep the button disables
        ///callbacks
        success: null,
        fail: null,
        done: null,

    };


    return {
        get: get,
        getContent:getContent,
        post:post,
        add: add,
        update: update,
        remove: remove,
        copy: copy,
        sort:sort,
        customAjax: customAjax
    };


})(jQuery);