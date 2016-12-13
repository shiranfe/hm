

$(document).ready(function () {

    var ops = {
        entity: "JobTask",
        cntrl: "/Admin/JobTask",
        success: function (data) {
            $("#JobTaskID").val(data.JobTaskID);
            /** wanted to add employee before task was created*/
            if ($("#EmployeeID").val()) {
               return  $("#AddTaskEmployee").click();
            }

            location.href = "/Admin/#/Outside";
        }
    }


   crud(ops);

  

    /*****************         UI FUNCTIONS          ******************/
    



});






