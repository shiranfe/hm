
$(document).ready(function () {

    var ops = {
        entity: "SupplierProduct",
        cntrl: "/Admin/SupplierProduct",
        showRow: function ($row) {

            var showMaterial = $("#showMaterial").is(":checked") ? true : !($row.attr("data-ismaterial") === "True");
            var showService = $("#showService").is(":checked") ? true : !($row.attr("data-ismaterial") === "False");

            return showMaterial && showService;

        }
    }


   crud(ops);

  

    /*****************         UI FUNCTIONS          ******************/
    



});






