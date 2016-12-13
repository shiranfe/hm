
var machinesIndex = true;
$(document).ready(function () {

   
    var crudOps = crud({
        entity: "Machine",
        cntrl: "/Admin/Machine",
        ForeignID: $("#ClientID").val(),
        indexFilters: [["ClientID", "0"]]
        
    });


    /*****************         UI FUNCTIONS          ******************/


    $(".machineIndexDiv .JobClientDrop #t li a").click(function () {
        var clientID = $(this).attr("data-id");
      //  crudOps.changeForeignId(clientID);
        $("#ClientID").val(clientID);
    });

    $("#MergeMachinesBtn").click( function () {
        $.OpenGeneralPop("MergeMachines", "Admin/Machine", { ClientId: $("#ClientID").val() }, true);
    });

});










