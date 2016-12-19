$(document).ready(function () {

 
    var headerActiosClass = ".VbTabBlockHeader .HeaderActions";
    var container = "#fieldList";
    var selectedTag = "selected";
    var selectedClass = "." + selectedTag;
    var containerSelected = container + " " + selectedClass;

    var fieldClass = ".field";
    var fieldsDiv = container + " " + fieldClass;
    var fieldsSelected = fieldClass + selectedClass;
    var fieldsDivSelected = container + " " + fieldsSelected;
    var fieldsDivTitle = fieldsDiv; 

    var ops = {
        entity: TaskFieldEntity,
        cntrl: "/Admin/" + TaskFieldEntity,
        container: container
    };

    crudOps = crud(ops);

  
    function add(model, done) {

        rest.update(
            $.extend({}, ops, {
                data: model,
                success: function (data) {

                    if (done)
                        return done();

                    //mciApp.refresh();
                    $(container).append(data);

                    CreateDropDown();

                    //success(data)
                    initNewItems()

                    restAddField();
                }
            })
        );
    }

    function deleteItems( ids) {

        rest.remove(
            $.extend({}, ops, {
                data: {
                    "ids": ids,
                },
                traditional: true,
                success: function (data) {
                   
                    removeFieldsForView();
                   setSortable();
                },
                fail: function () {
                    $.BugAlert("לא ניתן למחוק אובייקט אשר מידע מקושר אליו");
                }
            })
        );

    }

    function sortItems(ids) {

        rest.sort(
            $.extend({}, ops, {
                data: {
                    "ids": ids,
                },
                traditional: true,
                success: function () {
                    //console.log("done");
                },
                //fail: function () {
                //    data.msg = (data.responseText.indexOf("<!DOCTYPE html>") > -1) ? data.statusText : data.responseText;
                //    console.log(data.msg);
                //},

            })
        );
    }

    /*****************         Field Edit Funcions          ******************/

    function restAddField() {
        $.setAutoCompleteVal("#AddFieldDrop");
    }

    function getLastInOrder() {
        return $(fieldsDiv).length + 1;
    }

    function toggleCopyBtn() {

        if ($(containerSelected).length === 0) {
          //  $(".move").hide();
            $("#DeleteBtn").hide();
        }
        else {
           // $(".move").show();
            $("#DeleteBtn").show();
        }

    }

    function GetDataRowID(that) {
        return $(that).find("input, select, textarea").not(".EditableSelect").attr("name").split('_')[0]; /** maybe us subgroup field*/
    }

    function getSelectedItems() {
        var selecedIds = [];
       $(containerSelected).map(function () {
           selecedIds.push(GetDataRowID(this));
        });

        return selecedIds;
    }


    function removeFieldsForView() {
        $(containerSelected).remove();

        toggleCopyBtn();
    }


    function setSortable() {
       
        $(container).sortable({
            update: function (event, ui) {
                var dataRow = ui.item[0];

                var fields = [];
                $(fieldsDiv).map(function () {
                    fields.push(GetDataRowID(this));
                });

                sortItems(fields);
            }
        });

    }


    function initNewItems() {
        bindFieldClick();
        setSortable();
    }

    /*****************         UI FUNCTIONS          ******************/

    $("#AddFieldDrop").change(function () {
        var params = {
            BankTaskID: $("#BankTaskID").val(),
            JobTaskID: $("#JobTaskID").val() ,
            BankFieldID: $(this).val(),
            OrderVal: getLastInOrder()
        };

        if (params.BankTaskID===0)
            return $.updateBankTask(function (id) {
                params.BankTaskID = id;
                add(params, function myfunction() {
                    replaceUrlParams({ id: id });
                });
            });

        add(params);
      
    });




    $("#DeleteBtn").live("click", function () {

        var fieldsSelcted=getSelectedItems();

        deleteItems( fieldsSelcted);

    });

    function bindFieldClick() {
        /** prevent double binding by using "has been sorted"*/

        $(fieldsDiv + ":not(.ui-sortable-handle), #fieldList .CheckZone:not(.ui-sortable-handle)").click(function () {
            $(this).toggleClass(selectedTag);
            toggleCopyBtn();
        });
    }
  

    initNewItems();
});
