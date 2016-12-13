
$(document).ready(function () {

    $.LoadClient = function (ClientID) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = '/AdminSettings/ClientLogoSelected';
        $.post(pg, {
            "ClientID": ClientID,
        }, function (data) {
            $("#ClientLogoSelectedDiv").html(data);

        });
    };



    /*****************         UI FUNCTIONS          ******************/

    

    $(".ClientDiv").live('click', function () {
        var ClientID = this.getAttribute('data-id');
        $.LoadClient(ClientID);
    });



});








