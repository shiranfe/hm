
$(document).ready(function () {

    var opsGroup = {
        entity: "JobTaskEmployee",
        cntrl: "/Admin/JobTaskEmployee",
    };


    function addEmp(params) {
        rest.add(
            $.extend({}, opsGroup, {

                data: params,
                success: function (data) {
                    addEmpToDom(data.JobTaskEmployeeID);
                    /** reset form*/
                    $.setAutoCompleteVal("#EmployeeID");
                   // $("#VisitStart,#VisitEnd").val(null);
                }
            })
        );
    }


    function deleteEmp(id) {
        rest.remove(
            $.extend({}, opsGroup, {
                data: {
                    id:id
                },
                success: function () {
                    $('[data-id="' + id + '"').remove();
                }
            })
        );
    }


    function addEmpToDom(id) {

        var template = '<tr class="even" data-id="' + id + '">' +
                    '<td>' + $.getAutoCompleteText("#EmployeeID") + '</td>' +
                    '<td>' + $("#VisitStart").val() + '</td>' +
                   ' <td>' + $("#VisitEnd").val() + '</td>' +
                    '<td style="text-align:center"><span class="linktext delete">מחק</span> </td>' +
                '</tr>';


        $("#emp-items").append(template);
    }

    function tryAddEmp() {
        var newEmp = {
            EmployeeID: $("#EmployeeID").val(),
            VisitStart: $("#VisitStart").val(),
            VisitEnd: $("#VisitEnd").val(),
            JobTaskID: $("#JobTaskID").val()
        }

        if (!newEmp.EmployeeID || !newEmp.VisitStart || !newEmp.VisitEnd)
            return alert("יש למלא את כל השדות");

        addEmp(newEmp);

    }


    /*****************         UI FUNCTIONS          ******************/
    $("#VisitStart").datetimepicker({
        allowTimes: true,
        format: 'd/m/Y H:i',
        // value: '0',
        //onShow:function( ct ){
        //    this.setOptions({
        //        maxDate: $("#VisitEnd").val() || false
        //    })
        //},
        onChangeDateTime: function (ct) {
            // if (!$("#VisitEnd").val() || $("#VisitEnd").val() < $("#VisitStart").val()) {
            var end = moment(ct).add(2, 'hours').format("DD/MM/YYYY HH:mm");

            $("#VisitEnd").val(end);

            // }
        }

        //onClose: function (selectedDate) {
        //    $("#VisitEnd").datepicker("option", "minDate", selectedDate);
        //    $("#DueDate").datepicker("option", "minDate", selectedDate);
        //}
    });

    $("#VisitEnd").datetimepicker({
        allowTimes: true,
        format: 'd/m/Y H:i',
        // value: '0',
        //onShow:function( ct ){
        //    this.setOptions({
        //        minDate: $("#VisitStart").val() || false
        //    })
        //}
        //onClose: function (selectedDate) {
        //    $("#VisitStart").datepicker("option", "maxDate", selectedDate);
        //}
    });


    $("#AddTaskEmployee").click(function () {
        if (!$("#JobTaskID").val() || $("#JobTaskID").val()=="0")
            return $("#UpdateJobTaskBtn").click();

        tryAddEmp();
      

    });

    $("#emp-items .delete").live('click',function () {
        var id = $(this).parents("tr").attr("data-id");
        deleteEmp(id);
    });


});