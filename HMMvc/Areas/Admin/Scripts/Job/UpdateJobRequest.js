//$.validator.setDefaults({
//    ignore: [],

//});


$(document).ready(function () {

    var form = "#UpdateJobRequestForm";
    $(form).validate().settings.ignore = "";

    function UpdateJob(data) {
       rest.post({
            url: "/Admin/Job/JobRequest",
            data: data,
            btn:"#UpdateJobBtn",
            //showSuccessNotification: true,
            success: function (data) {
                if (data.msg) {
                    return $.BugAlert(data.msg);
                }
               
                $.SuccessAlert(null, {
                    content: {
                        text: '<h1>מספר מכונה ' + $("#RefubrishDetailsDM_MachineID").val() + '</h1>',
                        title: "העבודה נוצרה בהצלחה"
                    },
                    hide: {
                        event: 'unfocus'
                    },
                    style: {
                        classes: "qtip-green",
                    },
                    events: true
                });

                  //  '<p>העבודה נוצרה בהצלחה</p><h1>מספר מכונה ' + $("#RefubrishDetailsDM_MachineID").val() + '</h1>');
                /** keep diabled to prevent double inserting*/
                $("#UpdateJobBtn").prop('disabled', true);

                //var jobType = getUrlParamValue("jobType");
                if (data.FirstPartID)
                    return location.href = "/Admin/#/Refubrish/" + data.FirstPartID;

                location.href = "/Admin/#/Refubrish";
               

            },
            //fail: function (data) { },
        });

    }


    function loadClientMachine(clientID,macId) {
        $.LoadClientMachine(clientID, macId, true);
    }

    /** when client created will hit the "a.click();" in addNewClientToTree*/
    function clientSelected(clientID) {
        $("#ClientID").val(clientID).valid();
       
      //  $.LoadContacts(clientID, "#ContactID");
        loadClientMachine(clientID)
       // $(".plusBtn.EditClientBtn").attr("data-id", clientID);
    }

    $.clientUpdated = function (clientID) {
        clientSelected(clientID);
    };



    function loadMachineParts(macID) {
        var name = '[name="RefubrishDetailsDM.MachineTypeID"]';
        $(name).parent().removeClass('exist');

        if (macID == -1)
            return;

        rest.get({
            url: "/Admin/Job/RequestMachineParts",
            data: {
                MachineID: macID
            },
            success: function (data) {
               
                for (var i = 0; i < data.typeIds.length; i++) {
                    $(name + '[value=' + data.typeIds[i] + ']').parent().addClass('exist');

                }
            }
        });

      

    }

    function addQuickMachine(macId,clientID, newName) {
        rest.update(
               {
                   entity: "Machine",
                   cntrl: "/Admin/Machine",
                   data: {
                       "MachineID":macId,
                       "ClientID": clientID,
                       "MachineName": newName
                   },
                   success: function (data) {
                       $("#RefubrishDetailsDM_MachineID").val(data.MachineID);
                       loadClientMachine(clientID, data.MachineID);
                       //loadMachineParts(data.MachineID);
                   }
               }
            );
    }

    /*****************         UI FUNCTIONS          ******************/
    $("#StartDate").datepicker({
        defaultDate: 0,
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        dateFormat: "dd/mm/yy",
        onClose: function (selectedDate) {
            $("#EndDate").datepicker("option", "minDate", selectedDate);
            $("#DueDate").datepicker("option", "minDate", selectedDate);
        }
    });

    $("#DueDate").datepicker({
        //defaultDate: 0,
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        dateFormat: "dd/mm/yy",
        //onClose: function (selectedDate) {
        //    $("#StartDate").datepicker("option", "maxDate", selectedDate);
        //}
    });


    function bindClientClicked(elem) {
        $(elem).click(function () {
            var clientID = $(this).attr("data-id");
            if (clientID <= 0)
                return;

            clientSelected(clientID);
        });
    }


    bindClientClicked(form + " .JobClientDrop #t li a");

    /** need to bind on client in drop created*/
    $.bindClientClick = function (a) {
        bindClientClicked(a);

    };

   
    function hasPartsF() {
        if (jobType == "Outside")
            return true;

        $("#partsValidation").hide();

        var hasParts = $("#PartsDIv li input:checked").length > 0;
        if (!hasParts)
            $("#partsValidation").show();

        return hasParts;
    }
  

    function stringfyPlaceHolder(field) {
        return $(field).attr("placeholder").replace(" ", "+");
    }

    $("#UpdateJobBtn").click( function (e) {
        
        /** if is hidden wouldnot show merror message*/
        $(idName).valid();

        var hasParts = hasPartsF();
      
        if ($(form).valid() && hasParts) {
            var data = $(form).serialize();
            data += "&ClientName=" + stringfyPlaceHolder("#srchClient") +
                    "&RefubrishDetailsDM.MachineName=" + stringfyPlaceHolder(".machineSelect .text");

            //console.log(data);
            UpdateJob(data);
        }
            

    });

    $("#RefubrishDetailsDM_MachineID").change(function (e) {
        setTimeout(function myfunction() {
            $("#Rpm").val($(".machineSelect [selected]").data("rpm"));
            $("#Kw").val($(".machineSelect [selected]").data("kw"));
            $("#Address").val($(".machineSelect [selected]").data("address"));
        },100);
    
    });


    $("#PartsDIv li").click(function (e) {

        $(this).toggleClass("selected");
        $(this).find("input").each(function () { this.checked = !this.checked; });
        hasPartsF();
    });


    $(".machineSelect select,.machineSelect [type=hidden]").change(function (e) {
        loadMachineParts(parseInt( $(this).val() || 0));
    });


    $(".addMachine").click(function () {
        var clientID = $("#ClientID").val();
        if (!clientID)
            return alert("יש לבחור לקוח תחילה");

        /** so the placeholder will be updated already*/
        setTimeout(function () {
            var newName = $("#RefubrishDetailsDM_MachineID").next().attr("placeholder");

            // console.log(newName);
            addQuickMachine(-2, clientID, newName);

        }, 200);
      
   
         
     
    });


});








