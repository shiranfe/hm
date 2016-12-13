

/*****************         MACHINE JS          ******************/

$(document).ready(function () {
    var form = "#UpdateMachineForm";
    var ctrl = "/Admin/Machine";

    crud({
        entity: "Machine",
        cntrl: "/Admin/Machine",
        onlyUpdate:true,
        container: form,
        success: function (data) {
            $("#macPartsDiv").show();
            $("#MachineID").val(data.MachineID);

            //no parts for machine - show error and dont close machine edit form
            if (!macHasParts) {

                $("#noPartsInfo").show();
                $("#UpdateMachineBtn").attr("disabled", "disabled");
                return;
            }

            //updated or added machine in popup
            if ($("#popUpdateMachine").is("*")) {
                closePopup("popUpdateMachine");
                if ($("#UpdateJobForm").is("*")) {
                    var clientID = $("#UpdateJobForm .JobClientDrop #ClientID").val();
                    $.LoadClientMachine(clientID, data.MachineID);
                }
                if ($("#UpdateQuoteVersionForm").is("*")) {
                    var clientID = $("#ClientID").val();
                    //need to load side menu...
                }

                return;
            }

            //is machine page edit
            $.SuccessAlert("העדכון בוצע בהצלחה");
        }
    });


    //$.UpdateMachine = function () {
        
    //    $.ajax({
    //        type: "Post",
    //        url: ctrl+ "/Update",
    //        data: $(form).serialize(),
    //        success: function (data) {
               
                  
                
    //        },
    //        //fail: function (data) { },
    //    });

        
    //};
       
  

    //console.log("AddPartBtn");
    $("#AddPartBtn").click( function () {
        var macID = $("#MachineID").val();
        var MachineTypeID = $("#MachineTypeID").val();

      //  console.log("AddPartBtn");
        $.addMachinePart(macID, MachineTypeID);
    });
});
