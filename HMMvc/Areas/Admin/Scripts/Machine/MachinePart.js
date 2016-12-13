

/*****************         MACHINE PART JS          ******************/



$(document).ready(function () {


    var ops = {
        entity: "MachinePart",
        cntrl: "/Admin/MachinePart",
        ForeignID: $("#MachineID").val(),
        success: function () {

            GetPartList();
            closePopup("popUpdateMachinePart");
            macHasParts = true;
        },

    }


    /** dont validate when opening machone for new job*/
    if ($("#popUpdateMachine").is("*")) {
        ops.container = "#popUpdateMachine";
        ops.dontValidate = true;
    }
    else {
        ops.container = "[data-page='/Admin/Machine/Update']";
    }
        

    function GetPartList(MachineID) {
        rest.get({
            url: ops.cntrl + "/Index",
            htmlSuccessTo: "#partList",
            data: {
                "MachineID": $("#MachineID").val(),
            },
        });

    }

    //$.showRow = function ($row, srchText) {
    //    var srchTerm = $row.text().toLowerCase().indexOf(srchText.toLowerCase()) > -1;

    //    return srchTerm;

    //};

   
    crud(ops);





    /*****************         UI FUNCTIONS          ******************/



    //$.openUpdateMachiePart = function myfunction() {
    //    $.OpenGeneralPop('Update', ops.cntrl, {}, true);
    //};

    function openUpdatePart(MachineID, partID) {
        var params = { "id": partID, "ForeignID": MachineID }
        $.OpenGeneralPop("Update" + ops.entity, null, params, false, ops.cntrl + "/Update");
    }

    //only adds machine type - then open tech details edit
    var addMachinePart = function (MachineID, MachineTypeID) {
        $.ajax({
            type: "Post",
            url: ops.cntrl + "/Add",
            data: {
                "MachineID": MachineID,
                "MachineTypeID": MachineTypeID,
            },
            success: function (data) {
                ops.success(data);

                //openUpdatePart(MachineID, data.MachinePartID);
                $("#UpdateMachineBtn").removeAttr("disabled");
                //if (data.msg == undefined)
                //    location.replace("/Admin/Refubrish");
                //else
                //    $.BugAlert(data.msg);
            },
            //fail: function (data) { },
        });

    };


    $.addMachinePart = function (macID, MachineTypeID) {
        
        addMachinePart(macID, MachineTypeID);
    }





});
