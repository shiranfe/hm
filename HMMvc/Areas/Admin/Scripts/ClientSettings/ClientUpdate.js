var clientID;
$(document).ready(function () {

    /** handle save client update (pop was open from general)*/
    crud({
        entity: "Client",
        cntrl: "/Admin/Client",
        onlyUpdate: true,
        container: "#popUpdateClient",
        success: function (model) {
            $.addNewClientToTree(model);
            ////$.ClientCreated(model);

            if ($("#popUpdateClient").is("*"))
                closePopup("popUpdateClient");


        }
    });

    crud({
        entity: "Contact",
        cntrl: "/Admin/Contact",
        container: "#popUpdateClient", /**items table container*/
        ForeignID: clientID,
        success: function (html) {

            $("#userDiv").html(html);

            try {
                closePopup("popUpdateContact");
            } catch (e) {

            }

        }
    });

    /** before can add Contact need to save client first and get id*/
   function addClient(params) {
       rest.update(
               {
                   entity: "Client",
                   cntrl: "/Admin/Client",
                   data: params,
                   success: function (client) {
                       if (clientID == client.ClientID)
                           return;

                       clientID = client.ClientID;
                       $(container).find("#ClientID").val(clientID);
                       getContact(clientID);
                   }
               }
            );
   }

   function getContact(clientID) {
        // var params = $.extend({ "id": id }, ops.data);
       var params = { "ForeignID": clientID }
       $.OpenGeneralPop("UpdateContact", null, params, false, "/Admin/Contact/Update");
    }



    ///*****************         UI FUNCTIONS          ******************/


    $("#AddContactBtn").click( function () {

        //clientID = $("#ClientID").val();
        if (clientID > 0) {
            getContact(clientID);
            return;
        }

        var form = "#UpdateClientForm";
        var params = $(form).serialize();
        // add item
        if ($(form).valid()) {
            addClient(params);
        }
       
    });

 
    $("#popUpdateClient .ForBtnDiv input").click(function () {
        if (isFunction($.clientUpdated))
            $.clientUpdated(clientID);

    });
});








