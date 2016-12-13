
var _ = {
    versionItemID: "QuoteVersionItemID",
    versionID: "QuoteVersionID",
    title: "ItemTitle",
    sort:  "ItemSort",
    quntity: "ItemQuntity",

    pricePerUnit:"ItemPricePerUnit",
    catalogItemID:"CatalogItemID",
    notes:  "ItemNotes",
    fieldPoolID:"FieldPoolID",
    fieldValue:"FieldValue",
    parentID:"ItemParentID"
};

var rows = {
    info: "openOptions",
    notes: "notes",
    actions: "actions"
};

var quoteVersionItem = (function ($) {

    var setRowValuesFromCatatlog = function (item, formTr) {
        setVal(formTr, _.title, item.text);
        setVal(formTr, _.catalogItemID, item.value);
        setVal(formTr, _.pricePerUnit, item.price,true);
        setVal(formTr, _.fieldPoolID, item.fieldPoolID);

        var notesTr = $(formTr).next();
        setVal(notesTr, _.notes, item.notes);

        calcItemTotalPrice(formTr);
    };


    var getVal = function (row, field) {
        var id = $(row).data("id");
        var name = "[name=\"" + field + "\"]";
        return $('[data-id="' + id + '"]').find("input" + name + ", textarea" + name + ", select" + id).val();
    };

    var setVal = function (row, field, value, isMoney) {
        var id = "[name=\"" + field + "\"]";
        $(row).find("input" + id + ", select" + id + ", textarea" + id).val(value);
       
        updateLabel( field, value, isMoney);
    };

    var updateLabel = function (field, value, isMoney) {

        $('.open [label="' + field + '"]').text(isMoney ? toNis(value) : value);
    };

    var calcItemTotalPrice = function (formTr) {
        var ItemQuntity = getVal(formTr, _.quntity);
        var ItemPricePerUnit = getVal(formTr, _.pricePerUnit);

        var total = ItemQuntity * ItemPricePerUnit;
        var totalForm = toNis(total);

        $(formTr).find(".itemTotalPrice").text(totalForm);

        //calcQuoteSum()
    };


    function calcQuoteSum() {
        var total = 0;
        $('.openOptions:not(.child):not(.add) .itemTotalPrice').each(function () {
            total += strCurrencyToFloat($(this).text());
        });

        $("#VersionSum").text(toNisRound(total));
        calcSum();
    }




    return {
        setRowValuesFromCatatlog: setRowValuesFromCatatlog,
        setVal: setVal,
        getVal: getVal,
        updateLabel:updateLabel,
        quoteVersionItem: quoteVersionItem,
        calcItemTotalPrice: calcItemTotalPrice,
        calcQuoteSum: calcQuoteSum
    };

})(jQuery);

