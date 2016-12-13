/// <reference path="UI/jquery.qtip.min.js" />

var jss = new Array();
var dollar;
var idName, jobType;
var waitingUploadCrop = [];


jQuery.cachedScript = function (url, options) {

    // Allow user to set any option except for dataType, cache, and url
    options = $.extend(options || {}, {
        dataType: "script",
        cache: true,
        url: url
    });

    // Use $.ajax() since it is more flexible than $.getScript
    // Return the jqXHR object so we can chain callbacks
    return jQuery.ajax(options);
};

function LoadScript(scrpt,ops) {

    $.cachedScript(scrpt, ops).done(function (data, textStatus, jqxhr) {
        var fileName = splitAndLeave(scrpt, null, true).replace(".js", "_ready");
         //console.log("ready - " + fileName);
        if (isFunction(fileName)) {
            window[fileName]();
        }
    });

}

function tryLoadScript(scrpt) {
   
    if (!jss[scrpt]) {
        jss[scrpt] = true;
      //  console.log("loading script " + scrpt);
        LoadScript(scrpt);
    }
}

function isFunction(elem) {
    return elem != undefined && (typeof elem == 'function' || typeof window[elem] == 'function');
}


function CreateDropDown() {
    $("select:not(.selectbox)").each(function () {
        var placeholder = $(this).attr('placeholder');
        var defualtVal = $(this).find("option[selected]").text() || this.options[this.selectedIndex].innerHTML || placeholder || "";
        var classes = this.classList.value;
        var combo = $("<div class=\"OuterSelect\" data-id=\""+ classes + "\">" +
                            "<span class=\"SelectVal\">" + defualtVal + "</span>" +
                            "<span class=\"SelectBtn\"></span>" +
                       " </div>");
        $(this).before($(combo));
        $(this).addClass("selectbox");
        $(combo).append($(this));
    });
}


/** $.setAutoCompleteVal for .autocomplete*/
function changeDropSelected(combo, value) {
    $(combo).val(value);
    var placeholder = $(combo).attr('placeholder');
    var text = value || placeholder || "";
    $(combo).parents(".OuterSelect ").find('.SelectVal').text(text);
}

var ajax_load = "<div class='LoadGif'><img src='/images/Loading.gif' alt='loading...'  /></div>";
var small_Loading = "<div class='small-Loading'><img src='/images/SendMsgLoad.gif' alt='loading...'  /></div>";


function showSmallLoading(elem) {
    //$(elem).after(small_Loading);
}

function hideSmallLoading(elem) {
    //$(".small-Loading").remove();
}

function initPivot(data, ops) {
    tryLoadScript("/Areas/Admin/Scripts/Report/mainReport.js");
    tryLoadScript('/Scripts/UI/Pivot/pivot.js');


    if (isFunction($("#output").pivotUI) && isFunction('reportPivot'))
        return reportPivot(data, ops);


    setTimeout(function () {
        initPivot(data, ops);
    }, 100);

}


var height;
function jqUpdateSize() {
    // Get the dimensions of the viewport

    height = $(window).height();

    $("#SiteBody").css("min-height", $(window).height() - 300);
    $(".VbTabInner").css("height", height - 120); // also called in MachieVbTab.chhtml
}

$(window).resize(jqUpdateSize);


function unbindBackButton() {
    $(document).unbind("keydown");
}

function closePopup(id, dontRemove) {
    unbindBackButton();
    var pop = $("#" + id);
    if (pop.length == 0 || !pop) {
        console.log("popup " + id + " doesnt exist");
    }
    else {
        pop.bPopup().close();
        if (!dontRemove)
            pop.remove();
    }


}

function closeAllPopup() {
    if ($(".PopFrame").length) {
        unbindBackButton();
        $(".PopFrame").bPopup().close();
        $(".PopFrame").remove();
        $(".b-modal").remove();
    }

}


function setPopTitle(entity, title, notUpdate) {
    if (!notUpdate)
        entity = "Update" + entity;
    if (!title)
        title = "עדכון פריט";

    $('#pop' + entity + ' .PopHead_title')[0].innerHTML = title;
}

/**
 *  load partial view indead of page
 * @returns {} 
 */
