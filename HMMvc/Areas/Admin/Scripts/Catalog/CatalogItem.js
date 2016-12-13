var catalogItemID;
var catalogItems;
$(document).ready(function () {

    var ctrl = "/Admin/CatalogItemComponent";

    var itemID = $("#popUpdateCatalogItem #CatalogItemID");
    
 
    var ops = {
        entity: "CatalogItemComponent",
        cntrl: ctrl,
        container: "#popUpdateCatalogItem",
        ForeignID: catalogItemID,
        success: function (data, removed) {
            if (!removed)
                closePopup("popUpdateCatalogItemComponent");

            rest.get({
                url: ctrl + "/ItemComponents",
                data: {
                    CatalogItemID: catalogItemID
                },
                success: function (html) {
                    $("#componentsDiv").html(html);
                }
            });
        },
       
    }

    crud(ops);

    //overide crud update
    function update(data) {
        var ops = {
            entity: "CatalogItem",
            cntrl: "/Admin/CatalogItem",
        };

        rest.update(
            $.extend({
                data: data,
                success: function (data) {
                   
                    /** catalog page*/
                    if (isInPage('/Admin/CatalogItem')) {
                        lastSearch = $("#srch").val();
                        return partialLoad();
                    }
                       
                    /** quote page*/
                    catalogItems.push(data);

                    quoteVersionItem.setRowValuesFromCatatlog(data, $("tr.open input[name=\"ItemTitle\"]").parents("tr"));

                    closePopup("popUpdateCatalogItem");
                }
            }, ops)
        );
    }


    /*****************         UI FUNCTIONS          ******************/

    //overide crud get
    function openUpdateComponent(id){
        var action = "Update";
        var params = { "ForeignID": id }
       
        $.OpenGeneralPop(action + ops.entity, null, params, false, ops.cntrl + "/" + action);
    }

    // must have CatatlogItemID before add components
    $("#AddComponentBtn").live("click", function () {
       
        if (catalogItemID > 0) {
            openUpdateComponent(catalogItemID);
            return;
        }

        // catalog item wasnt created yet
       
        var form = "#UpdateCatalogItemForm";

        // add item
        if ($(form).valid()) {
            rest.update({
                entity: "CatalogItem",
                cntrl: "/Admin/CatalogItem",
                data: $(form).serialize(),
                success: function (data) {
                    $("#noCompError").hide();
                    catalogItemID = data.value;

                    itemID.val(catalogItemID);
                    $("#CatalogItemIDLabel").text(catalogItemID);
                    ops.ForeignID = catalogItemID;
                    openUpdateComponent(catalogItemID);
                }
            }
             
           );
        }
       
    });
    


    $("#UpdateCatatlogItem").live("click", function () {
        $("#noCompError").hide();
        var form = "#UpdateCatalogItemForm";
       

        if ($(form).valid() ) {
            var hasComponetns = $("#componenTable").length;
            if (!hasComponetns) {
                return $("#noCompError").show();
            }
            update($(form).serialize());
        }

    });
    
});
