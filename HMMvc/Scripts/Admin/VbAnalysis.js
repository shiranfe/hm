

$(document).ready(function () {
    $.LoadVBMachine = function (MachineID) {
        $("#VBMachineDiv").html(ajax_load);
        var pg = "/Admin/VB/VBMachine";
        $.get(pg, {
            "MachineID": MachineID,
            "JobID": JobID
        }, function (data) {
            $("#VBMachineDiv").html(data);

        }).fail(function () { $.BugAlert(); });
    };

    $.UpdateHiddenResualt = function (PointResultID, Hidden) {
       
        var pg = "/Admin/VB/VBUpdateHiddenResualt";
        $.post(pg, {
            "PointResultID": PointResultID,
            "Hidden": Hidden,
        }, function (data) {
            $.MarkDuplicates();
        }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
    };

    $.VBUpdateResualtStatus = function (PointResultID, StatusID) {

        var pg = "/Admin/VB/VBUpdateResualtStatus";
        $.post(pg, {
            "PointResultID": PointResultID,
            "StatusID": StatusID,
        }, function (data) { }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
    };

    $.VBUpdateMultyResualtStatus = function ( StatusID) {
        var MachineID = $(".SideMacList .linktext.selected").attr("data-id");
        var pg = "/Admin/VB/VBUpdateMultyResualtStatus";
        $.post(pg, {
            "MachineID": MachineID,
            "JobID": JobID,
            "StatusID": StatusID,
        }, function (data) { }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
    };

    $.VBUpdatePublishReport = function (IsPosted) {
        var pg = "/Admin/VB/VBUpdateIsPosted";
        $.post(pg, {
            "JobID": JobID,
            "IsPosted": IsPosted,
        }, function (data) {
            $("#PublishVbReport").toggleClass("green").toggleClass("red");

        }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
    };

    $.MarkDuplicates = function () {
        $("#VbEditReportTable tbody tr").attr("style", "");
        var $tableRows = $("#VbEditReportTable tbody tr:not(.RowDis)");

        $tableRows.each(function (n) {
            var id = $(this).attr("data-id");
            var resualtypeid = $(this).attr("data-resualtypeid");

            $tableRows.each(function (n) {
                if ($(this).attr("data-id") != id) {

                    if ($(this).attr("data-resualtypeid") == resualtypeid) {

                        $(this).css("color", "red");
                    }
                }
            });
        });
    };


    $.DeleteJobVB = function () {
        var pg = "/Admin/VB/DeleteJobVB";
        $.post(pg, {
            "JobID": JobID,
        }, function (data) {
            location.href ="/Admin/VB/Index";

        }).fail(function () { $.BugAlert("המחיקה לא בוצעה"); });
    };

    /*****************         UI FUNCTIONS          ******************/

    

    $(".SideMacList .linktext")[0].classList.add("selected");

    $(".SideMacList .linktext:not(.selected)").live("click", function () {
        $(".SideMacList .linktext").removeClass("selected");
        $(this).addClass("selected");
        $.LoadVBMachine($(this).attr("data-id"));
    });

    $("#item_IsHidden").live("click", function () {
        var row = $(this).closest("tr");
        $(row).toggleClass("RowDis");
        var el = $(row).find("select");

        $.ToggleDisable(el);
      
        
        var PointResultID = $(this).closest("tr").attr("data-id");
        var Hidden = $(this).prop("checked");
        $.UpdateHiddenResualt(PointResultID, Hidden);

    });

    $(".StatusByCombo").live("change", function () {
        var StsID = $(this).find("option:selected").val(); 
        $(this.parentNode).attr("data-id", StsID);
        var PointResultID = $(this).closest("tr").attr("data-id");
        $.VBUpdateResualtStatus(PointResultID, StsID);
    });

    $("#ChangeAllSts").live("change", function () {
        if (confirm("הינך עומד לשנות סטטוס לכל הערכים, האם להמשיך?")) {
            var StsID = $(this).find("option:selected").val();
            $(".OuterSelect").attr("data-id", StsID);
            $(".SelectVal").text($(this).find("option:selected").text());
            $.VBUpdateMultyResualtStatus(StsID);
        }
    });

    $("#PublishVbReport").live("click", function () {
        var IsPosted = $(this).attr("data-val")=="True";
      
        $.VBUpdatePublishReport(!IsPosted);
    });


    $("#DeleteJobIcon").live("click", function () {
        if (confirm("האם אתה בטוח שהינך מעוניין למחוק את העבודה? שים לב, לא ניתן לשחזר אותה"))
        {
            $.DeleteJobVB();
        }
        
    });
});








