
$(document).ready(function () {

    crud({
        entity: "Contact",
        cntrl: "/Admin/Contact"
    });



    ///*****************         UI FUNCTIONS          ******************/
  
    $("#userClient #t li a").live("click", function () {
        var ClientID = $(this).attr("data-id");
        $("#ClientID").val(ClientID).valid();

        $.LoadContacts(ClientID, "#C");
        //$.LoadClientUnQuotedJobs(ClientID);
    });


   
});








