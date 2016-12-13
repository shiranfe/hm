$(document).ready(function () {
    var drop = ".machineSelect select";
    var autoComplete = ".machineSelect .autocomplete";

    function loadOptions(clientID, success) {
        rest.get({
            type: "Get",
            url: "/Admin/Machine/GetClientMachines",
            data: {
                ClientID: clientID,
            },
            success: success
        });
    }

    

    $.LoadClientMachine = function (clientID, selected,loadParts) {


        if (clientID < 0)
            return;

        var isAutoCpmlete = $(autoComplete).is("*");

        if (isAutoCpmlete) {
            var cntrl = $(autoComplete);
            return $.setAutoCompleteOptions(cntrl, function (populateDataOnSuccess) {
                loadOptions(clientID, populateDataOnSuccess)
            }, selected);
        }
            


         $(drop).find("option").remove();
        $(drop).val("");
        $.updateSelectOutter($(drop));

        loadOptions(clientID, function (data) {
            $(drop).append("<option></option>");
            $.each(data, function (field, item) {
                var option = "<option value=\"" + item.MachineID + "\">" + item.MachineName + "</option>";
                $(drop).append(option);
            });

            var macID = selected || "";
            //if (selected) {
            //     macID = $("#RefubrishDetailsDM_MachineID option").filter(function () { return $(this).html() == "aaa"; }).val();
            //}


            $.changeDrop(drop, macID);

            if (loadParts) {
                $.LoadMachineParts(macID);
            }


        });
       
    };

   
    
});