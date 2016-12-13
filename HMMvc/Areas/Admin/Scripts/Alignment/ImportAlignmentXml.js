

$(document).ready(function () {

   
    $(".FileUpload").fineUploader({
        request: {
            endpoint: "/Admin/AlignmentXml/UploadXml"
        },
        validation: {
            allowedExtensions: ["xml"],
            //sizeLimit: 2048000 // 2MG = 2000 * 1024 bytes  
        },
        text: {
            uploadButton: " <input type=\"button\" id=\"imprt\" value=\"בחר קובץ\"/>"
        },
        messages: {
            typeError: "סוג קובץ לא מתאים{extensions}",
            sizeError: "גודל לא מתאים"
        },
        multiple: false,
        debug: true
    }).on("complete", function (event, id, fileName, responseJSON) {
        if (responseJSON.success) {
            $.LoadHtmlAndChooseMacines();
        }
    });

    $.LoadHtmlAndChooseMacines = function () {
        $("#Step3").hide();
        $("#Step4").hide();
        $("#Step2").show();
        $.Scroll("#Step2");
        $("#Step2").html(ajax_load);
        $.post("/Admin/Import/ImportVbLoadHtmlAndChooseMacines", {
            //lng: "en-US"

        }, function (data) {
            $("#Step2").html(data);
           
        }).fail(function () { $.BugAlert(); });
    };

    $.ChooseJob = function () {
        $("#Step3").show();
        $("#Step3").html(ajax_load);
        $.Scroll("#Step3");
        $.post("/Admin/Import/ImportVbChooseJob", {

        }, function (data) {
            $("#Step3").html(data);
           
        }).fail(function () { $.BugAlert(); });
    };

    $.CommitImport = function () {
        $("#Step4").show();
        $("#Step4").html(ajax_load);
        $.Scroll("#Step4");

        $.post("/Admin/Import/ImportVbCommitImport", {
            "ClientID": ClientID.value,
            "JobID": $("#jobs").val(),
        }, function (data) {
            var msg = (data.JobID == null) ?
                (data.msg || "קרתה שגיאה בייבוא...") :
                 "<h3>הקובץ על בהצלחה!</h3> <a class=\"linktext\" href=\"/Admin/VB/VBAnalysis?JobID=" + data.JobID + "\">המשך לניתוח הדוח</a>";;
         
            $("#Step4").html(msg);
            
        }).fail(function () { $.BugAlert(); });

    };

    $.ToggleInclude = function (that, entity) {
        $.post("/Admin/Import/ImportVbToggleIncludes", {
            "Id": $(that).attr("data-id"),
            "Include": $(that).hasClass("out"), //toggle not being done yet so search "out"
            "Entity":entity,
        }, function (data) {
            $(that).toggleClass("in").toggleClass("out");
        }).fail(function () { $.BugAlert(); });
    };

    $.Scroll = function (trgt) {
        //tabs
        window.scrollTo(10, 175);
        $(".importsubcontent").scrollTo($(trgt), 700);

    };

    /*****************         UI FUNCTIONS          ******************/

   

    //$('#imprt').live('click', function () {
    //    $.LoadHtmlAndChooseMacines();

    //});

    $("#ToStep3Btn").live("click", function () {
        $.ChooseJob();
    });

    $("#ImportReportBtn").live("click", function () {
        $.CommitImport();
    });

    $("#Step2 .ImportItemDiv").live("click", function () {
        $.ToggleInclude($(this), "Machine");
    });

    $("#Step3 .ImportItemDiv").live("click", function () {
        $.ToggleInclude($(this), "Date" );      
    });

   

    $("#JobCreateBtn").live("click", function () {
        $("#jobs").prop("selectedIndex", -1);
        $("#JobType").text("עבודה חדשה");
        $("#ImportDivZone").show();
        $.Scroll("#ImportDivZone");
    });

    $("#jobs").live("change", function () {
        $("#JobType").text(this.options[this.selectedIndex].innerHTML);
        $.Scroll("#ImportDivZone");
    });
    



});