$(document).ready(function () {

    var ops = {
        entity: "QuoteVersionItem",
        cntrl: "/Admin/QuoteVersionItem",
        container: "#QuoteVersionItems"
    };

    crudOps = crud(ops);


    // for adding o catalog option
    crud({
        entity: "CatalogItem",
        cntrl: "/Admin/CatalogItem"
    });

    /*****************         METHODS          ******************/

    var success = function (data, reload) {

      

        if (reload)
            return partialLoad(false);

        quoteVersionItem.calcQuoteSum();

        setMarginsClass();
    };

    function update(model, done) {
      
        rest.update(
            $.extend({
                data: model,
                success: function (data) {
                    if (!model.QuoteVersionItemID)
                        $("#quote-items").append(data);
                    CreateDropDown();
                    success(data)
                    if (done) {
                        done();
                    }
                }
            }, ops)
        );
    }

    function remove(id) {
      
        rest.remove(
             $.extend({
                 data: { "id": id },
                 success: function (data) {
                     $('tr[data-id="' + id + '"]').remove();

                     success(data)
                 }
             }, ops)    
        );

    }

    /** get bearing list... - loading all from start no nned*/
    function getFieldPool(fieldPoolID) {
        showLoading("quoteItem");
        rest.get({
            entity: "DynamicFields",
            cntrl: "/Admin/DynamicFields",
           // url: "/Admin/DynamicFields",///GetDynamicSelectList
            data: {
                FieldPoolID: fieldPoolID
            },
            success: function (data) {
                console.log(data);

                hideLoading("quoteItem");
            },
            fail: function () {
                $.BugAlert("שגיאה בטעינה");
            }
        } );

    }


    function clearAddForm() {

        $("tr.add input[type!=\"button\"]").val("");
        $("tr.add textarea").val("");
        setVal("tr.add", _.quntity, 1);
        $.setAutoCompleteVal("tr.add #" + _.fieldPoolID, "0");

        $("tr.add .itemTotalPrice").empty();
    }


    clearAddForm();

    function openAddItemToCatalog(ItemTitle) {
        $.OpenGeneralPop("UpdateCatalogItem", null, { ItemTitle: ItemTitle }, false, "/Admin/CatalogItem/Update");
    }


    function addFromJob(item) {
       
        var catalogItem = catalogItems.filter(function (obj) {
            return obj.value == item.CatalogItemID;
        })[0];

      
        var param = $.extend({},

            {
                QuoteVersionID: $("#" + _.versionID).val(),
                ItemTitle: catalogItem.text,
                ItemSort: getLastInOrder(),
               // ItemQuntity: getVal(formTr, _.quntity),

                ItemPricePerUnit: catalogItem.price,
               // CatalogItemID: getVal(formTr, _.catalogItemID),
                ItemNotes: catalogItem.notes

            }, item);

       // console.log(param);
        update(param, success);

    }

    function saveItem(btnRow, success) {


        //var btnRow = $(row).parents("tr");
        var id = getRowId(btnRow);
        var notesTr = getRow(btnRow,rows.notes);
        var formTr = getRow(btnRow, rows.info);


        if (!$(formTr).find("input").valid())
            return false;

        var param = {
            QuoteVersionID: $("#" + _.versionID).val(),
            ItemTitle: getVal(formTr, _.title),
            ItemSort: getVal(formTr, _.sort),
            ItemQuntity: getVal(formTr, _.quntity),

            ItemPricePerUnit: getVal(formTr, _.pricePerUnit),
            CatalogItemID: getVal(formTr, _.catalogItemID),
            ItemNotes: getVal(notesTr, _.notes),
            FieldPoolID: getVal(notesTr, _.fieldPoolID),

            ItemParentID: getVal($(btnRow), _.parentID),

        }

        if (id)
            param.QuoteVersionItemID =id;

        if (param.FieldPoolID)
            param.FieldValue = getVal(notesTr, "FieldValue");

        update(param, success);

        return true;

    }


    function saveIfNotNew() {
        isAddRow = $(".options.open").hasClass('add');
        if (!isAddRow)
            saveItem($(".options.open").last());
    }

    /*****************         UI FUNCTIONS          ******************/

    function setMarginsClass() {
        $('#quote-items tr').removeClass('last').removeClass('first');

        $('#quote-items .actions').first().addClass('first');
        $('#quote-items .openOptions').first().addClass('first');

        $('#quote-items .actions').last().addClass('last');
        $('#quote-items .openOptions').last().addClass('last');
    }

    setMarginsClass();

    function getVal(row, field) {
        return quoteVersionItem.getVal(row, field);
    }

    function setVal(row, field, value) {
        quoteVersionItem.setVal(row, field, value);
    }

    function closeOps() {
        //close row
        $("tr:not(.add)").removeClass("open");
        $(".options:not(.add)").hide();

    }


    // reset form to its db values (if start to edit row but didnt save, this will reset the values)
    function resetEditForm() {
        var fields = $("tr.open [name]");

        $.each(fields, function (field, item) {

            $(item).val($(item)[0].defaultValue);
        });

        //
    }

    function getLastInOrder() {
        var $hiddenSorts = $('[data-id]:not(.add)  [name=' + _.sort + ']');
        var lastOrder = $hiddenSorts.length > 0 ? $hiddenSorts.last().val() : 0;
        return  parseInt(lastOrder) + 1;
    }
    
    function changeParent(thisBtnRow) {
        saveItem(thisBtnRow);
        quoteVersionItem.calcQuoteSum();
    }


    function changeOrder(model) {

        var oldOrder = getVal(model.srcInfo, _.sort);
        var newOrder = getVal(model.destInfo, _.sort);

        //console.log("newOrder:" + newOrder);
        //console.log("oldOrder:" + oldOrder);

        setVal(model.destInfo, _.sort, oldOrder);

        var thisPid = getVal(model.srcActions, _.parentID);
        var newPid = getVal(model.destActions, _.parentID);

        if (thisPid != newPid)
            setVal(model.destInfo, _.sort, oldOrder);

        saveItem(model.destActions, function () {
            setVal(model.srcInfo, _.sort, newOrder);
            saveItem(model.srcActions);
        });

    }

    function assignAutoComplete(element) {
        $(element).autocomplete({
            minLength: 0,
            source: catalogItems,
            focus: function (event, ui) {
                if (ui.item.value == "add" || ui.item.value == "notExist")
                    return false;

                $(this).val(ui.item.label);
                return false;
            },
            select: function (event, ui) {

                if (ui.item.value == "add") {
                    var newItemName = $("tr.open input[name=\"" + _.title + "\"]").val();
                    openAddItemToCatalog(newItemName);
                    return false;
                }

                if (ui.item.value == "notExist") {

                    return false;
                }

                $(this).val(ui.item.label);

                quoteVersionItem.setRowValuesFromCatatlog(ui.item, $(this).parents("tr"));

                saveIfNotNew();

                return false;
            },
            response: function (event, ui) {
                //ui.content.push({
                //    value: 'add',
                //    label : " + הוסף כפריט חדש",
                //    notes:"בכדי להוסיף את הפריט יש ליצור אותו תחילה בקטלוג"
                //});
                if (ui.content.length == 0) {
                    ui.content.push({
                        value: "notExist",
                        text: "פריט לא קיים בקטלוג",
                        notes: "אנא פנה למנהל מערכת בכדי להוסיף פריט"
                    });
                }


            }
        })
        .autocomplete("instance")._renderItem = function (ul, item) {

            var liHtml = getAutoComleteRowHtml(item);

            return $("<li>")
              .append(liHtml)
              .appendTo(ul);
        };
    }

    function getAutoComleteRowHtml(item) {
        if (item.value == "add") {
            return "<a id=\"AddItemBtn\"><span class=\"b\">" + item.text + "</span><div  style=\"color:gray\">" + item.notes + "</div></a>";//add to catatlog row
        }

        if (item.value == "notExist") {
            return "<a ><span class=\"b red\" >" + item.text + "</span><div  style=\"color:gray\">" + item.notes + "</div></a>";//item dont exist in catalog
        }

        //catalog row
        return "<a>" + item.text + "<span class=\"fl\" style=\"color:gray\">" + toNis(item.price) + "</span></br><span  style=\"color:gray\">" + item.notes + "</span></a>"
    }

    /*****************      ROW HELPER       **************/

    function getRowId(elem) {
        return isItemRow(elem) ? $(elem).data("id") : $(elem).parents("tr").data("id");
    }

    function getRow(elem, row) {
        var id = getRowId(elem);

        var str = '[data-id="' + id + '"].';

        // console.log($(str + row)[0]);
        return $(str + row);

    }

    function getItemRows(elem) {
        var id = getRowId(elem);

        return $('[data-id="' + id + '"]');

    }


    function isItemRow(item) {
        return $(item).is("[data-id]");
    }

    function getSibblingClass(elem) {
        var pid = getVal(elem, _.parentID);//[data-id="' + pid + '"]
        return pid ? '.child' : ':not(.child)';
    }

    function getPrevSibbling(elem, row) {
        var condClass = getSibblingClass(elem);
        return getRow(elem.prev(condClass), row);
    }

    function getNextSibbling(elem, row) {
        var condClass = getSibblingClass(elem);
        return getRow(elem.next(condClass), row);
    }


    /***************************************************/

    $(".openOptions").live("click", function (e) {
        if ($(this).hasClass("open") || e.target.className.indexOf("move-row") > -1) {
            return; // clicked on opened
        }



        $("tr").removeClass("open");
        $(".options").hide();

        $(this).addClass("open");
        var notesTr = $(this).next();
        $(notesTr).toggle().addClass("open");

        var optionsTr = $(notesTr).next();
        $(optionsTr).toggle().addClass("open");



        //resetEditForm();

    });

    $(".saveItem").live("click", function () {
        //var isFirstItem = $(".openOptions").first().hasClass("add");

        /** set new item sort number*/
     
        setVal("tr.add", _.sort, getLastInOrder());
       
        if (saveItem( getRow( $(this), rows.actions ) ) ){
            clearAddForm();
            $(".openOptions.add").find("input[name='"+_.title+"']").focus();
            closeOps();
            
        }
       
    });

    $("[data-catalog]").live("click", function () {
      
        addFromJob({
            ItemQuntity:   $(this).parents("tr").find(".Quntity").text() || 1,
            CatalogItemID: $(this).data("catalog")
        })
    });

    $(".delete").live("click", function () {
        var id = getRowId($(this).parents("tr"));
        remove(id);
    });

    $(".closeOptions").live("click", function () {
        closeOps();
    });

    $(".move-row").live("click", function () {
        
        var model = {
            srcInfo: getRow(this, rows.info),
            srcActions: getRow(this, rows.actions),
            destInfo:null,
            destActions:null
        }

        var itemRows = getItemRows(model.srcInfo);
        /** 
            - destActions
            - -------------
            - srcInfo
            - 
            - srcActions
            - -------------
            - destInfo
        */

        /** move row up*/
        if ($(this).is(".up") ) {
            if (!isItemRow(model.srcInfo))
                return;

            /** cant move up the first item*/
            if(model.srcInfo.is(".first"))
                return;

            model.destActions = getRow(model.srcInfo.prev(), rows.actions);
            model.destInfo = getRow(model.destActions, rows.info);

            changeOrder(model);
          
            itemRows.insertBefore(model.destInfo);
            return;

        }

        if ($(this).is(".down")) {
            if (!isItemRow(model.srcActions))
                return;

            /** cant move down the last item*/
            if (model.srcInfo.is(".last"))
                return;


            model.destInfo = getRow(model.srcActions.next(), rows.info);
            model.destActions = getRow(model.destInfo, rows.actions);

            changeOrder(model)

            itemRows.insertAfter(model.destActions);

            return;
        }
        
        var thisPid = getVal(model.srcActions, _.parentID);
        model.destActions =model.srcInfo.prev();

        if (!isItemRow(model.destActions))
            return;

        
        /** make root*/
        if ($(this).is(".root")) {
            if (!thisPid)
               return;

            setVal(model.srcActions, _.parentID, null);
            itemRows.removeClass('child');
            return changeParent(model.srcActions)
           
        }

        /** if prev is also child - take its parent*/
        var newPid = getVal(model.destActions, _.parentID);

        /** make child*/
        if ($(this).is(".child")) {
            if (thisPid)
                return;

            if (!newPid)
                newPid = getRowId(model.destActions);

            setVal(model.srcActions, _.parentID, newPid);
            itemRows.addClass('child');
            changeParent(model.srcActions)
            
        }
    });

    $("input[name=\"ItemQuntity\"], input[name=\"ItemPricePerUnit\"]").live("keyup", function () {
        var formTr = getRow(this, rows.info);
        quoteVersionItem.calcItemTotalPrice(formTr);


    });

    $(".open input, .open textarea,.open select#FieldValue").live("change", function () {
        var name = $(this).attr('name');

        quoteVersionItem.updateLabel(name, $(this).val(), (name == _.pricePerUnit));
        
        saveIfNotNew();

        //if (name == "ItemQuntity" || name == "ItemPricePerUnit")
        //    $('.open [name="' + name + '"]').text($(this).val());
    });

    $("#FieldPoolID").live("change", function () {
        //        getFieldPool($(this).val());
        var poolId = $(this).val();
        var $fieldValue =  $(".open #FieldValue");

        /** toggle fieldValue visibality. if poolId empty update db*/
        if (poolId && poolId!="0") {
            $fieldValue.parents('.floatlast').show();
        }
        else {
            $fieldValue.parents('.floatlast').hide();
            $(this).val(null);
            quoteVersionItem.updateLabel(_.fieldValue, "");
            saveIfNotNew();
        }
  
        /** filter fieldValue slect options bu fieldpoolId*/
        var $valueFilter = $fieldValue.parents(".filterBox");
        $valueFilter.find(".list li").hide();
        $valueFilter.find(".list li[data-extra='" + poolId + "']").show();

        /** reset FieldValue value*/
        $valueFilter.find('.text').val(null);
        $valueFilter.find('.text').attr("placeholder", null);
        $valueFilter.find('[type=hidden]').val(null);
        //changeDropSelected($fieldValue, "");
        //quoteVersionItem.updateLabel("FieldValue", "");
       
        //console.log($fieldValue.find("option[data-extra=" + poolId + "]"));
    });

   
    $("input[name=\"ItemTitle\"]").live("focus", function () {
        assignAutoComplete(this);
    });

    ///** need to change it to textarea*/
    //$("input[name=\"ItemNotes\"]").live("focus", function () {     
    //    $(this).autosize();
    //});

   $('body').live('click', function (t) {
        if (!($(t.srcElement).parents('.open').is("*"))) {
            closeOps();
        }
    });

    /** on page load*/
    $("[name=ItemNotes]:not([style])").autosize();
   
    /** on item created*/
    $("[name=ItemNotes]:not([style])").live('click', function () {
        $(this).autosize();
    });
       
});








