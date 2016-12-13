
$(document).ready(function () {

   
    crud({
        entity: "Client",
        cntrl: "/Admin/Client",
        indexFilters: true
    });

    /** prevent extra loading from user client drop*/
    jss["ClientDrop"] = true;


    $.LoadClient = function (clientId) {
        // $("#PointSelectedDiv").html(ajax_load);

        var pg = "/Admin/Client/ClientSelectedLogo";
        $.post(pg, {
            "ClientID": clientId,
        }, function (data) {
            //$("#ClientLogoSelectedDiv").html(data);
            refreshPage();
        });
    };

  


    function add(data) {
        rest.copy(
            $.extend({
                data: data,
                success: function (data) {
                    openVersion(data.NewVersionID);
                }
            }, ops)
        );
    }

    ///*****************         UI FUNCTIONS          ******************/

    
    $("#MergeClientBtn").click(function () {
        $.OpenGeneralPop("MergeClients", "Admin/Client");
    });

    
});








