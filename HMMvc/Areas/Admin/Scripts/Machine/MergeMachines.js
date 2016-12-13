var MacNewMergeID;
var MacOldMergeID;

$(document).ready(function () {
 
    function loadClientMacs (ClientID,contentDiv) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = "MergeMachinesClientSelected";
        $.post(pg, {
            "ClientID": ClientID,
        }, function (data) {
            $(contentDiv).html(data);

        }).fail(function () { $.BugAlert(); });
    }

    function mergeMachines  () {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = "MergeMachines";
        $.post(pg, {
            "MacNewMergeID": MacNewMergeID,
            "MacOldMergeID": MacOldMergeID,
        }, function (data) {
            $.SuccessAlert("מיזוג בוצע בהצלחה");
            loadClientMacs($("#NewClientID").val(), "#NewMacs .MacList");
            loadClientMacs($("#OldClientID").val(), "#OldMacs .MacList");
            MacNewMergeID = MacOldMergeID = null;
            $("#NewMacText").text("בחר מכונה");
            $("#OldMacText").text("בחר מכונה");

        }).fail(function () { $.BugAlert(); });
    }

    /*****************         UI FUNCTIONS          ******************/

    $("#OldClientID").change(function () {
        var id = $(this).val();
        loadClientMacs(id, "#OldMacs .MacList");
    });

    $("#NewClientID").live("change", function () {
        var id = $(this).val();
        loadClientMacs(id, "#NewMacs .MacList");
    });


    $("#OldMacs .MacName:not(.selected, .disabled)").click(function () {
        $("#OldMacs .MacName").removeClass("selected");
        $(this).addClass("selected");

        MacOldMergeID = this.getAttribute("data-id");
        $("#NewMacs .MacName").removeClass("disabled");
        $("#NewMacs .MacName[data-id='" + MacOldMergeID + "']").addClass("disabled");

       
        $("#OldMacText").text($(this).text());
    });

    $("#NewMacs .MacName:not(.selected, .disabled)").click(function () {
        $("#NewMacs .MacName").removeClass("selected");
        $(this).addClass("selected");

        MacNewMergeID = this.getAttribute("data-id");
        $("#OldMacs .MacName").removeClass("disabled");
        $("#OldMacs .MacName[data-id='"+MacNewMergeID+"']").addClass("disabled");


      
        $("#NewMacText").text($(this).text());
    });

    $("#MergeMachinesBtnConfirm").click( function () {
        if (confirm("האם אתה בטוח כי הינך מעוניים למזג את המכונות?")) {
            mergeMachines();
        }
    });

 

});








