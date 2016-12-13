$(document).ready(function () {

    var listId = "#taskFieldsList";

    var ops = {
        entity: "BankTask_Field",
        cntrl: "/Admin/BankTask_Field",
        container: listId
    };

    crudOps = crud(ops);

    function add(model, done) {

        rest.update(
            $.extend({
                data: model,
                success: function (data) {
                   
                    $(listId).append(data);

                    CreateDropDown();

                    //success(data)
                    if (done) {
                        done();
                    }

                    restAddField();
                }
            }, ops)
        );
    }


    function restAddField() {
        $.setAutoCompleteVal("#AddFieldDrop");
    }

    function getLastInOrder() {
        return $(listId + ' .field').length + 1;
    }

    /*****************         UI FUNCTIONS          ******************/

    $("#AddFieldDrop").change(function () {

        add({
            BankTaskID: $("#BankTaskID").val(),
            BankFieldID: $(this).val(),
            OrderVal:getLastInOrder()
        });
        
    });

});
