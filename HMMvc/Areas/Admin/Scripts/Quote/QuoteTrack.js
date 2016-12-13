

$(document).ready(function () {
    var form = "#UpdateQuoteTrackForm ";


    function isCurrentStatus(status) {
        return $(form + "#QuoteStatusID").val() == status;
    }

     /** if order accepated must have order number or attachment to proove it*/
    function orderAcceptedNotValid() {
        var orderAccepted = isCurrentStatus(QuoteStatus.OrderAccepted);
        return orderAccepted && !$(form + "#OrderNumber").val() && !$(form + "#orderAttachmentLink").text();
    }


    function orderDoneButNoInvoice() {
        return isCurrentStatus(QuoteStatus.Done)
            && !$(form + "#InvoiceNumber").val();
    }

    function dateNotUpToDate() {
        /** if order is closed no need for date update*/
        if (isCurrentStatus(QuoteStatus.Done) || isCurrentStatus(QuoteStatus.Rejected))
            return false;

        /** follow date not exist or is in the past ?*/
        return !$("#FollowDate").val() || new Date(replaceDate($("#FollowDate").val())) < new Date();

    }

    var ops = {
        entity: "QuoteTrack",
        storageEntity:"Quote",
        cntrl: "/Admin/QuoteTrack",
        customValid: function () {
            $(form +".field-validation-error").hide();
            if (orderAcceptedNotValid()) {
                $("#orderAcceptedError").show();
                return false;       
            }

            if (orderDoneButNoInvoice()) {
                $("#DoneError").show();
                return false;
            }

            if (dateNotUpToDate()) {
                $("#dateError").show();
                return false;
            }

            return true;
        },
        indexFilters: quoteFilters,
        onlyUpdate:true
      
    }

    var actions = crud(ops);

    //function loadPage(dontFilter) {
    //    partialLoad(false, "/Admin/QuoteTrack?dontFilter=" + dontFilter);
    //}

    function deleteAttachment(id) {
        rest.update(
            $.extend({}, ops, {
                data: {
                    QuoteID: id,
                    DeleteAttachment:true
                },
                success: function () {
                    $(form + "#OrderAttachment").val(null);
                    $("#deleteAttach").addClass("dn");
                    $("#orderAttachmentLink").empty().addClass("dn");
                },
                fail: function (responseJson) {
                    $.BugAlert("something went wrong... try again later");
                    console.log(responseJson);
                }
            })
        );
    }


   
    //dontFilter
    /*****************         UI FUNCTIONS          ******************/

    //$("#dontFilter").live("change", function () {
    //    loadPage($(this).is(":checked"));
    //});


   

  

    $("#deleteAttach").live("click", function () {
        deleteAttachment($(this).data("id"));
    });



  
});






