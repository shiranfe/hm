$(document).ready(function () {

    var _then;
 
    crud({
        entity: "BankTask",
        cntrl: "/Admin/BankTask",
        onlyUpdate: true,
        success: function (data) {
            if ($("#BankTaskID").val()!=0)
                return location.href = "/Admin/#/BankTask";


            $("#BankTaskID").val(data.BankTaskID);
            _then(data.BankTaskID);
            _then = null;

        }
    });

   

    /*****************         UI FUNCTIONS          ******************/

    $.updateBankTask = function (then) {
        _then = then;
        $("#UpdateBankTaskForm .ForBtnDiv [type=button]").click();
    };


});
