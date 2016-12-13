
var selectedTab;

$(document).ready(function () {
   

    function loadTab(tab) {

        tab = tab || "details";

        $(".Step[data-id="+tab+"]").addClass("selected");

        var url="/Admin/#/";
        switch (tab) {
            case "vb":
                url += "Vb/MachineJobs";
                break;
            case "refubrish":
                url += "Refubrish/MachineJobs";
                break;
            case "alignment":
                url += "Alignment/MachineJobs";
                break;
            //case "dynamic":
            //    url += "Machine/Update";
            //    break;
            case "details":
                url += "Machine/Update";
        }


        location.href = url + "?id=" + $("#MachineID").val() + "&tab=" + tab;
/*
        rest.get({
            url: url,
            htmlSuccessTo:"#machinePageContent",
            data: {
                "id": $("#MachineID").val()
            },
            success: function () {
                CreateDropDown();
            } 
        });

        */
    }

    /*****************         UI FUNCTIONS          ******************/

  

    

    //$("#Parts").live("change", function () {
    //    //var partID = $(this).val();
    //    //location.replace("Refubrish?JobRefubrishPartID=" + partID);
    //});

    $(".Step:not(.selected)").click( function () {
        var tab = $(this).attr("data-id");
     
        $(".Step").removeClass("selected");
      
        loadTab(tab);
    });


   
    loadTab(urlParam("tab"));

});