function partialLoad(withSideMenu, path, success) {
    showLoading("partialLoad");
    var elem = withSideMenu ? "#SiteBody" : ".sitesubcontent";
    closeAllPopup();

    mciApp.refresh();

    /** include crud reset*/
    //mciApp.trigger('reload', mciApp);


    //$.get(path || getPath(), function (pageHtml) {

    //    /** fox for sammy*/
    //    var wrap = document.createElement("div");
    //    $(wrap).html(pageHtml);

    //    var $contnt = $(wrap).find(".sitesubcontent");
    //    if (withSideMenu)
    //        $contnt = $contnt.parent();

    //    $(elem).replaceWith($contnt);

    hideLoading("partialLoad");

    if (lastSearch) {
        $("#srch").val(lastSearch).trigger("keyup");

        lastSearch = null;
    }

    if (success)
        success();



    // CreateDropDown();
    // });
}


function showLoading(id) {
    //data-ind=' + loaderId + '
    $("body").append('<div class="loading-div-action shadow corn_3" data-id=' + id + '><img src="/images/Loading.gif" alt="loading..."></div>');
    // console.log("show " + id);
}

function hideLoading(id) {

    var elem = $(".loading-div-action[data-id='" + id + "']");
    if (elem.length) {
         
        return elem.remove();
    }

  //  console.log("nothing to hide " + id);

    //setTimeout(function () {
    //    hideLoading(id);
    //}, 300);
}

function strCurrencyToFloat(val) {
    if (!val)
        return 0;

    var num = val.toString().replace("₪", "").replace(/,/g, "");//remove ',' , 'ש"ח'

    return parseFloat(num);
}

function toNis(val) {
    var num = strCurrencyToFloat(val).toFixed(2);

    var numFormated = numeral(num).format("0,0.00");

    return "₪ " + numFormated;
}

function toNisRound(val) {
    var num = strCurrencyToFloat(val);

    var numFormated = numeral(num).format("0,0");

    return "₪ " + numFormated;
}

function getDollar() {
    var script = document.createElement("script");
    var from = "USD", to = "ILS";

    script.setAttribute("src", "http://query.yahooapis.com/v1/public/yql?q=select%20rate%2Cname%20from%20csv%20where%20url%3D'http%3A%2F%2Fdownload.finance.yahoo.com%2Fd%2Fquotes%3Fs%3D" + from + to + "%253DX%26f%3Dl1n'%20and%20columns%3D'rate%2Cname'&format=json&callback=parseExchangeRate");
    document.body.appendChild(script);
}

function parseExchangeRate(data) {
    dollar = parseFloat(data.query.results.row.rate, 10);
    console.log(dollar);
}


function getUrlParamValue(param) {
    var qs = location.search;
    qs = qs.split("+").join(" ");

    var params = {}, tokens,
        re = /[?&]?([^=]+)=([^&]*)/g;

    while (tokens = re.exec(qs)) {
        params[decodeURIComponent(tokens[1])]
            = decodeURIComponent(tokens[2]);
    }

    return params[param];
}


function log(data) {
    console.log(data);
}

function logRed(msg) {
    console.log('%c ' + msg, "color:red");
}


function scrollToError() {
    var elem = $("#SiteBody .input-validation-error,#SiteBody .field-validation-error").first();
    if (elem.is("*"))
        scrollTo(elem);
}

function scrollTo(elem) {
    try {
        $('html, body').animate({
            scrollTop: $(elem).offset().top - 20
        }, 500);
    //  $('body').scrollTo($(elem).offset().top - 20);
    } catch (e) {

    }
  
}

function scrollToTop() {

    $('html,body').animate({ scrollTop: 0 }, 'slow');
}

/** object to url param*/
function serialize(obj) {
    return $.param(obj);
    //var str = "";
    //for (var key in obj) {
    //    if (str != "") {
    //        str += "&";
    //    }
    //    str += key + "=" + encodeURIComponent(obj[key]);
    //}
    //return str;
}

/** url param to object*/
function deserialize(search) {
    return JSON.parse('{"' + decodeURI(search).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g, '":"').replace(/\+/g, ' ').replace(/\%2F/g, '/') + '"}');
}


function sanitize(string) {
    // console.log($.trim(string).replace(/\s+/g, '').toLowerCase());
    return $.trim(string).replace(/\s+/g, ' ').toLowerCase();
}

function urlParamsToObj() {
    var params = location.href.split("?")[1];

    return params ? deserialize(params) : null;

}

function setUrlParams(data) {


    var path = location.href.split("?")[0];
    path += "?" + serialize(data);
    location.href = path;

}

function replaceUrlParams(data) {


    var path = location.href.split("?")[0];
    path += "?" + serialize(data);
    location.replace(path);

}


