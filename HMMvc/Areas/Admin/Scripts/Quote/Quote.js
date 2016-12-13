

$(document).ready(function () {

    try {
        $.validator.setDefaults({ ignore: null });
    } catch (e) {

    }

    /** if is editing from update page need to load it*/
    if (isInPage('/Admin/QuoteVersion/Update')) {
        var ops = {
            entity: "Quote",
            cntrl: "/Admin/Quote",
            onlyUpdate:true,
        }

        crud(ops);
    }

    /*****************         METHODS          ******************/

    var form = "#UpdateQuoteForm";
    $(form).validate().settings.ignore = "";

    /** before can link job need to save quote*/
    function addQuote(params) {
        rest.update(
        {
            entity: "Quote",
            cntrl: "/Admin/Quote",
            data: params,
            success: function (model) {
                   
                $(form).find("#QuoteID").val(model.QuoteID);
                       
                openUnQuotedJobs($(form).find("#ClientID").val());
            }
        });
    }


    function openUnQuotedJobs(clientID) {
        $.OpenGeneralPop("JobsUnQuoted", "Admin/Job", { ClientID: clientID }, false);
    }

    function updateJobQuote(jobID, QuoteID) {
        rest.post({
            url: "/Admin/Job/UpdateJobQuote",
            data: {
                QuoteID: QuoteID,
                JobID: jobID
            },
            success: function (data) {

                loadQuoteJobs(QuoteID);
            }
        });
    }

    function loadQuoteJobs() {

        rest.get({
            url: "/Admin/QuoteJob/Index",
            data: {
                QuoteID: $(form + " #QuoteID").val()
            },
            htmlSuccessTo: "#quoteJobsDiv"
        });
    }

    function clientChanged(clientID) {
        $("#ClientID").val(clientID).valid();

        $.LoadContacts(clientID, "#UserID");
        $(".plusBtn.EditClientBtn").attr("data-id", clientID);
        //$.LoadClientUnQuotedJobs(ClientID);
    }

    $.clientUpdated = function (clientID) {
        clientChanged(clientID);
    };

    /*****************         UI FUNCTIONS          ******************/


    //$.ClientCreated = function (model) {
    //    var ClientID = model.ClientID;


    //    clientChanged(ClientID);
    //};

    $(form + " .JobClientDrop #t li a").live("click", function () {
        var ClientID = $(this).attr("data-id");

        clientChanged(ClientID);
    });

    $("#LinkJobBtn").live("click", function () {
        var clientID = $(form + " #ClientID").val();
        var quoteId = $(form + " #QuoteID").val();
        if (clientID > 0 && quoteId > 0 ) {
            return openUnQuotedJobs(clientID);
        }

        $(form + " #ClientID").valid();
        if ($(form).valid()) {
            addQuote($(form).serialize());
        }

    });


    $("#popJobsUnQuoted .linkJob").live("click", function () {
        var jobID = $(this).attr("data-id");
        var quoteID = $(form + " #QuoteID").val();

        updateJobQuote(jobID, quoteID);
        $(this).removeClass("linktext").removeClass("add").text("העבודה שויכה");

        closePopup("popJobsUnQuoted");
    });

    $(".removeJob").live("click", function () {
        var jobID = $(this).attr("data-id");

        updateJobQuote(jobID, null);
        $(this).parents("tr").remove();
    });


    $("#QuoteJobDetails").live("click", function () {
        getUnLinkedClientJobs();
    });



});








