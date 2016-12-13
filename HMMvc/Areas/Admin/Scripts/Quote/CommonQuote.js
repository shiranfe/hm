var QuoteStatus = {
    WatingForQuote: 541,
    InProccess: 543,
    WatingForClient: 544,
    OralApproved: 545,
    OrderAccepted: 546,
    Rejected: 547,
    Done: 590
};


var quoteFilters= [
            ["isCover",""],["CreatorID","-1"],["QuoteStatusID","-1"]       
];

//function onFilterChange(saveToStorage) {
//    saveToStorage({
//        isCover: $("#isCover").val(),
//        CreatorID: $("#CreatorID").val(),
//        srch: $("#srch").val(),
//        QuoteStatusID: $("#QuoteStatusID").val()
//    });
// }

//function onPageLoad(data) {
   
//    $("#srch").val(data.srch);
//    /** NEED TO RE POPULATE DROPS*/
//    $.setAutoCompleteVal("#isCover", data.isCover || "");
//    $.setAutoCompleteVal("#CreatorID", data.CreatorID || "-1");
//    $.setAutoCompleteVal("#QuoteStatusID", data.QuoteStatusID || "-1");
//}


//function getShowByStatus($row){
//    var status = $("#QuoteStatusID").val();
//    if( !status || status == "-1" )
//        return true ;

//    var rowStatus = $row.attr("data-status");

//    /** open quotes*/
//    if (status == "100")
//        return rowStatus != QuoteStatus.Done && rowStatus != QuoteStatus.Rejected;

//    /** closed quotes*/
//    if (status == "101")
//        return rowStatus == QuoteStatus.Done || rowStatus == QuoteStatus.Rejected;

//    return rowStatus === status;

//}


//function showRow($row) {
            
//    var showisCover = !$("#isCover").val() ? true :
//        ($("#isCover").val() == "cover" ? $row.attr("data-iscover") === "True"
//       : $row.attr("data-iscover") === "False");
        
//    var showByEmp = !$("#CreatorID").val() || $("#CreatorID").val() == "-1" ? true :
//    $row.attr("data-emp") === $("#CreatorID").val();

//    var showByStatus =getShowByStatus($row);


//    return showisCover && showByEmp && showByStatus;

//}




/*****************      QUOTEVERSION   METHODS          ******************/

var quoteVersionOps = {
    entity: "QuoteVersion",
    cntrl: "/Admin/QuoteVersion",

}

$.initVersion = function () {
    // $('#Terms').autosize();
    //$("#Appendices").autosize();
    CKEDITOR.disableAutoInline = true;
    try {
        CKEDITOR.inline("Appendices");
        CKEDITOR.inline("Terms");
    } catch (e) {

       
    }
    
    for (var i in CKEDITOR.instances) {
       // console.log(CKEDITOR.instances[i].name);
        CKEDITOR.instances[i].on('blur', saveVersion);
    }

    calcSum();
};

function calcSum() {
   
    var sum = strCurrencyToFloat($("#VersionSum").text());
    var disccount = $("#Disscount").val();
    var sumAfter = sum * (1 - disccount);
    $("#SumAfterDiscount").text(toNisRound(sumAfter));

    var vat = parseFloat($("#Vat").text()) / 100;
    var vatMoney = vat * sumAfter;
    $("#VatMoney").text(toNisRound(vatMoney));

    var sumWithVat = sumAfter + vatMoney;
    $("#SumAfterVat").text(toNisRound(sumWithVat));
}


function openQuoteDetails(quoteID) {
    $.OpenGeneralPop("UpdateQuote", null, { id: quoteID }, false, "/Admin/Quote/Update");
}

function openJobDetails(id) {
    $.OpenGeneralPop("JobDetails", null, { JobID: id }, false, "/Admin/QuoteJob/JobDetails");
}

function importFromQuote(id) {
    var params = JSON.parse(localStorage.getItem("Quote"));
    params.DestVersionID = id;

    $.OpenGeneralPop("ImportFromQuote", null, params, false, "/Admin/Quote/ImportFromQuote");
}

function openVersion(versionID) {
    location.href = "/Admin/#/QuoteVersion/Update?id=" + versionID;
}

function add(data) {
    rest.copy(
        $.extend({
            data: data,
            success: function (data) {
                openVersion(data.NewVersionID);
            }
        }, quoteVersionOps)
    );
}

function update(data) {

    rest.update(
       $.extend({
           data: data
       }, quoteVersionOps)
   );
}

function saveVersion() {
    if ($("#VersionDate").valid() && $("#Disscount").valid()) {
        var param = {
            VersionDate: $("#VersionDate").val(),
            Disscount: $("#Disscount").val(),
            Terms: CKEDITOR.instances.Terms.getData(),
            Appendices: CKEDITOR.instances.Appendices.getData(),
            QuoteVersionID: $("#QuoteVersionID").val(),
            QuoteID: $("#QuoteID").val(),
            Version: $("#Version").val(),
        }

        update(param);
    }


    ////close all items editor so wont show in validation
    //$("tr").removeClass('open');
    //$(".options").hide();

    //var form = '#UpdateQuoteVersionForm';
    //if ($(form).valid()) {
    //    update($(form).serialize());
    //}

}


function openEmail() {

    $.OpenGeneralPop("QuoteEmail", null, { id: $("#QuoteVersionID").val() }, false, "/Export/QuoteEmail", true);

}



$(".openTrack").click(function () {
   
    var id = $(this).data("id") || $(this).parents("tr").data("id");
    $.OpenGeneralPop("UpdateQuoteTrack", "Admin/QuoteTrack", { id: id }, false, "/Admin/QuoteTrack/Update");
});

// getDollar();

// getDollarRate();


/*****************     QUOTEVERSION    UI FUNCTIONS          ******************/