function urlParam(paramName) {
    var searchString = location.hash.split("?")[1];

    /** param in slash cant find it... ( ... Refubrish/1255/DetailsStep ) */
    if (!searchString) {

        return null;
    }

    var i, val, params = searchString.split("&");

    for (i = 0; i < params.length; i++) {
        val = params[i].split("=");
        if (val[0] == paramName) {
            return val[1];
        }
    }
    return null;
}



function getPath(withHash, withoutSearch,url) {
    var path = location.pathname + (withHash ? location.hash : location.hash.replace("#/", ""));
    if (withoutSearch)
        path = path.split("?")[0];

    return path;
}


function isInPage(page) {
    return $("[data-page='" + page + "']").is("*");
}



function refreshPage() {
    $("#SiteBody").html(ajax_load);
    mciApp.refresh();
}

/** split str by char and remove the lasr/first match*/
function splitAndLeave(str, char, last) {
    char = char || "\/";

    char = new RegExp(char, 'g');

    var split = str.split(char);
    var i = last ? split.length - 1 : 0;
    return split[i];//.slice(0, i).join(char) + char;

}

/** 25/07/16 12:50 -> 07/25/16 12:50*/
function replaceDate(hebDate) {
    return hebDate.replace(/(\d{2})\/(\d{2})\/(\d{2})/, "$2/$1/$3");
}

function togglemenuElems() {
    //$("#menu-mask").toggle();
    //$(".SiteSideMenu:not(.Step,.menuInfoDiv),#StepsMenu").slideToggle(300);
    $("#perspective").toggleClass("animate");
}

//$(".mobileMenuBtns .menu").live('click',function () {
   
//});

function setViewSettings() {
    var mobileView = window.matchMedia("(max-device-width: 1024px)");
    var clickable = "#menu-mask, .SiteSideMenu:not(.menuInfoDiv) a,.menuInfoDiv a, #SideMenuAccordion a";
    //.mobileMenuBtns .menu,
    /** clear previos binding*/
 //   $(clickable).unbind("click");
    $(".mobileMenuBtns .back").unbind("click");


    if (mobileView.matches) {
        //$(clickable).bind('click', function () {
        //    togglemenuElems();
        //});

        $(".mobileMenuBtns .back").bind('click', function () {
            window.history.back();
        });
    }

}

/**   /Date(456465545)/ to datetime */
function encodedToTime(str) {
    return moment(JSON.parse(str.replace(/\D+/g, "")));
}

