$(document).ready(function () {


    function loadOptions(clientID,success) {
        rest.get({
            url: "/Admin/Contact/GetClientContacts",
            data: {
                id: clientID,
            },
            success: success
        });
    }

    

    
    $.LoadContacts = function (clientID, drop) {
     
        if (clientID < 0)
            return;

        var isAutoCpmlete = $(drop).is("[type=hidden]");

        if (isAutoCpmlete) {
            var cntrl = $(drop).parents(".filterBox");
            return $.setAutoCompleteOptions(cntrl, function (populateDataOnSuccess) {
                loadOptions(clientID, populateDataOnSuccess)
            });
        }
           

        $(drop).find("option").remove();
        $(drop).val("");
        $.updateSelectOutter($(drop));
        
        loadOptions(clientID, function (data) {
            $(drop).append("<option></option>");
            $.each(data, function (field, item) {
                var option = "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
                $(drop).append(option);
            });

            var userID = data[0] ? data[0].Value : "";//selected ||
            //if (selected) {
            //     macID = $("#RefubrishDetailsDM_MachineID option").filter(function () { return $(this).html() == "aaa"; }).val();
            //}

            $.changeDrop(drop, userID);
        });


    };
});