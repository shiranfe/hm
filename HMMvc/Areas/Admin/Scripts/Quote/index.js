$(document).ready(function () {

    crud({
        entity: "Quote",
        cntrl: "/Admin/Quote",
        indexFilters: quoteFilters,
        success: function success(data, removed) {

            if (data.updated || removed) {
                closePopup("popUpdateQuote");
                return refreshPage();
            }


            location.href = "/Admin/#/QuoteVersion/Update?id=" + data.CurrentVersionID;
            refreshPage(); /** from some reason location.href  wont redirect by itself*/


        }
    });


});