$(document).ready(function () {



    jqUpdateSize();



    createPopModal = function (name,title) {
        var popID = "pop" + name;
        var ht = "<div class=\"PopFrame corn_3\" id=\"" + popID + "\">" +
                    "<div class=\"PopMainHead\">" +
                        "<div class=\"PopHead_title\">" +( title || "") + "</div>" +
                        "<span class=\"rec-icon PopCloseButton  b-close\"></span></div>" +
                 "<div class=\"PopContent\" id=\"" + popID + "_Content\">" +
                "</div></div>";

        $("body").append(ht);
    };

    function makeAPopUp(name, content) {
        var pop = "#pop" + name;

        $(pop).bPopup({
            contentContainer: pop + "_Content",
            closeClass: name + "_close",
            follow: [false, false] //x, y
            //loadUrl: url,
            //loadData: parm,
        });

        $(pop + "_Content").html(content);
    }

    function loadPopModal(name, url, parm) {
        var pop = "#pop" + name;
        makeAPopUp(name, ajax_load);

        rest.get({
            url: url,
            data: parm,
            success: function (html) {
                $(pop + "_Content").html(html);
                CreateDropDown();
            }
        });

    };

    $.popup = function (pop) {
        $(pop).bPopup();
    };


    //$("input, textarea").live("keydown", function (e) {
    //    if (e.which === 8) {
    //        e.preventDefault();
    //    }
    //});

    $.OpenGeneralPop = function (name, controller, parm, cache, url, allowBack) {

        //if(!allowBack){
        //$(document).on("keydown" ...
        //}


        var pop = "#pop" + name;
        if (!$(pop).is("*") || !cache) {//if was not loaded
            $(pop).remove();

            createPopModal(name);

            url = url || "/" + controller + "/" + name;

            loadPopModal(name, url, parm);

            //PopTitle is inserted in view
        }
        else {
            $.popup(pop)
        }


    }


    $.createAndOpenPop = function (name, content,title) {
        $("#pop" + name).remove();
        createPopModal(name, title);
        makeAPopUp(name, content);
    }


    var alertOps = {
        //content: {
        //    text: msg,
        //    //title: {
        //    //    text: 'Attention!',
        //    //    button: true
        //    //}
        //},
        position: {
            target: [0, 0],
            container: $("#qtip-growl-container")
        },
        show: {
            event: false,
            ready: true,
            effect: function () {
                $(this).stop(0, 1).animate({ height: "toggle" }, 400, "swing");
            },
            delay: 0,
            persistent: false
        },
        hide: {
            event: false,
            effect: function (api) {
                $(this).stop(0, 1).animate({ height: "toggle" }, 400, "swing");
            }
        },
        style: {
            width: 250,
            //classes: 'qtip-red',
            tip: false
        },
        events: {
            render: function (event, api) {
                if (!api.options.show.persistent) {
                    $(this).bind("mouseover mouseout", function (e) {
                        var lifespan = 5000;

                        clearTimeout(api.timer);
                        if (e.type !== "mouseover") {
                            api.timer = setTimeout(function () { api.hide(e) }, lifespan);
                        }
                    })
                    .triggerHandler("mouseout");
                }
            }
        }
    }

    $.BugAlert = function (msg) {

        if (msg == null)
            msg = "Oops, you found a bug...\n\n A report was sent to us, and the bug will be fixed in no time!\n Thank you for you Patience.";

        var target = $(".qtip.jgrowl:visible:last");

        $.extend(alertOps, {
            content: {
                text: msg
            },
            style: {
                classes: "qtip-red",
            }
        });

        $("<div/>").qtip(alertOps);
    };

    $.SuccessAlert = function (msg,ops) {
        if (ops) {
            ops = $.extend({},alertOps, ops);
        }
        else {
            if (msg == null)
                msg = "הפעולה בוצעה בהצלחה";

            var target = $(".qtip.jgrowl:visible:last");

            ops = $.extend({}, alertOps, {
                content: {
                    text: msg
                },
                style: {
                    classes: "qtip-green",
                }
            });
        }
     
        $("<div/>").qtip(ops);
    };

    $.ToggleDisable = function (el) {
        if ($(el).attr("disabled")) $(el).removeAttr("disabled");
        else $(el).attr("disabled", "disabled");

    };

    $.OpenUpdateMachine = function (params) {
        $.OpenGeneralPop("UpdateMachine", null, params, false, "/Admin/Machine/Update");
    }

    /*****************         UI FUNCTIONS          ******************/

    //$.fn.qtip.defaults.show.event = "click";
    //$.fn.qtip.defaults.hide.event = "unfocus";
    $.fn.qtip.defaults.style.classes = "qtip-Black qtip-rounded qtip-shadow";
    $.fn.qtip.defaults.position.at = "top center";
    $.fn.qtip.defaults.position.my = "bottom center";// + DirLast;


    $(".PopCloseButton").live("click", function () {

        closePopup($(this).parents(".PopFrame").attr("id"));
    });


    $(".OpenAdminMacPage").live("click", function () {
        var id = $(this).attr("data-id");
        location.href = "/Admin/Machine/" + id;

    });

    $("#RefreshCacheBtn").live("click", function () {
        $.post("/Admin/Home/RefreshCache", function (data) {
            location.reload();
        });

    });

    $(".selectbox").live("change", function () {
        $.updateSelectOutter($(this));


    });

    $.updateSelectOutter = function (select) {
        var selectVal = select.parent().find("span.SelectVal");//$(this).attr('class').replace('selectbox ', '#SelectVal.');

        selectVal.text(select.find("option:selected").text());
    };

    $(".EditableSelect").live("change", function () {
        var outtr = $(this).parent().parent();
        var txtArea = $(outtr).next();

        if ($(this).find("option:selected").val() != "AddOption") {
            var txtbox = txtArea.find("input");
            txtbox.val($(this).val());
            txtbox.valid();
        }
        else {

            $(this).val("");
            txtArea.show();
            outtr.hide();
        }

    });

    $(".EditabelText .close").live("click", function () {
        var txtArea = $(this).parent();
        var outtr = $(txtArea).prev();
        var select = $(outtr).find("select");

        select.find("option:selected").val(null);
        $.updateSelectOutter(select);

        txtArea.find("input").val(null);

        txtArea.hide();
        outtr.show();

    });

    CreateDropDown();

    $("select").live("change", function () {
        $(this).before().before().innerHtml = $(this).innerHtml;
    });

    $.changeDrop = function (drop, val) {
        $(drop).val(val).change();
    };


    /** SWITCH BTN **/

    function changeSwitchBtn($SwitchDivInner, value) {
        /** 0 - true, -100 = false, -50 =none*/
        //var marginRight = value == "" ? -50 : (value =="true" ? 0 : -100);


        // var $SwitchDivInner = $(btn).parent();
        // var marginRight = parseInt($SwitchDivInner.css("marginRight"), 10) == margin ? -50 : margin;
        // $SwitchDivInner.animate({ marginRight: marginRight });

        var $HiddenValue = $SwitchDivInner.parent().find("input");
        // var val = marginRight == -50 ? "" : marginRight == 0;
        $SwitchDivInner.attr("data-val", value);
        $HiddenValue.val(value);
        try {
            $HiddenValue.valid();
        } catch (e) {

        }

    }

    $(".SwitchBtn.true").live("click", function () {
        var $SwitchDivInner = $(this).parent();
        var newVal = $SwitchDivInner.attr("data-val") == "true" ? "" : "true";
        changeSwitchBtn($SwitchDivInner, newVal);
    });

    $(".SwitchBtn.false").live("click", function () {
        var $SwitchDivInner = $(this).parent();
        var newVal = $SwitchDivInner.attr("data-val") == "false" ? "" : "false";
        changeSwitchBtn($SwitchDivInner, newVal);
    });


    $('.SwitchDivInner').live("swipeleft", function () {
        changeSwitchBtn($(this), "true");
    });

    $('.SwitchDivInner').live("swiperight", function () {
        changeSwitchBtn($(this), "false");
    });



    if ($.browser.msie) {
        $("input[placeholder]").each(function () {

            var input = $(this);

            $(input).val(input.attr("placeholder"));

            $(input).focus(function () {
                if (input.val() == input.attr("placeholder")) {
                    input.val("");
                }
            });

            $(input).blur(function () {
                if (input.val() == "" || input.val() == input.attr("placeholder")) {
                    input.val(input.attr("placeholder"));
                }
            });
        });
    };


    $.openUpdateClient = function (params) {
        params = params || {};
        $.OpenGeneralPop("UpdateClient", "Admin/Client", params, false, "/Admin/Client/Update");
    };

    $(".AddClientBtn").live("click", function () {
        $.openUpdateClient();

    });

    $(".EditClientBtn").live("click", function () {
        var ClientID = $(this).attr("data-id");
        $.openUpdateClient({ "id": ClientID });
    });


    $(".addContact").live("click", function () {
        var clientID = $("#ClientID").val();
        if (clientID)
            $.openUpdateClient({ "id": clientID, });
        else
            alert("יש לבחור לקוח תחילה");
    });

    $(".EditMachineBtn").live("click", function () {
        var macID = $(this).attr("data-id");
        if (macID > 0) {
            $.OpenGeneralPop("UpdateMachine", null, { "id": macID }, false, "/Admin/Machine/Update");
        }

    });





    setViewSettings();

    //$(window).on("orientationchange", function () {
    //    //  setViewSettings();
    //    location.reload();
    //});

    //$("a").live("click", function (e) {
    //    var href = $(this).attr("href");
    //    if (!href)
    //        return true;

    //   // partialLoad(true, href);

    //    return false;

    //});

    function monitorDownload(downloadID) {
        showLoading("monitorDownload");
        var cookiePattern = new RegExp(("downloadID=" + downloadID), "i");

        var cookieTimer = setInterval(checkCookies, 500);

        // I check the local cookies for an update.
        function checkCookies() {
            // If the local cookies have been updated, clear
            // the timer and say thanks!
            if (document.cookie.search(cookiePattern) >= 0) {
                clearInterval(cookieTimer);
                hideLoading("monitorDownload");
                return (
                    console.log("Download complete!!")
                );
            }
            //console.log(
            //    "File still downloading...",
            //    new Date().getTime()
            //);
        }
    }


    $("a[download]").live('click', function (event) {

        var target = event.target;

        var downloadID = (new Date()).getTime();// The local cookie cache is defined in the browser
        var param = "downloadID=";

        /** if clicked for second time, overide param in href*/
        if (target.href.indexOf(param) > -1) {
            target.href = target.href.split(param)[0];
        }
        else {
            var sign = target.href.indexOf("?") > -1 ? "&" : "?";
            target.href += sign;
        }

        target.href += param + downloadID;


        monitorDownload(downloadID);

    });


});

function trackDownload() {

}

var timer;

var typewatch = (function () {

    return function (elem, callback, ms) {

        // $(elem).prop('disabled', true);
        clearTimeout(timer);

        timer = setTimeout(function done() {
            callback();
            //console.log($("#srch").val());
            //  $(elem).prop('disabled', false);
        }, ms);
    };
})();

