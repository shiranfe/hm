var stepToLoad;

function addStepImage(src) {
    $(".step-pics").prepend('<div class="MachineImg s1 pic-container"><img src="' + src + '"/> <div class="delete ">X</div></div>');

}

$(document).ready(function () {

    var container = "#RefubrishStepDiv";

    function loadStepPics(jobId, step, i) {
        /** can have step1, step2, step4, step7 ... - cecause deleted.
        so check up to 15, to be sure no miss
        */


        i = (i || 0) + 1;
        if (i > 15)
            return;

        var img = new Image();

        img.onload = function () {
            addStepImage(img.src);

            loadStepPics(jobId, step, i);
        }

        img.onerror = function () {
            loadStepPics(jobId, step, i);
        }

        img.src = "/Pics/Job/" + jobId + "/" + step + i + ".jpg";



    }

    

    //$.OnComplete = function (data) {

    //    if (data.msg == undefined)
    //        location.reload();
    //    else
    //        $.BugAlert(data.msg);
    //};


    function currentStepId() {
        return $(".Step.selected").attr("data-id");
    }

    function nextStep() {

    
        rest.update(
              {
                  entity: "Refubrish",
                  cntrl: "/Admin/Refubrish/NextStep",
                  data: $("#UpdateStepForm").serialize(),
                  btn: ".ForBtnDiv input",
                  success: function (data) {

                      if (data.QuoteID)
                          return location.href = "/Admin/#/Quote/Get/" + data.QuoteID;

                      if (data.Closed)
                          return location.href = "/Admin/#/Refubrish";

                      //if ($("#StepsDiv .Step").last().attr("data-id") != currentStepId()) {
                      //    $.SuccessAlert("השמירה בוצעה בהצלחה");
                      //    return;
                      //}

                      ///** refresh this step - refresh only content*/
                      //if (data.NextStep)
                      //    return $.LoadStep(data.NextStep);

                      /** else move to next step - need to reload side menu also*/
                      //location.reload();

                      //partialLoad(true, null, function () {
                      //    var selected = currentStepId();
                      //    $.LoadStep(selected);
                      //    //console.log(selected);
                      //});

                      /** reload with menu*/
                      location.href = "/Admin/#/Refubrish/" + data.JobRefubrishPartID;

                  }
              }
           );

        

    }

    function updateStep(refreshPage) {

        /** if need to update specific field, dont send all of them...*/

        rest.update(
              {
                  entity: "Refubrish",
                  cntrl: "/Admin/Refubrish/UpdateStep",
                  data:  $("#UpdateStepForm").serialize(),
                  success: function (data) {

                      /** changed speed (771)*/
                      if (refreshPage)
                          return $.LoadStep(refreshPage,true);

                  }
              }
           );

    }

    //$.GetQuat = function () {
    //    $.post('/Admin/Refubrish/GetQuat', {
    //        "JobID": $('#JobID').text(),
    //        "JobRefubrish_StepID": $('#JobRefubrish_StepID').val(),
    //        "JobRefubrishStepID": $('#JobRefubrishStepID').val(),
    //        "Notes": $('#Notes').val(),
    //    }, function (data) {
    //        location.reload();
    //    });

    //};


    //$.EngineBurned = function () {
    //    $.post('/Admin/Refubrish/EngineBurned', {
    //        "JobID": $('#JobID').text(),
    //        "JobRefubrish_StepID": $('#JobRefubrish_StepID').val(),
    //        "JobRefubrishStepID": $('#JobRefubrishStepID').val(),
    //        "Notes": $('#Notes').val(),
    //    }, function (data) {
    //        location.reload();
    //    }).fail(function () { $.BugAlert("העדכון לא בוצע"); });
      
    //};

    function fieldChanged() {
        setTimeout(function () {
            updateStep();
        }, 100);
    }

  
    /*****************         UI FUNCTIONS          ******************/

   

   

    $("#UpdateStepBtn").live("click", function (e) {
        if ($("#UpdateStepForm").valid())
            nextStep();
        else
            scrollToError();
    });

    $("#ErrorBtn").live("click", function () {
        $("#NextStep").val($(this).attr("data-errornextstep"));
       // $("#RefubrishStepDiv form").validate().cancelSubmit = true;
        // $("#RefubrishStepDiv form").submit();
        nextStep();
    });

    $("#jobRejectedBtn").live("click", function () {
        $("#NextStep").val("Cancelled");
        nextStep();
    });


    $(".selctee.started").live("click", function () {
        $(".Step").addClass("selctee");
        $(this).removeClass("selctee");
        var step = $(this).attr("data-id");
        $.LoadStep(step);
    });

    $("#JobDM_ClientID").live("change", function () {
        $("#MachineID option").remove();

        $.get("/Admin/Machine/GetClientMachines", {
            "ClientID": $(this).val(),
        }, function (data) {

            $(data).each(function () {
                $("#MachineID").append(new Option(this.Text, this.Value));
            });
            if (data[0] == undefined) {
                $("#MachineID").parent().find(".SelectVal").text("");
            }
            else {
                $("#MachineID").prop("selectedIndex", 0);
                $("#MachineID").parent().find(".SelectVal").text(data[0].Text);
            }

        });
    });

    $("#Parts").live("change", function () {
        var partID = $(this).val();
        location.href = "/Admin/#/Refubrish/" + partID;
    });

    $(container + " .CheckZone:not(.spinner)").live("click", function () {
        var hidden = $(this).find("input");
        $(this).toggleClass("selected");
        hidden.val($(this).hasClass("selected"));
      
    });

    $(container + " .CheckZone.spinner:not(.butn)").live("click", function (e) {

        if (e.target.classList[0] == "butn")
            return;
        var that = $(this);
        $(that).toggleClass("selected");
        var val = $(that).hasClass("selected") ? 1 : 0;
        $(that).find("input").val(val);
        if (val == 0)
            val = "";
        $(that).find(".label").text(val);

        fieldChanged();
    });

    $(container + " .spinner > .butn").live("click", function () {
        var $div = $(this.parentNode);
        var textbox = $div.find("input");
        var curVal = textbox.val();
        curVal = parseInt(curVal) + ($(this).hasClass("plus") ? 1 : -1);
        if (isNaN(curVal) || curVal < 0) curVal = 0;

        textbox.val(curVal);

        if (curVal == 0) {
            $div.removeClass("selected");
            curVal = "";
        }
        else
            $div.addClass("selected");


        $div.find(".label").text(curVal);
        fieldChanged();
    });


 


    /** motor AC speed - 1/2/3.
        reload to show sub groups accordenly
    */
    $(container + " [name=771]").live("change", function () {
        var id = $(this).val();
       
        updateStep("DetailsStep");//"&StayInStep=DetailsStep"

    //    console.log(id);
       // location.href ="Refubrish?JobRefubrishPartID=" + partID;
    });






    /** MONITOR FIELD CHANGES*/


    $(container + " .SwitchBtn").live("click", function () {
        fieldChanged();
    });
   
    $(container + " .SwitchDivInner").live("swipe", function () {
        fieldChanged();
    });

    $(container + " [name]").live("change", function () {
        fieldChanged();
    });

 $(container + " select").live("change", function () {
        fieldChanged();
    });

});


