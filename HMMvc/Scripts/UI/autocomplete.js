
/** AUTOCPMLETE
* if want keyborad and other stuff to work use    $(element).autocomplete
*/
$(document).ready(function () {

    // var _tempId = "data-temp-id";
    var _tempText = "data-temp-text";
    var _placeHolder = "data-placeholder";
    var _hidden = "[type=hidden]";
    var _search = "[type=text]";
    var _resultLi = ".list li:not(.empty)";
    var _emptyLi = ".list li.empty";
    var _noneLi = _emptyLi +".none";
    var _addLi =  _emptyLi +".add";

    /** GETTER */

    function getSearchText(container) {
        return container.find(_search).attr("placeholder");
    }

    function getSearchValue(container) {
        return container.find(_search).val();
    }

    function getPlaceholder(container) {
        return container.attr(_placeHolder);
    }


    //function getHiddenId(container) {
    //    return container.attr(_tempId);
    //}

    function getHiddenText(container) {
        return container.attr(_tempText);
    }

    function getItemByVal(container, id) {
        return container.find(".list li[data-id='" + id + "']").first();
    }

    /** can be under ".filer" or in client tree case outside of it*/
    function getHiddenInput(container) {
        if (container.find(_hidden).is("*"))
            return container.find(_hidden);
        /** in client tree field is not hidden*/
        return container.parents(".JobClientDrop").find("#ClientID");
    }

    function getHiddenValue(container) {
        return getHiddenInput(container).val();
    }


    /** SETTER */
    function setSearchText(container, value) {
        if (!value)
            value = getPlaceholder(container);
        container.find(_search).attr("placeholder", value);
    }

    function setSearchValue(container, value) {
        container.find(_search).val(value);
    }

    function setHiddenValue(container, value) {
        if (!value)
            return getHiddenInput(container).val(value);

        getHiddenInput(container).val(value).trigger('change');
    }

    //function setHiddenId(container, value) {
    //    container.attr(_tempId, value);
    //}

    function setHiddenText(container, value) {
        container.attr(_tempText, value);
    }


    function hasResult(container) {
        return container.find(_resultLi).length > 0;
    }




    function setOptionSelected(container, id, text) {

        /** final values*/
        setHiddenValue(container, id);
        setHiddenText(container, text);

        /** set visible search values*/
        setSearchValue(container, null);
        setSearchText(container, text);

        /** set selected css*/
        container.find(".list li").removeAttr("selected");
        container.find(".list li[data-id='" + id + "']").attr("selected", "selected");
        // ::-webkit-input-placeholder - will set as bold when has valid value
     
        if (!container.parents("form")[0])
            return;

        /** validate input*/     
        setTimeout(function () {
            getHiddenInput(container).valid();
        }, 201);

    }


    function closedOptions(container) {

        /** hide option list*/
        container.find('.list').hide();
        /** show all result*/
        container.find(_resultLi).show();
        if (hasResult(container))
            container.find(_noneLi).hide();

        var hasSearchText = (getSearchValue(container) || false);

        if (!hasSearchText || getHiddenText(container) == getSearchValue(container)) 
            return;

        /** no valid option was selected */


        /** allow aditions not from list and search has value*/
        if (container.is("[data-allowadd]") && hasSearchText) {
            return setOptionSelected(container, -1, getSearchValue(container));
        }

        /** resotre to last valid value*/
        setTimeout(function () {
            setOptionSelected(container, getHiddenValue(container), getHiddenText(container));
        }, 10);
        
    }

    $.autoCompSelected = function (container) {
        closedOptions(container);
    };


    /** select value - value selection on closedOptions*/
    $(".autocomplete " + _resultLi).live('click', function () {
        var container = $(this).parents(".autocomplete");

        setOptionSelected(container, $(this).data("id"), $(this).find("span").text());

    });

    /** filter list*/
    $('.autocomplete ' + _search).live('keyup', function () {
        var container = $(this).parents(".autocomplete");


        var str = sanitize($(this).val());
        var lis = container.find(_resultLi);
        if (str == '') {
            container.find(_addLi).hide();
            return lis.show();
        }


        lis.hide();
        container.find(_noneLi).hide();

        showSmallLoading(this);

        var matches = lis.filter(function (index) {
            return !$(this).hasClass("empty") && sanitize($(this).text()).indexOf(str) > -1;
        });

        if (matches.length > 0) {
            matches.show();
        }
        else {
            container.find(_noneLi).show();
        }

        if (getSearchValue(container))
            container.find(_addLi).show();
        else
            container.find(_addLi).hide();

        hideSmallLoading(this);

    });




    /** on search blur - (also when option selected)*/
    $('.filterBox ' + _search).live('blur', function (e) {
        var container = $(this).parents(".filterBox");
        setTimeout(function () {

            closedOptions(container);
        }, 200);

    });


    /** show list*/
    $(".filterBox input,.filterBox .SelectBtn,.autocomplete li").live('click', function () {
        var container = $(this).parents(".filterBox");

        /** if doabled dot open list*/
        if (container.find("[disabled]").is("*"))
            return;

        if (container.find('.list').is(':visible'))
            closedOptions(container);
        else {
            if (!hasResult(container))
                container.find(_noneLi).show();
            if (!getSearchText(container))
                container.find(_addLi).hide();

            container.find('.list').show();
           
           
        }

    });


    /** poplate optins after loaded in ajax*/
    $.setAutoCompleteOptions = function (container, loadOptions, selectedVal) {

      
        var ul = container.find("ul");
        ul.find("li:not(.empty)").remove();


        loadOptions(function populateDataOnSuccess(data) {

            var selected = {
                Value: "",
                Text: null
            };

           

            //var first = true;
            /** need to add result before "empty.add"*/
            data.reverse();
            $.each(data, function (field, item) {
                var extra = "";
                if (item.Rpm) {
                    extra = ' data-rpm="' + item.Rpm + '" data-kw="' + item.Kw + '"' + '" data-address="' + item.Address + '"';
                }
                var option = '<li data-id="' + item.Value + '" selected="false" ' +extra+ '>' + item.Text + '</li>';
               

                ul.prepend(option);
                //if (first) {
                if (selectedVal == item.Value) {
                    selected = item;
                }
                //first = false;
            });

            /** empty answer*/
            ul.prepend('<li data-id=""></li>');

            /** set selected option*/
            setOptionSelected(container, selected.Value, selected.Text);

            //closedOptions(container);

        });

    };


    $.setAutoCompleteVal = function (id, value) {
        var container = $(id).parents(".autocomplete");
        var selectedLi = getItemByVal(container, value);

        setOptionSelected(container, value, selectedLi.find("span").text());
    };


    $.getAutoCompleteText = function (id) {
        var container = $(id).parents(".autocomplete");
  
        return getSearchText(container);
    };
});