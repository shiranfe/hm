var lastPage = "start";
var mciApp;
var Routing = function (appRoot, contentSelector, defaultRoute) {
    var loadeOk = true;

    var isAdmin = location.href.indexOf("Admin") > -1;

    var defultRoute = isAdmin ? appRoot + "#/Home/Dashboard" : "/#/VB/CurrentSts";

    function authError(data) {
        if (!$(data).find("#LoginForm").is("*"))
            return false;


        location.replace(  isAdmin ? "/Admin/Account/Login" : "/User/Login");

        "/Admin/Account/Login";

        return true;
        //console.log($(data).find("#LoginForm"));
    }

    function error(data, loadingId) {

        if (authError(data))
            return;

        finishLoading(loadingId);
        loadeOk = false;
        $.BugAlert("משהו פה לא בסדר... | " + data.statusText);



    }

   

    function finishLoading(loadingId) {
        setTimeout(function myfunction() {
            hideLoading(loadingId);
            $(".searchFields input").prop('disabled', false);
        }, 100);

       

    }

    function subRoute(context, subContainer, mainUrl) {

        /** make sure right menu and subContainer is in loaded
        if subcontrainer is empty' load sub url
        shuold replace the main page url in history
        */

        /** if only layout loaded or this is  not the right page, redirect to main page url.
        hapens on refresh or first routing to this page*/
        if (!$(subContainer).is("*"))
            return location.replace(mainUrl);

        /** load only sub page content*/
        standartRoute(context, subContainer);


    }

    /** chnge route to page with sub menu.
   every item in submenu has href to subUrl*/

    function partialPage(ops) {
        //var ops = {
        //    context, subPage, setUrl, subContainer
        //}

        function init() {
            var subPage = ops.subPage;
            var subContainer = ops.subContainer;


            var loadMainPage = !$(subContainer).is("*") || subPage == "null";
            var paramsStr = serialize(ops.params.toHash());

            /** check if main id was chagnged*/
            var lastParams = lastPage.split("?")[1];
            var reloadPage = lastParams && splitAndLeave(lastParams, '=', true) != splitAndLeave(paramsStr, '=', true);

            /** if only layout loaded || this is  not the right page || samepage diffrent main id
                redirect to main page url.*/
            if (loadMainPage || reloadPage) {
                subPage = "main";
                subContainer = null;

            }
            else {
                $(".SiteSideMenu").removeClass("selected");
                $(".SiteSideMenu[data-id=" + subPage + "]").addClass("selected");

            }

            var url = ops.setUrl(subPage) + "?" + paramsStr;

            changeUrl(ops.context, url, subContainer, function success() {
                if (loadMainPage || reloadPage) {

                    /** if doesnt know sub menu id in advenced (example: "machinePointId") get from loaded menu*/
                    if (ops.subPage == "null") {
                        var refurishUrl = location.href.replace("null", $(".SiteSideMenu.selected").data("id"));
                        /** loaded refbrsh with jobId and notreufbrihrtId*/
                        if (ops.params.JobRefubrishPartID!==undefined && ops.params.JobRefubrishPartID == "0")
                            refurishUrl = refurishUrl.replace("/0/", "/" + $("#Parts").val()+ "/" );

                        return location.replace(refurishUrl);
                    }

                    init();
                }

            });
        }

        init();

    }

    

    function standartRoute(context, parital, midCall) {
        var hash = context.path;
        var url = hash.replace('#/', '');

        if (url === appRoot && (lastPage === url || lastPage == "start"))
            return;//url = defaultRoute;

        if (midCall && midCall())
            return;

        changeUrl(context, url, parital);
    }

    function changeUrl(context, url, parital, success) {

        if (backAsClose()) {
            /** return the url to current*/
            return window.history.forward();

            //var current =lastPage.replace("/Admin/","/Admin/#/");
            //return history.pushState(null,  current,current);
        }
            

        var loadingId = "routing_" + url;
        var pageChanged = lastPage.split("?")[0] != getPath(false, true);
      
        if (loadeOk)
            startLoading(loadingId, pageChanged);


        clearPage();

        var path = url.split("?")[0];
        $(contentSelector).attr('data-page', path);

        var title = ".mobileMenuBtns h1";
        $(title).text("...");


        context
            .load(url, {
                cache: path != getPath(false, true),
                error: function (data) { error(data, loadingId) },
                success: function (data) {
                    if (authError(data))
                        return;

                    $(parital || contentSelector).html(data);

                    CreateDropDown();

                    scrollToTop();

                    $(title).text($(".sitesubcontent  h1:first-child, .sitesubcontent > h2:first-child").text());

                    if (success)
                        success();

                    setViewSettings();

                    finishLoading(loadingId);
                    //setContainer();
                    // console.log($(".sitesubcontent > h2 ").text());
                }

            })

        ;
        lastPage = url;

        function startLoading(loadingId, pageChanged) {
            ///** set loading gif instead of current page if not search r popup */
            if (!parital && pageChanged)
                return $(contentSelector).html(ajax_load);
         
            setTimeout(function myfunction() {
                showLoading(loadingId);
                $(".searchFields input").prop('disabled', true);
            }, 100);

           
    }
    }

    ///** set partial container (#divName) or defualt ("#SiteBody")*/
    //function setContainer(container) {
    //    mciApp.element_selector = container || contentSelector;
    //    console.log(mciApp.element_selector);
    //}

    /** change partial page, while chaning url (serach, or submenu {steps}... )*/
    function filterd(name, context) {


        /** need to load side menu first*/
        //if ($(contentSelector).children().length == 0)
        //   return location.href = "#/" + name;

        /** "filter.On" method*/
        //standartRoute(context, context.params.On ? "#innerContent" : null);


        /** need to reload filters again, becuase no way to know values when click "back".
            if had the values could use the "filter.On" method (refubrish/index example)*/
        standartRoute(context, null, function initLastSerach() {
            /** initaite last search params.
            if local storage is empty - will set fefualt params in server*/
            if (!urlParamsToObj()) {
                var params = JSON.parse(localStorage.getItem(name));
                if (params) {
                    replaceUrlParams(params);
                    return true;
                }

            }

            return false;
        });
    }

    /** before redirecing clear data that stays because there is no post back*/
    function clearPage() {

        /** reset cruds and its binding*/
        for (var i = 0; i < crudEnts.length; i++) {
            crudEnts[i].unbindAll();
            delete crudEnts[i];
        }
        crudEnts = [];


        closeAllPopup();

        /** delete all scripts leftovers*/
        var elem = $("body > script").last().next()
        while (elem.is("*")) {
            $(elem).remove();
            elem = $("body > script").last().next();
        }

        /** hide mobile menu mask*/
        if ($("#menu-mask").is(":visible")) {
            togglemenuElems();

        }


    }

    return {
        init: function () {

            mciApp = Sammy(contentSelector, function () {

                /*********** ADMIN ZONE **********/

                this.get('/Admin/#/Quote', function (context) {
                    filterd("Quote", context);
                });

                this.get('/Admin/#/Refubrish', function (context) {
                    filterd("Refubrish", context);
                });

                this.get('/Admin/#/Machine', function (context) {
                    filterd("Machine", context);
                });

                this.get('/Admin/#/VB', function (context) {
                    filterd("VB", context);
                });

                this.get('/Admin/#/Client', function (context) {
                    filterd("Client", context);
                });

                //this.get('/Admin/#/Refubrish/Step', function (context) {

                //    /** need to load side menu first*/
                //    if ($(contentSelector).children().length == 0)
                //        return location.href = "#/Refubrish/Refubrish?JobRefubrishPartID=" + context.params.JobRefubrishPartID + "&step=" + context.params.step;

                //    $(".Step").removeClass("selected");
                //    $(".Step[data-id=" + context.params.step + "]").addClass("selected").removeClass("selctee");

                //    standartRoute(context, "#RefubrishStepDiv");
                //});


                this.get('/Admin/#/Refubrish/:JobRefubrishPartID/:step', function (context) {


                    partialPage({
                        context: context,
                        subPage: this.params["step"],
                        params: this.params,
                        subContainer: "#RefubrishStepDiv",
                        setUrl: function (subPage) {
                            if (subPage == "main")
                                return "/Admin/Refubrish/Refubrish";

                            return "/Admin/Refubrish/Step";
                        }

                    });

                });

                /** defualt machinePointId - null, so will get selected from loaded html menu*/
                this.get("/Admin/#/Refubrish/:JobRefubrishPartID", function (context) {
                    if (!this.params["JobID"])
                        return location.replace(location.href + "/null");

                    var url = location.href.split('?');
                    location.replace(url[0] + "/null?" + url[1]);
                    
                });

       

                //this.get(/Admin\/#\/.*\/MachineJobs|Admin\/#\/Machine\/Update/, function (context) {

                //    subRoute(context, "#machinePageContent",
                //        "/Admin/#/Machine/Page?id=" + context.params.id + "&tab=" + context.params.tab);

                //    //var a = [ "Admin/#/vb/MachineJobs" ,"Admin/#/Machine/Update?id=12138"];
                //});




                /*********** CUSTOMER **********/
                //this.get('/#/Home/Dashboard', function (context) {
                //    location.href = "/#/VB/CurrentSts";
                //});






                //this.get(/\/.*\/MachineJobs|\/Machine\/Update/, function (context) {

                //    subRoute(context, "#machinePageContent",
                //        "/#/Machine/Page?id=" + context.params.id + "&tab=" + context.params.tab);

                //    //var a = [ "#/vb/MachineJobs" ,"#/Machine/Update?id=12138"];
                //});


                this.get("#/VBMachine/:JobID/:MachineID/:machinePointID", function (context) {
                    partialPage({
                        context: context,
                        subPage: this.params["machinePointID"],
                        params: this.params,
                        subContainer: "#pointPageContent",
                        setUrl: function (subPage) {
                            if (subPage == "main")
                                return "/VBMachine/MachinePage";

                            return "/VBMachine/PointPage";
                        }

                    });
                });

                /** defualt machinePointId - null, so will get selected from loaded html menu*/
                this.get("#/VBMachine/:JobID/:MachineID", function (context) {
                    location.replace(location.href + "/null");
                });

                //this.get(/VBMachine\/PointPage/, function (context) {

                //    subRoute(context, "#pointPageContent",
                //        "/#/VBMachine/MachinePage?JobID=" + context.params.JobID + "&MachineID=" + context.params.MachineID + "&machinePointID=" + context.params.machinePointID);

                //    //var a = [ "#/VBMachine/MachinePage?JobID=3081&MachineID=6849", "#/VBMachine/PointPage?JobID=3081&MachineID=6849"];
                //});

                /*********** COMMON **********/


                this.get("#/Machine/:id/:subId", function (context) {

                    partialPage({
                        context: context,
                        subPage: this.params["subId"].toLowerCase(),
                        params: this.params,
                        subContainer: "#machinePageContent",
                        setUrl: function myfunction(subPage) {

                            switch (subPage) {
                                case "main":
                                    return appRoot + "Machine/Page";
                                case "details":
                                    return appRoot + "Machine/Update";
                                default:
                                    return appRoot + subPage + "/MachineJobs";
                            }
                        }

                    });

                });

                /** defualt machine*/
                this.get("#/Machine/:id", function (context) {
                    location.replace(location.href + "/details");
                });

                this.get('/' + appRoot, function (context) {
                    var hash = context.path;
                    var url = appRoot;

                    if (lastPage === url || lastPage == "start")
                        return;//url = defaultRoute;

                    changeUrl(context, url);
                });



                this.get(/\#\/(.*)/, function (context) {

                    standartRoute(context);
                });

                this.bind('reload', function (e, data) {
                    this.redirect(getPath(true)); // force redirect
                });
            })
            .run(defultRoute)
            ;
        }
    };
}

function backAsClose() {
   
    if ($(".PopFrame").is(":visible")) {
        closeAllPopup(); return true;
    }

    if ($("#menu-mask").is(":visible")) {
        togglemenuElems(); return true;
    }

    return false;

}

/** only on submit*/
//window.onbeforeunload = function () {
//    var ans = backAsClose();
//    console.log(ans);
//    return ans;
//};