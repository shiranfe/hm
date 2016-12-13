
$(document).ready(function () {

    $.LoadPointResualt = function (PointResualtID) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = "/Admin/VB/PointResualtSelected";
        $.post(pg, {
            "PointResualtID": PointResualtID,
        }, function (data) {
            $("#PointResualtSelectedDiv").html(data);

        });
    };



    /*****************         UI FUNCTIONS          ******************/
    
    $("#SrchjobID").live("focusout", function () {
        var JobID = this.value;
        location.href ="/Admin/VB/PointResualt?JobID="+JobID;
    });

    $(".PointResualtDiv").live("click", function () {
        var id = this.getAttribute("data-id");
        $.LoadPointResualt(id);
    });



});








