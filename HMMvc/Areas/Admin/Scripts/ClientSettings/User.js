
$(document).ready(function () {

    crud({
        entity: "User",
        cntrl: "/Admin/User"
    });

    //$.OpenUpdateUser = function (UserID) {
    //    $.OpenGeneralPop("UpdateUser", "Admin/ClientSettings", { "UserID": UserID }, false);
    //};

    //$.DeleteUser = function (UserID) {
    //    // $("#PointSelectedDiv").html(ajax_load);
    //    var pg = "/Admin/ClientSettings/DeleteUser";
    //    $.post(pg, {
    //        "UserID": UserID,
    //    }, function (data) {
    //        location.reload();
    //    }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
       
    //};


    ///*****************         UI FUNCTIONS          ******************/
  
    $("#userClient #t li a").live("click", function () {
        var ClientID = $(this).attr("data-id");
        $("#ClientID").val(ClientID).valid();

        $.LoadContacts(ClientID, "#C");
        //$.LoadClientUnQuotedJobs(ClientID);
    });

    //$("#AddContactBtn").live("click", function () {
    //    $.OpenUpdateUser();
    //});

  
    //$(".username").live("click", function () {
    //    var id = $(this).attr("data-id");
    //    $.OpenUpdateUser(id);
    //});

    //$(".deleteUser").live("click", function () {
    //    if (confirm("האם אתה בטוח שיהנך מעוניים למחוק את המשתמש?")) {
    //        var id = $(this).attr("data-id");
    //        $.DeleteUser(id);
          
    //    }
    //});

   
});








