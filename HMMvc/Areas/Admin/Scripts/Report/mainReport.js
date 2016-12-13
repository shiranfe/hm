

var radioTpl = '<label><input type="radio" name="pivotFilter" value="{val}" /><span>{text}</span></label>';

function reportPivot(data, ops) {

    
    if ($("#radioFilters").find("label").length === 0) {
        createRadioFilers();
    }

    var dateFormat = $.pivotUtilities.derivers.dateFormat;
    var sortAs = $.pivotUtilities.sortAs;

    var options = $.extend({}, {
        rendererName: "טבלה",
        aggregatorName: "ספירה",
        derived:{},
        derivedAttributes: {},
        sorters: function (attr) {
            if (attr === "חודש") {
                return sortAs(monthsArr);
            }

        }
    }, ops);


    trySetDervied("year", "שנה", "%y");
    trySetDervied("month", "חודש", "%n");
    trySetDervied("week", "שבוע", "%w");
 
    $("#output").pivotUI(data, options,true);

    /** after pivot created try set drop with their vals*/
    setTimeout(CreateDropDown, 100);

    function trySetDervied(id, name, format) {
        if (options.derived[id]) {

            options.derivedAttributes[name] = dateFormat(options.derived[id], format, true);
        }
    }
}


function createRadioFilers() {
    for (var pivotFilter in configs) {
        $("#radioFilters").append(radioTpl.replace("{val}", pivotFilter).replace("{text}", configs[pivotFilter].text));
    }

    $("#radioFilters").find("input").first().prop('checked', true);
}



$(document).ready(function () {


    $(".pvtUi select").live("change", function () {
        setTimeout(CreateDropDown, 100);
    });

    $("[name=pivotFilter]").live("click", function () {
        $("#pivotTitle").text($(this).next().text());
        initPivot(pivotData, configs[$(this).val()]);
    });
});
