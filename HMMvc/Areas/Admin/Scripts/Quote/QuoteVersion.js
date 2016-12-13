

function CommonQuote_ready() {

   
    crudOps = crud(quoteVersionOps);

    /*****************         UI FUNCTIONS          ******************/


    $.initVersion();


    $("#Disscount").keyup(function () {
        calcSum();

    });

    $("span.EditIcon").click(function () {
        var quoteID = $("#UpdateQuoteVersionForm #QuoteID").val();
        openQuoteDetails(quoteID);

    });

    $("#Versions").change(function () {
        var versionID = $(this).val();
        openVersion(versionID);

    });

    $(".EmailIcon").click(function () {
        openEmail();

    });


    $("#addVersion").click(function () {
        add({
            SrcVersionID: $("#QuoteVersionID").val()
        });

    });
    $("#GetJobDetailsBtn").click(function () {
        openJobDetails($(this).attr("data-id"));
    });

    $("#ImportFromQuoteBtn").click(function () {
        importFromQuote($("#QuoteVersionID").val());
    });


    $("#UpdateQuoteVersionForm .VbTabBlockInfo:not('#QuoteVersionItems') [name]").live("change", function () {
        saveVersion();
    });

    $("#UpdateVersionBtn").click(function () {
        saveVersion();
    });

   

    $("#VersionDate").datepicker({
        defaultDate: "-1y",
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        dateFormat: "dd/mm/yy",

    });


}




/** if commom not ready, will be called from "LoadScript"*/
if (isFunction($.initVersion))
    CommonQuote_ready();

