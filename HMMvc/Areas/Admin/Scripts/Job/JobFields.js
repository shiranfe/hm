
var groupsSelcted = [];
var subGroupsSelcted = [];
var fieldsSelcted = [];
var fieldsGroupdId;
var subPid;
var IsCut = false;

$(document).ready(function () {
    var ctrl = "Admin/JobTaskField";
    var path = "/" + ctrl + "/";
    var opsField = {
        entity: "JobTaskField",
        cntrl: "/Admin/JobTaskField"
       // done: hideLoading
    };

    var opsGroup = {
        entity: "JobTaskGroup",
        cntrl: "/Admin/JobTaskGroup",
    };

    function getKeys() {
        var foreignType = $("#StepID").val() == "DetailsStep" ? "MachineType" : "RefubrishStep";
        //
        return {
            "foreignType": foreignType,//$("#DynamicObject").val(),
            "machineType": $("#MachineTypeID").val(),
            "step": $("#StepID").val()
        };
    }

    function getSubKeys(that) {
       
        var params = getKeys();
        params.Pid = GetFieldGroupID(that);

        return params;
    }

    var headerActiosClass = ".VbTabBlockHeader .HeaderActions";
    var container = "#fieldList";
    var selectedTag = "selected";
    var selectedClass = "." + selectedTag;
    var containerSelected = container + " " + selectedClass;

    var groupClass=".VbTabBlock";
    var groupDiv = container + " " + groupClass;
    var groupDivSelected = groupDiv + selectedClass;
    var groupDivTitle = groupDiv + " .BlockHeaderTitle";

    var subGroupClass = ".sub-group";
    var subGroupDiv = container + " " + subGroupClass;
    var subGroupDivSelected = subGroupDiv + selectedClass;
    var subGroupDivTitle = subGroupDiv + " .sub-group-title";

    var fieldClass=".field";
    var fieldsDiv = container + " " + fieldClass;
    var fieldsSelected = fieldClass + selectedClass;
    var fieldsDivSelected = container + " " + fieldsSelected;
    var fieldsDivTitle = fieldsDiv; 


    function GetFieldGroup(that) {
        return $(that).parents(groupClass);
    }
    function GetFieldSubGroup(that) {
        return $(that).parents(subGroupClass)
    }

    function GetGroupID(that) {
        return $(that).attr("data-id");
    }

    function GetSubGroupID(that) {
        return GetFieldSubGroup(that).attr("data-id");
    }

    function GetFieldGroupID(that) {
        return GetFieldGroup(that).attr("data-id");
    }

    function GetFieldGroupSelcted(that) {
        return GetFieldGroup(that).find(fieldsSelected);
    }

    function GetFieldGroupSelctedID() {
        return GetFieldGroupID($(fieldsSelected).first());
    }

    function GetSubPidID() {
        return GetFieldGroupID($(subGroupDivSelected).first());
    }
   
  

    function GetDataRowID(that) {
        return $(that).find("input, select, textarea").not(".EditableSelect").attr("name").split('_')[0]; /** maybe us subgroup field*/
    }

    /** is in group or sub*/
    function GetFieldContainer(dataRow) {
        var isInSubGroup = $(dataRow).parents(subGroupClass).is('*');
        return  isInSubGroup ? GetFieldSubGroup(dataRow) : GetFieldGroup(dataRow);
    }

    /*****************         db calls         ******************/

    function loadFields() {
        serSortable();
     
    }

    loadFields();

    function changeGroup(params) {
   
        rest.update(
          $.extend({}, opsGroup, {
              data: params,
              success: function () {
                  loadFields();
                  closePopup("popUpdateDynamicGroups");
              }
          })
      );

    }

    function changeField(params) {
       
        rest.update(
           $.extend({}, opsField, {
               data: params,
               success: function () {
                   loadFields();
                   closePopup("popUpdateDynamicFields");
               }
           })
       );
    }

    function deleteItems(ops, ids) {
      
        rest.remove(
          $.extend({}, ops, {
              data: {
                  "ids": ids,
              },
              traditional: true,
              success: function (data) {
                  if (data.sts === "Success")
                      loadFields();
                  else
                      $.BugAlert(data.msg);
              },
              fail: function () {
                  $.BugAlert("לא ניתן למחוק אובייקט אשר מידע מקושר אליו");
              }
          })
      );
 
    }

    function copyItems(ops, params) {
        params.IsCut = IsCut;
        
        rest.copy(
         $.extend({}, ops, {
             data: params,
             traditional: true,
             success: function () {
                 loadFields();
                 restMoveFields();
                 togglePasteBtn();
             },
             fail: function () {
                 data.msg = (data.responseText.indexOf("<!DOCTYPE html>") > -1) ? data.statusText : data.responseText;
                 console.log(data.msg);
             }
         })
     );
    }

    function sortItems(ops, params) {
       
        rest.sort(
         $.extend({}, ops, {
             data: params,
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


    function get(params, ops) {
        var action = "Update";

        $.OpenGeneralPop(action + ops.entity, null, params, false, ops.cntrl + "/" + action);
    }

    /*****************         UI FUNCTIONS          ******************/

    function serSortable() {
        /** fields sort*/
        $(groupClass + ' .VbTabBlockInfo .clearfix').sortable({
            update: function (event, ui) {
                var dataRow = ui.item[0];
                
                var searchIn = GetFieldContainer(dataRow);
                var fieldsSelcted = [];

                /** update sort of all fields in group/sub*/
                $(searchIn).find(fieldClass).map(function () {
                    fieldsSelcted.push(GetDataRowID(this));
                });

                var params = {
                    ItemsSelected: fieldsSelcted,
                    DynamicGroupID: GetFieldGroupID(dataRow),

                };

                //console.log(params);
                sortItems(opsField, params);
            }
        });

        /** group sort*/
        $(container).sortable({
            update: function (event, ui) {
                var params = {
                    ItemsSelected: []
                };
              
                /** update sort of all groups*/
                $(container).find(groupClass).map(function () {
                    params.ItemsSelected.push(GetGroupID(this));
                });
     
                sortItems(opsGroup, params);
            }
        });
    }
  


    function togglePasteBtn() {
        $(".PasteFieldsBtn").hide();
        $("#PasteBtn").hide();

        if (groupsSelcted.length > 0) {
            $("#PasteBtn").show();

            return;
        }

        if (subGroupsSelcted.length > 0) {
            $(".PasteFieldsBtn").show();
            var grppasteBtn = $(groupClass + "[data-id=" + subPid + "]").find(".PasteFieldsBtn");
            $(grppasteBtn).hide();

            return;
        }

        if (fieldsSelcted.length > 0) {

            $(".PasteFieldsBtn").show();
            var grppasteBtn = $(groupClass + "[data-id=" + fieldsGroupdId + "]").find(".PasteFieldsBtn");
            $(grppasteBtn).hide();
        }
    }

    function restMoveFields() {
        fieldsSelcted = [];
        groupsSelcted = [];
        subGroupsSelcted = [];
        fieldsGroupdId =subPid= null;
        $("#DeleteBtn").hide();
        $("#PasteBtn, .PasteFieldsBtn ").hide();
    }

    function toggleCopyBtn() {

        if ($(containerSelected).length == 0) {
            $(".move").hide();
            $("#DeleteBtn").hide();
        }
        else {
            $(".move").show();
            $("#DeleteBtn").show();
        }

    }

    function setSelectedItems() {
        restMoveFields();

        setSelctedByType();

       
        togglePasteBtn();
    }

    function setSelctedByType() {
        $(groupDivSelected).map(function () {
            groupsSelcted.push($(this).attr("data-id"));
        });

        if (groupsSelcted.length > 0)
            return;

        $(subGroupDivSelected).map(function () {
            subGroupsSelcted.push($(this).attr("data-id"));
        });
        subPid = GetSubPidID();

        if (subGroupsSelcted.length > 0)
            return;

        $(fieldsDivSelected).map(function () {
            fieldsSelcted.push(GetDataRowID(this));
        });
        fieldsGroupdId = GetFieldGroupSelctedID();

    }

    //make phase active
    $("#AddPhaseBtn").live("click", function (e) {
        var params = getKeys();

        addPhase(params);
    });

    //copy groups/fields
    $("#CopyBtn").live("click", function (e) {
        IsCut = false;
        setSelectedItems();
    });

    //cut groups/fields
    $("#CutBtn").live("click", function (e) {
        IsCut = true;
        setSelectedItems();
    });


    $("#SortModeBtn").live("click", function (e) {
       
    });
   
    $("#PasteBtn, .PasteFieldsBtn ").live("click", function () {

        /** paste groups*/
        if (groupsSelcted.length > 0) {
            var params = getKeys();
            params.ItemsSelected = groupsSelcted;

            copyItems(opsGroup, params);

            return;
        }

       
        /** paste sub groups*/
        if (subGroupsSelcted.length > 0) {

            var params = getSubKeys(this);
            params.ItemsSelected = subGroupsSelcted;

            copyItems(opsGroup, params);

            return;
        }

        /** paste fields*/
        var tergetGroupID = GetFieldGroupID(this);
        var params = {
            ItemsSelected: fieldsSelcted,
            DynamicGroupID: tergetGroupID,

        };
        copyItems(opsField, params);

       

    });

    //delete  groups/fields
    $("#DeleteBtn").live("click", function () {
       
        setSelectedItems();

        if (groupsSelcted.length > 0) {
            if (confirm("האם אתה בטוח שיהנך מעוניין למחוק את הקבוצה כולה כולל השדות שבה?")) {
              
                deleteItems(opsGroup, groupsSelcted);
            }
           
            return;
        }

   

        if (subGroupsSelcted.length > 0) {
            return deleteItems(opsGroup, subGroupsSelcted);
        }

        deleteItems(opsField, fieldsSelcted);

    });


    //select group
    $(groupDivTitle).live("click", function () {
        $(fieldsDiv).removeClass(selectedTag);//remove fields selected

        var grp =GetFieldGroup(this);
        $(grp).toggleClass(selectedTag);
        toggleCopyBtn();
    });

   //select sub group
    $(subGroupDivTitle).live("click", function () {
        $(fieldsDiv).removeClass(selectedTag);//remove fields selected

        var grp = GetFieldSubGroup(this);
        $(grp).toggleClass(selectedTag);
        toggleCopyBtn();
    });

    //select field
    $(fieldsDivTitle + ", #fieldList .CheckZone").live("click", function () {
        $(groupDiv).removeClass(selectedTag);//remove group selected

       
        var grpSelctedFields = GetFieldGroupSelcted(this);
       
        $(fieldsDivSelected).not($(grpSelctedFields)).removeClass(selectedTag);

        $(this).toggleClass(selectedTag);
        toggleCopyBtn();
    });

    //open edit field
    $(fieldsDiv).live("dblclick", function () {
        $(this).removeClass(selectedTag);
        var fieldID = GetDataRowID(this);
        get({ "FieldID": fieldID }, opsField);
    });

    //open new field
    $(".OpenAddFieldBtn").live("click", function () {
        var groupdid = GetFieldGroupID(this);
        get({ "GroupID": groupdid }, opsField);
    });

  

    //open edit group
    $(groupDivTitle).live("dblclick", function () {
        $(this).removeClass(selectedTag);
        var groupdid = GetFieldGroupID(this);
        get({ "GroupID": groupdid }, opsGroup);
    });

    //open new group
    $("#OpenChangeGroupBtn").live("click", function () {
        var params = getKeys();
        get(params, opsGroup);
    });

    //open edit group
    $(subGroupDivTitle).live("dblclick", function () {
        $(this).removeClass(selectedTag);
        var groupdid = GetSubGroupID(this);
        get({ "GroupID": groupdid }, opsGroup);
    });



    //open new group
    $(".OpenAddSubGroupBtn").live("click", function () {
       
        var params = getSubKeys(this);
        
        get(params, opsGroup);
    });

    //save group edit/add
    $("#ChangeGroupBtn").live("click", function () {
        var form = "#ChangeGroupForm";

        if ($(form).valid()) {
            changeGroup($(form).serialize());
        }


    });

    //save field edit/add
    $("#ChangeFieldBtn").live("click", function (e) {
        var form = "#AddFieldForm";

        if ($(form).valid()) {
            changeField($(form).serialize());
        }

    });



    //load page by navigator
    $("#navigator select").live("change", function () {
        loadFields();
    });





    //$("#DynamicObject").live("change", function () {
    //    $("#navigator #steps").toggle();
    //    //var foreignType = $("#DynamicObject").val();
    //});

    //if ($("#DynamicObject").val() == "RefubrishStep")
    //    $("#navigator #steps").show();

    $("#filterFields").live("keyup", function () {

        var str = $(this).val();
        var lis = ".fields li";
        if (str == "") {
            $(lis).show();
        }
        else {
            $(lis).hide();
            $(lis).filter(function (index) {
                return $(this).text().toLowerCase().indexOf(str) > -1;
            }).show();
        }


    });


});








