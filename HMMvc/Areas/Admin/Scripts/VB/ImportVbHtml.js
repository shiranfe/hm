var h = $(window).height() - 10;

$(document).ready(function () {

   

    var steps = {
        load: "LoadHtmlAndChooseMacines",
        choose: "ChooseJob",
        commit: "CommitImport"
    };

    function postStep(action, data) {
        var step = "#" + action;
        action = "ImportVb" + action;
     
        $(step).show();
        scrollTo(step);

        rest.post({
            url: "/Admin/Import/" + action,
            htmlSuccessTo: step,
            data:data
        });
    }


    function toggleInclude(that, entity) {

        rest.post({
            url: "/Admin/Import/ImportVbToggleIncludes",
            data: {
                "Id": $(that).attr("data-id"),
                "Include": $(that).hasClass("out"), //toggle not being done yet so search "out"
                "Entity": entity,
            },
            success: function (data) {
                $(that).toggleClass("in").toggleClass("out");
            }
        });
    }



    function loadHtmlAndChooseMacines() {
        $("#" + steps.choose).hide();
        $("#" + steps.commit).hide();

        postStep(steps.load);
    }


    function chooseJob() {
        postStep(steps.choose);
    }

    function commitImport() {
        postStep(steps.commit, {
            "ClientID": $(".ImportBtnRow #ClientID").val(),//ClientID.value,
            "JobID": $("#jobs").val(),
        });
    }

    //$.ChooseJob = function () {
     
    //    $.post("/Admin/Import/ImportVbChooseJob", {

    //    }, function (data) {
    //        $("#Step3").html(data);
           
    //    }).fail(function () { $.BugAlert(); });
    //};


   

    //$.CommitImport = function () {
    //    $("#Step4").show();
    //    $("#Step4").html(ajax_load);
    //    scrollTo("#Step4");

    //    $.post("/Admin/Import/ImportVbCommitImport", {
    //        "ClientID": $(".ImportBtnRow #ClientID").val(),//ClientID.value,
    //        "JobID": $("#jobs").val(),
    //    }, function (data) {
    //        var msg = (data.JobID == null) ?
    //            (data.msg || "קרתה שגיאה בייבוא...") :
    //             "<h3>הקובץ על בהצלחה!</h3> <a class=\"linktext\" href=\"/Admin/VB/VBAnalysis?JobID=" + data.JobID + "\">המשך לניתוח הדוח</a>";;
         
    //        $("#Step4").html(msg);
            
    //    }).fail(function () { $.BugAlert(); });

    //};



    //$.ToggleInclude = function (that, entity) {
    //    $.post("/Admin/Import/ImportVbToggleIncludes", {
    //        "Id": $(that).attr("data-id"),
    //        "Include": $(that).hasClass("out"), //toggle not being done yet so search "out"
    //        "Entity":entity,
    //    }, function (data) {
    //        $(that).toggleClass("in").toggleClass("out");
    //    }).fail(function () { $.BugAlert(); });
    //};

    function scrollTo (trgt) {
        //tabs
      //  window.scrollTo(10, 175);
        $("body").scrollTo($(trgt), 700);

    }

    /*****************         UI FUNCTIONS          ******************/

    //$(".importsubcontent").css("height", h);

    $(".FileUpload").fineUploader({
        request: {
            endpoint: "/Admin/Import/UploadVbHtml"
        },
        validation: {
            allowedExtensions: ["html", "htm"],
            //sizeLimit: 2048000 // 2MG = 2000 * 1024 bytes  
        },
        text: {
            uploadButton: " <input type=\"button\" id=\"imprt\" value=\"בחר קובץ\"/>"
        },
        //callbacks:{
        //    onProgress: function (id, fileName, loaded, total) {
        //        console.log(loaded + " of " + total);
        //    },
        //},
        messages: {
            typeError: "סוג קובץ לא מתאים{extensions}",
            sizeError: "גודל לא מתאים"
        },
        multiple: false,
        debug: true
    }).on("complete", function (event, id, fileName, responseJSON) {
        if (responseJSON.success) {
            loadHtmlAndChooseMacines();
        }
        else {
            $.BugAlert(responseJSON.msg);
        }
    });


    $("#ToStep3Btn").live("click", function () {
        chooseJob();
    });

    $("#ImportReportBtn").live("click", function () {
        commitImport();
    });

    $("#LoadHtmlAndChooseMacines .ImportItemDiv").live("click", function () {
        toggleInclude($(this), "Machine");
    });

    $("#ChooseJob .ImportItemDiv").live("click", function () {
        toggleInclude($(this), "Date" );      
    });

   

    $("#JobCreateBtn").live("click", function () {
        $("#jobs").prop("selectedIndex", -1);
        $("#JobType").text("עבודה חדשה");
        $("#ImportDivZone").show();
        scrollTo("#ImportDivZone");
    });

    $("#jobs").live("change", function () {
        $("#JobType").text(this.options[this.selectedIndex].innerHTML);
        scrollTo("#ImportDivZone");
    });
    



});








