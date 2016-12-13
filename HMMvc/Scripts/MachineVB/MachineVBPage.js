var MachineID, ActiveTab, JobID;
$(document).ready(function () {

    $("#tabs").tabs({
        active: ActiveTab,
        beforeLoad: function (event, ui) {
            ui.panel.html(ajax_load);
            ui.jqXHR.error(function () {
                ui.panel.html(
                  "Couldn't load this tab. Please contact Hashhmal-Motor. ");
            });
        }
    });

    /*****************         UI FUNCTIONS          ******************/


   
});








