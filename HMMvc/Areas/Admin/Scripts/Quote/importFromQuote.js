$(document).ready(function () {

    var form = "#popImportFromQuote";

    var ops = crud({
       // entity: "ImportFromQuote",
        entity: "Quote",
        cntrl: "/Admin/Quote/ImportFromQuote",
        indexFilters: quoteFilters,
        container: form,
        filterInPop: function filterInPop(params) {
            params.DestVersionID = $("#QuoteVersionID").val();
            //rest.get({
            //    url: "/Admin/Quote/ImportFromQuote",
            //    data: params,
            //    success: function (html) {
            //        $(form + "_Content").html(html);
            //        CreateDropDown();
            //    }
            //});
        }
    });

    function importQuote(id) {
        $(".importQuote").prop('disabled', true);
       
        rest.post({
            url: "/Admin/QuoteVersion/ImportFromQuote",
            data: {
                SrcVersionID: id,
                DestVersionID: destVersionID
            }, success: function (data) {
                closePopup("popImportFromQuote");
                //partialLoad(false, null, function () {
                //    $.initVersion();
                //});
                refreshPage();

                //location.reload()
            },
            done: function () {
               
                $(".importQuote").prop('disabled', false);
            }
        });
    }


    $(".importQuote").click( function () {
        var id = $(this).parents("tr").data("id");
        importQuote(id);


    });
});