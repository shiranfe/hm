
$(document).ready(function () {




    jss["MachineVBTab"] = true;



    function getHtml(action,param, container) {
  
        $.get("/VBMachine/" + action, param,
            function (data) {
                $(container).html(data);
            })
            .fail(function () {
                $.BugAlert();
            });
    }

    function updateData(action, param) {

        rest.update({
            entity: "VBMachine",
            cntrl: "/VBMachine",
            data: param
        }
      );


    }

    $.LoadPointValue = function (PointResultID) {
        console.log("LoadPointValue");
        // $("#PointSelectedDiv").html(ajax_load);
        getHtml("MachineVBTabPointVal", {
            "PointResultID": PointResultID,
            "StartDate": $("#StartDate").val(),
            "EndDate": $("#EndDate").val()
        }, "#VbTabPointVal");


    };
    

    $.LoadMachineVBTab = function (JobID) {
        getHtml("MachineVBTab", {
            "MachineID": MachineID,
            "JobID": JobID
        }, "#ui-tabs-2");

    };


    //$.ToggleClientNotesEdit = function () {
    //    $("#EditClientNoteDiv").toggle();
    //    $("#ClientNoteText").toggle();
    //    $("#EditClientNotesBtn").toggle();
    //};

    //$.ToggleSKUEdit = function () {
    //    $("#EditSKUDiv").toggle();
    //    $("#SKUText").toggle();
    //    $("#EditSKUBtn").toggle();
    //};

    //$.ToggleCommentsEdit = function () {
    //    $("#EditCommentsDiv").toggle();
    //    $("#CommentsText").toggle();
    //    $("#EditCommentsBtn").toggle();
    //};

    //$.ToggleDetailsEdit = function () {
    //    $("#EditDetailsDiv").toggle();
    //    $("#DetailsText").toggle();
    //    $("#EditDetailsBtn").toggle();
    //};

   

    $.ToggleMacPointsPointListDiv = function (element) {
        $(".MacPointsPointListDiv.selected").toggleClass("selected");
        $(element).toggleClass("selected");
    };


    function editClientNotes() {
   
        var ClientNotes = $("#ClientNoteTextBox").val();

        updateData("ClientNotes", {
            "MachineID": MachineID,
            "ClientNotes": ClientNotes,
            "JobID": JobID
        }, function (data) {
            //$("#ClientNoteText")[0].innerHTML = ClientNotes;
            //$.ToggleClientNotesEdit();
        });

    }

    $.EditSKU = function () {
        var SKU = $("#SKUTextBox").val();

        updateData("SKU",  {
            "MachineID": MachineID,
            "SKU": SKU
        }, function (data) {
            $("#ClientNoteText")[0].innerHTML = ClientNotes;
            $.ToggleClientNotesEdit();
        });

    };

    $.EditDetails = function () {
        var Details = $("#DetailsTextBox").val();

        updateData("Details", {
            "MachineID": MachineID,
            "Details": Details
        }, function (data) {
            $("#DetailsText")[0].innerHTML = Details;
            $.ToggleDetailsEdit();

        });

    };

    $.EditComments = function () {
        var Comments = $("#CommentsTextBox").val();
        updateData("Comments",{
            "MachineID": MachineID,
            "Comments": Comments
        }, function (data) {
            $("#CommentsText")[0].innerHTML = Comments;
            $.ToggleCommentsEdit();

        });

    };

 


    $.LoadPointDetails = function (MachinePointID) {
        // $("#PointSelectedDiv").html(ajax_load);
       // $.Scroll("#VbTabMacPoints");
        var pg = "/VBMachine/PointDetails";
        $.post(pg, {
            "MachinePointID": MachinePointID,
            "JobID": JobID
        }, function (data) {
            $("#PointSelectedDiv").html(data);

        }).fail(function () { $.BugAlert(); });
    };

  

    $.Scroll = function (trgt) {
        //tabs
        //window.scrollTo(10, 175);
        //$(".VbTabInner").scrollTo($(trgt), 700);

    };

    $.ToggleVbTabNav = function (tab) {
        var trgt = "#" + tab.getAttribute("data-trgt");
        $(".Tab.Selected").removeClass("Selected").animate({ right: 0, left: 0 }, 350);

        $(tab).addClass("Selected").animate({}, 350);
        $.Scroll(trgt);

    };
    //$.plot($("#placeholder"), data, options);
    //  $.plot($("#placeholder"), [[[0, 0], [1, 1]]], { yaxis: { max: 1 } });


    var data = [];
    function onDataReceived(series) {

        // Extract the first coordinate pair; jQuery has parsed it, so
        // the data is now just an ordinary JavaScript object
        var options = {
            lines: {
                show: true
            },
            points: {
                show: true
            },
            xaxis: {
                tickDecimals: 0,
                tickSize: 1
            }
        };
        data.push(series);

        $.plot($("#placeholder"), data, options);
    }

    /* $.ajax({
         url: "/VBMachine/GetChart",
         type: "POST",
         dataType: "json",
         success: onDataReceived
     });*/


    /*****************         UI FUNCTIONS          ******************/



    $("#ClientNoteTextBox").blur( function () {
        editClientNotes();
    });

    //$(".ToggleClientNotesBtn").live("click", function () {
    //    $("#ClientNoteTextBox")[0].value = $("#ClientNoteText")[0].innerHTML;
    //    $.ToggleClientNotesEdit();
    //});

    //$("#SaveSKUBtn").live("click", function () {
    //    $.EditSKU();
    //});

    //$(".ToggleSKUBtn").live("click", function () {
    //    $("#SKUTextBox")[0].value = $("#SKUText")[0].innerHTML;
    //    $.ToggleSKUEdit();
    //});

    //$("#SaveDetailsBtn").live("click", function () {
    //    $.EditDetails();
    //});

    //$("#SaveCommentsBtn").live("click", function () {
    //    $.EditComments();
    //});

    //$(".ToggleCommentsBtn").live("click", function () {
    //    $("#CommentsTextBox")[0].value = $("#CommentsText")[0].innerHTML;
    //    $.ToggleCommentsEdit();
    //});

    //$(".ToggleDetailsBtn").live("click", function () {
    //    $("#DetailsTextBox")[0].value = $("#DetailsText")[0].innerHTML;
    //    $.ToggleDetailsEdit();
    //});

    $(".MacPointsPointListDiv a").click( function () {
      
        $.ToggleMacPointsPointListDiv($(this).parent());
        //$.LoadPointDetails(this.getAttribute("data-id"));
    });

    $(".marker").click( function () {
        var MachinePointID = this.getAttribute("data-id");
        var pointdiv = $(".MacPointsPointListDiv[data-id=" + MachinePointID + "]")
        $.ToggleMacPointsPointListDiv(pointdiv);
        $.LoadPointDetails(MachinePointID);
    });

    //$(".Tab").live("click", function () {

    //    $.ToggleVbTabNav(this);

    //});

    //$(".Tab:not(.Selected)").live({
    //    mouseenter: function () {
    //        $(this).animate({ right: -8, left: -8 }, 300);
    //    },
    //    mouseleave: function () {
    //        $(this).animate({ right: 0, left: 0 }, 300);
    //    }
    //});



    

    //$("#PointSortBy").on("change", function () {
    //    $("#PointSortZone").jSort({
    //        sort_by: "PointListDivTitle",//$(this).attr('value'),
    //        item: "div",
    //        order: "desc"
    //    });

    //});

    //$(".selectpicker").selectpicker({
    //    style: "btn"
    //});

    $("#DateCombo").change( function () {
     
        location.href = "/#/VBMachine/MachinePage?JobID=" + $(this).val() + "&MachineID=" + MachineID;


      //  $.LoadMachineVBTab($(this).val());

    });


    


  


});








