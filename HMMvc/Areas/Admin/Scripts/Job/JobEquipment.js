
$(document).ready(function () {

    var opsJobEquipment = {
        entity: "JobEquipment",
        cntrl: "/Admin/JobEquipment",

        success: function (data) {
            //  console.log(data);
            CreateToDom(data);
            /** reset form*/
            //$.setAutoCompleteVal("#MachineTypeID");
            $("#EquipmentTitle,#Rpm,#Kw,#Hp,#Address").val(null);
           // $(btn).prop('disabled', false);
        }
    };

   // crud(opsJobEquipment);


    function Create(params,btn) {
      
        rest.add(
            $.extend({}, opsJobEquipment, {
                btn:btn,
                data: params,
                showSuccessNotification:true,
                success: function (data) {
                  //  console.log(data);
                    CreateToDom(data);
                    /** reset form*/
                    //$.setAutoCompleteVal("#MachineTypeID");
                    $("#EquipmentTitle,#Rpm,#Kw,#Hp,#Address").val(null);
                }
            })
        );
    }



    function Remove(id) {
        rest.remove(
            $.extend({}, opsJobEquipment, {
                data: {
                    id: id
                },
                success: function () {
                    $('[data-id="' + id + '"').remove();
                }
            })
        );
    }


    function CreateToDom(data) {

        var template = '<tr class="even" data-id="' + data.JobEquipmentID + '">' +
                    '<td>' + data.EquipmentName + '</td>' +
                    '<td>' + $.getAutoCompleteText("#MachineTypeID") + '</td>' +
                    '<td style="text-align:center"><span class="linktext deleteItem">מחק</span> </td>' +
                '</tr>';


        $("#equip-items").append(template);
    }

  


    /*****************         UI FUNCTIONS          ******************/
   


    $("#AddJobEquipment").click(function () {

        if (!$("#MachineTypeID").valid())
            return;

        /** *dont add twice...*/
        var newItem = {
            //EquipmentID: $("#EquipmentID").val(),
            //EquipmentIdentifier: $("#EquipmentIdentifier").val(),
            "EquipmentDM.EquipmentTitle": $("#EquipmentTitle").val(),
            "EquipmentDM.MachineTypeID": $("#MachineTypeID").val(),
            "EquipmentDM.Rpm": $("#Rpm").val(),
            "EquipmentDM.Kw": $("#Kw").val(),
            "EquipmentDM.Address": $("#Address").val(),
            "EquipmentDM.ClientID": $("#ClientID").val(),

            "JobID": $("#JobID").text()
        }

      

        Create(newItem, "#AddJobEquipment");
       // console.log(newItem);

    });

    $("#equip-items .deleteItem").live('click', function () {
        var id = $(this).parents("tr").attr("data-id");
        Remove(id);
    });

    $(".equip-menu span").live('click', function () {
        $(".equip-menu span").toggleClass("selected");
        $(".equip-div").toggle();
    });

});