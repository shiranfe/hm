
var machinesIndex = true;
$(document).ready(function () {

   
    var crudOps = crud({
        entity: "VB",
        cntrl: "/Admin/VB",
        ForeignID: $("#ClientID").val(),
        indexFilters: [["ClientID", "0"],["IsPosted", ""]]
       
    });


    /*****************         UI FUNCTIONS          ******************/


    $(".vbIndexDiv .JobClientDrop #t li a").click(function () {
        var clientID = $(this).attr("data-id");
        $("#ClientID").val(clientID);
    });


});










