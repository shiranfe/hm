//$.validator.setDefaults({
//    ignore: [],

//});


$(document).ready(function () {

    var initialMac = $("#MachineID").val(),
        form = "#UpdateJobForm";

    $(form).validate().settings.ignore = "";

    function loadMachineParts(macID) {
        
        if (!macID)
            return;

        var jobID = $("#JobID").val() == "null" ? 0 : $("#JobID").val();

        rest.get({
            url: "/Admin/Job/MachineParts",
            data: {
                MachineID: macID,
                JobID: jobID,
                jobType: jobType
            },
            success: function (html) {
                $("#PartsDIv").html(html);
            }
        });

    }


    loadMachineParts(getMachineId());

    function UpdateJob() {

        rest.post({
            url: "/Admin/Job/Details",
            btn: "#UpdateJobBtn",
            data: $(form).serialize(),
            success: function (data) {
                if (data.msg) {
                    return $.BugAlert(data.msg);
                }

                /** keep diabled to prevent double inserting*/
                $("#UpdateJobBtn").prop('disabled', true);

                //var jobType = getUrlParamValue("jobType");

                /** go to task assignment*/
                if (jobType == "Outside")
                    return location.href = "/Admin/#/JobTask/Update?JobID=" + data.JobID;

                location.href = "/Admin/#/" + jobType ;

            },
            //fail: function (data) { },
        });

    }


    function changeJobClient(jobId, clientId) {

        rest.update(
              {
                  entity: "Job",
                  cntrl: "/Admin/Job/ChangeJobClient",
                  btn: "#popMoveJob [type=button]",
                  data: {
                      JobID: jobId,
                      ClientID: clientId,
                  },
                  success: function (data) {
                      closePopup("popMoveJob");
                      refreshPage();
                  }
              }
           );
    }


    /** client drop has changed, update machine and contact*/
    function changeClient(clientID) {
        $("#ClientID").val(clientID).valid();

        $.LoadContacts(clientID, "#ContactID");
        $.LoadClientMachine(clientID, null, true);
        $(".plusBtn.EditClientBtn").attr("data-id", clientID);
    }


    /** create popMoveJob to decide if to move client macine and contact*/
    function popMoveJob(clientID) {
        var oldClientName = $("#srchClient").attr("placeholder");
        setTimeout(function () {
            var newClientName = $("#srchClient").attr("placeholder");
            var content = '<div><input type="hidden" id="destClientId" value="' + clientID + '"/><p>המכונה ו/או איש הקשר הרשומים בעבודה שייכים ללקוח <b>' + oldClientName + '</b>.</p><p>האם ברצונך להעביר את המכונה ואיש הקשר שנבחרו ללקוח <b>' + newClientName + '</b>? </p> <div class="ForBtnDiv clearfix"><input onclick="$.moveClient(true)" type="button" value="העבר ללקוח" class="buttondefault floatlast"><input onclick="$.moveClient(false)" type="button" value="אל תעביר" class="buttondefault red floatlast"></div></div>';
            $.createAndOpenPop("MoveJob", content, "שינוי לקוח");
        }, 100);
      
    }

    /** called from moveit popup buttons*/
    $.moveClient = function (moveIt) {
        var clientID = $("#destClientId").val();
        /** want to select new machine and contact*/
        if (!moveIt) {
            changeClient(clientID);
            return closePopup("popMoveJob");
        }

        changeJobClient($("#JobID").val(), clientID);
        
    };


    /** when client created will hit the "a.click();" in addNewClientToTree*/
    function clientSelected(clientID) {
        if ($("#ClientID").val()!=clientID && ($("#RefubrishDetailsDM_MachineID").val() || $("#ContactID").val()))
            popMoveJob(clientID);
        else
            changeClient(clientID);
       
    }

    $.clientUpdated = function (clientID) {
        clientSelected(clientID);
    };
    //$.ClientSelected = function (id) {
    //    clientSelected(clientID);
    //}

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

    $("#EndDate").datepicker({
        //defaultDate: 0,
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        dateFormat: "dd/mm/yy",
        onClose: function (selectedDate) {
            $("#StartDate").datepicker("option", "maxDate", selectedDate);
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

    $("#UpdateJobBtn").click(function (e) {

        /** if is hidden wouldnot show merror message*/
        if (idName && $(idName).is("*"))
            $(idName).valid();

        var hasParts = hasPartsF(); //equipMode || 

        if ($(form).valid() && hasParts) {

            UpdateJob();
        }


    });

    $("#PartsDIv li input").live("change", function (e) {
        hasPartsF();
    });



    $(".machineSelect select,.machineSelect [type=hidden]").change(function (e) {
        var macID = parseInt($(this).val() || 0);

        if (macID > 0)
            loadMachineParts(macID);
        else
            $("#PartsDIv").html('<span>נא לבחור מכונה</span>');

    });

    $(".AddMachineBtn").click( function () {
        var clientID = getClientId();
        var newName = $("#RefubrishDetailsDM_MachineID").next().val();
        if (clientID)
            $.OpenUpdateMachine({ "ForeignID": clientID, MachineName: newName });
        else
            alert("יש לבחור לקוח תחילה");

    });

    $(".editMachineJob").click(function () {

        var macId = getMachineId();
        if (!macId)
            return alert("יש לבחור מכונה בכדי לערוך אותה");

        $.OpenUpdateMachine({ "id": macId });
    });


    function getClientId() {
        return $(form + " .JobClientDrop #ClientID").val();
    }

    function getMachineId() {
      
        return $(idName).val();
    }

});








