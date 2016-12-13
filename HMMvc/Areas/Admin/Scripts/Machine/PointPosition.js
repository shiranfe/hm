
$(document).ready(function () {
    var ctrl = "/Admin/Machine/";


    $.LoadClientMac = function (ClientID) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = ctrl+ "ClientSelectedMachines";
        $.post(pg, {
            "ClientID": ClientID,
        }, function (data) {
            $("#ClientSelectedDiv").html(data);

        });
    };

    $.LoadDefMachine = function (MachineID) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = ctrl+ "PointPositionSelected";
        $.post(pg, {
            "MachineID": MachineID,
        }, function (data) {
            $("#DefSelMac").html(data);

        });
    };

    $.UpdatePointXY = function (id,x,y) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = ctrl+ "UpdatePointXY";
        $.post(pg, {
            "MachinePointID": id,
            "X":x,
            "Y":y
        }, function (data) {
            if (data.res == "1") {
                //alert("הערך נשמר");
            }

        });
    };

    $.UpdatePointShow = function (PointID,Show) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = ctrl+ "UpdatePointShow";
        $.post(pg, {
            "MachinePointID": PointID,
            "Show": Show,
        }, function (data) {
            if (data.res == "1") {
                //alert("הערך נשמר");
                $.LoadDefMachine(ObjID);
            }

        });
    };


    /*****************         UI FUNCTIONS          ******************/

    

    $(".DefMacDiv").live("click", function () {
        var MachineID = this.getAttribute("data-id");
        $.LoadDefMachine(MachineID);
    });

    $("#ClientID").live("change", function () {
        $.LoadClientMac($(this).val());
    });

    $("input[type=checkbox]").live("click", function () {
        var val = $(this).val();
       
        $(".marker[data-id=" + val + "]").toggle();
        $.UpdatePointShow(val, this.checked);
    });

});








