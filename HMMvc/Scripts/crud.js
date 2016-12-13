var lastSearch;
var crudEnts = [];

function crud(options) {
    var obj = $.crud(options);
    crudEnts.push(obj);
    return obj;
    //crudEnts[options.entity] = $.crud(options);
    //return crudEnts[options.entity];
}




$.crud = function (options) {

    //console.log(options.cntrl);

    var defualts = {
        entity: null,
        storageEntity: null,//serch by quote and not by quote track...
        cntrl: null,
        loadContent: false,
        itemUrl: null,
        container: null,//"#SiteBody"
        ForeignID: null,
        success: null,
        allowBack: false, //allow back key in popup
        onlyUpdate: false, //then no nned in serch, get...
        indexFilters: false,
        onFilterChange: null,//on filer change, save param to cache
        filterInPop: null,
        onPageLoad: null, // when return to page, refilter by storage
        showRow: function () { // more terms to show/hide row in search
            return true;
        },
        customValid: null,
        dontValidate: false,
        subEntity: "" //if have same another entity in page, add this attr to buttons live click
    }

    var ops = $.extend({}, defualts, options);

    var action = "Update";
    var form = "#" + action + ops.entity + "Form";
    var subm = form + " .ForBtnDiv [type=button]";
    //var pop = 'pop' + action  +ops.entity;

    /** if pop on pop need to have container for buttons*/
    var container = ops.container || "[data-page='" + ops.cntrl + "']";
    container += " ";

    var storageKey = ops.storageEntity || ops.entity;

    if (ops.indexFilters) {
        /** need only srch*/
        if (ops.indexFilters == true)
            ops.indexFilters = [];

        ops.indexFilters.push(["srch", null]);
    }

    function crudSuccess(data, removed) {

        if (data.sts == "fail")
            return $.BugAlert(data.msg);
        if (options.success)
            options.success(data, removed);
        else {
            // lastSearch = $("#srch").val();
            partialLoad(false, null, function () {
                initSearch();
            });
        }
    }

    function crudFail(data) {

        $.BugAlert("העדכון לא בוצע.");
        //console.log(  data);
    }


    function get(id) {

        /** open page*/
        if (ops.itemUrl) {
            location.href = ops.itemUrl + id;
            return;
        }

        /** open popup*/
        var params = { "id": id, "ForeignID": ops.ForeignID }
        $.OpenGeneralPop(action + ops.entity, null, params, false, ops.cntrl + "/" + action, ops.allowBack);
    }

    function loadContent() {
        rest.getContent({
            cntrl: ops.cntrl,
            success: function () {
                initSearch();
            }
        });
    }



    function update(data) {
        $(subm).prop('disabled', true);
        if (ops.ForeignID) {
            data += "&ForeignID=" + ops.ForeignID;
        }


        rest.update(
            $.extend({}, ops, {
                data: data,
                success: crudSuccess,
                fail: crudFail,
                done: function () {
                    $(subm).prop('disabled', false);
                }
            })
        );
    }

    function remove(id) {

        rest.remove(
           $.extend({}, ops, {
               data: { "id": id },
               success: function (data) {
                   crudSuccess(data, true);
               },
               fail: crudFail
           })
        );

    }


    function showRow($row, srchText) {
        var srchTerm = sanitize($row.text()).indexOf(srchText) > -1;

        return srchTerm && ops.showRow && ops.showRow($row);
    }

    function addToStorage(data) {
        localStorage.setItem(storageKey, JSON.stringify(data));
    }

    function getFilterParams() {
        return urlParamsToObj() || getFromStorage();
        // return JSON.parse(localStorage.getItem(storageKey)) || {};
    }

    function getFromStorage() {
        return JSON.parse(localStorage.getItem(storageKey)) || {};
        // return JSON.parse(localStorage.getItem(storageKey)) || {};
    }

    /** filter on server side*/
    function filterTable(pageChanged) {

        if (!ops.indexFilters)
            return;

        function onFilterChange(saveToStorage) {
            var filters = {};

            for (var i = 0; i < ops.indexFilters.length; i++) {
                var filter = ops.indexFilters[i];
                var id = filter[0];

                filters[id] = $("#" + id).val();
            }

            saveToStorage(filters);
        }


        onFilterChange(function saveAndThen(data) {
            /** need to recalculte pagees if changed filters*/
            var filterData = pageChanged ? $.extend({}, {
                Page: $("#Page").val(),
                PageTotal: $("#PageTotal").val(),

            }, data) : data;

            filterData.On = true;

            if (ops.filterInPop) {
                ops.filterInPop(filterData);
                /** table filter in pop*/
                // ops.filterInPop(filterData);
                showLoading("filterInPop");
                rest.get({
                    url: ops.cntrl,
                    data: filterData,
                    success: function (html) {
                        $(ops.container + "_Content").html(html);
                        onPageLoad(getFromStorage());
                        CreateDropDown();
                        hideLoading("filterInPop");
                    }
                });
            }
            else {
                setUrlParams(filterData);

            }
            addToStorage(data);

        });



    }

    function onPageLoad(data) {

        for (var i = 0; i < ops.indexFilters.length; i++) {
            var filter = ops.indexFilters[i];
            var id = filter[0];
            var defVal = filter[1];

            if (defVal == null)
                $("#" + id).val(data[id]);
            else
                $.setAutoCompleteVal("#" + id, data[id] || defVal);
        }

    }

    /*
    function filterTable() {

        var srchStr = sanitize($("#srch").val());

        typewatch(function () {
            //split the current value of searchInput

            //create a jquery object of the rows
            var jo = $(container + "tbody").find("tr");
            if (jo.length == 0)
                jo = $(".searchZone").find(".searchSrc");//for none table items

            if (this.value == "") {
                jo.show();
                return;
            }
            //hide all the rows
            jo.hide();

            showSmallLoading($("#srch"));
            //Recusively filter the jquery object to get results.
            var rowsToShow = jo.filter(function (i, v) {
                var $row = $(v);

                return showRow($row, srchStr);

            });

            for (var i = 0; i < rowsToShow.length; i++) {
                var group = $(rowsToShow[i]).parent().find(".group");
                if (!group)
                    break;

                group.show();
            }

            //show the rows that match.
            rowsToShow.show();
            hideSmallLoading($("#srch"));


            addToStorage();

        }, 500);


    }
    */

    function getGoToPage(that) {

        /** clicked a page number*/
        if (that.hasClass("page"))
            return that.text();

        var current = parseInt($("#Page").val());

        if (that.hasClass("previous"))
            return current - 1;

        //else if (that.hasClass("next"))
        return current + 1;
    }

    function initSearch() {
        /** try ge filter values from storage*/
        if (ops.indexFilters)
            onPageLoad(getFilterParams());

        /** initiate filterinf on page load*/
        //  filterTable();
    }

    var bindings = [];


    /** unbind all "live"  */
    function unbindAll() {

        for (var i = 0; i < bindings.length; i++) {
            bindings[i].die();
        }
    }


    //function changeForeignId(id) {
    //    ops.ForeignID = id;
    //}

    $(document).ready(function () {


        /*****************         UI FUNCTIONS          ******************/

        //will ignore un visible fields
        // $(form).validate({ ignore: ":not(:visible)" });

        bindings[0] = $(subm).live("click", function () {

            if (ops.dontValidate || ($(form).valid() && (!ops.customValid || ops.customValid()))) {
                update($(form).serialize());
            }
            else {
                if (!$(form).valid()) {
                    scrollToError();
                }
            }

        });

        if (ops.onlyUpdate)
            return;

        bindings[1] = $(container + "#AddItemBtn" + ops.subEntity).click(function () {
            get();

        });

        bindings[2] = $(container + ".openItem" + ops.subEntity).live("click", function () {
            //console.log(ops.container);
            var id = $(this).parents("tr").attr("data-id");
            get(id);

        });

        bindings[3] = $(container + ".deleteItem" + ops.subEntity).live("click", function () {
            var id = $(this).parents("[data-id]").attr('data-id');
            remove(id);
        });

        ///** will trigger also after returning from update, hapning in generel.js/partialLoad */
        bindings[4] = $("#srch").live('keyup', function (e) {
            if (e.keyCode == 13 || !$(this).val()) {
                filterTable();
            }

        });


        if ($(container + "table tr").length > 1)
            $(container + "table").tablesorter();
        //console.log('form: ' +subm);
        //console.log('container: ' +container);

        if (ops.loadContent) {
            loadContent();
        }


        if ($(".searchFields")[0]) {


            bindings[5] = $(".searchFields select,.searchFields input[type=checkbox]").live("change", function () {
                filterTable();
            });

            bindings[6] = $(".searchFields .filterBox li").live("click", function () {
                filterTable();
            });

            bindings[7] = $(".searchFields .clear").live("click", function () {
                onPageLoad({}); //need to set defualt value in mehtod when data is empty like here
                filterTable();
            });


            bindings[8] = $(".page-control span:not(.current)").live("click", function () {

                $("#Page").val(getGoToPage($(this)));

                filterTable(true);

                if (!ops.filterInPop)
                    scrollTo("thead");
            });

            initSearch();

        }
    });

    return {
        filterTable: filterTable,
        loadContent: loadContent,
        id: form,
        remove: remove,
        unbindAll: unbindAll,
        // changeForeignId: changeForeignId
    };
};

