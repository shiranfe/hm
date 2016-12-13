var arr = [];

$(document).ready(function () {


    $.AddToFilter = function (sts, stsval) {
        if ($("#FilterNone").is(":visible")) {
            $("tr.trrow").hide();
            $("#FilterExist").show();
            $("#FilterNone").hide();
            $("#FilterValue")[0].innerHTML = stsval
        }
        else {
            $("#FilterValue")[0].innerHTML += "," + stsval
        }
        $("tr.trrow[data-sts=" + sts + "]").show();

     
        //var Htmurl = $('.ReportIcon').attr('href');
        //$('.ReportIcon').attr('href', url + '&ids=' + sts);
    };

    $.RemoveFromFilter = function (sts, stsval) {
        $("tr.trrow[data-sts=" + sts + "]").hide();
        var filtval = $("#FilterValue")[0];
        filtval.innerHTML=filtval.innerHTML.replace("," + stsval, "");
        filtval.innerHTML=filtval.innerHTML.replace(stsval, "");

        if (arr.length==0) {
            $("tr.trrow").show();
            $("#FilterExist").hide();
            $("#FilterNone").show();
            //arr = [];
        }
       
    };


    function updateHref() {
        var arrParams = "";

         $.each(arr, function (index, value) {
            arrParams += "&ids=" + value;
        });

        $(".ReportIcon").each(function (index, item) {
            var current = $(item).attr("href").split("&")[0];
            $(item).attr("href", current + arrParams);
          //  console.log($(item).attr("href"));
        });
    }
    /*****************         UI FUNCTIONS          ******************/

    $(".VBReportMikraStsDiv").live("click", function () {
        var sts = $(this).attr("data-sts");
        var ind = arr.indexOf(sts)
        if (ind == -1) {
            //not int list 
            arr.push(sts);
            $.AddToFilter(sts, this.children[1].innerHTML);
        }
        else {
            arr.splice(ind, 1);
            $.RemoveFromFilter(sts, this.children[1].innerHTML);
        }
        updateHref();
       
    });


    $("#RemoveFilter").live("click", function () {
        $("tr").show();
        $("#FilterExist").hide();
        $("#FilterNone").show();
        arr = [];
       updateHref();
      
    });

    //$(".ReportIcon").live("click", function () {
    //    var met = "VBReportHtml";
    //    if ($(this).hasClass("ExcelIcon"))
    //        met = "VBReportExcel";
    //    else if ($(this).hasClass("PdfIcon"))
    //        met = "VBReportPdf";
  
    //    var url = "/Export/" + met + "?id=" + JobID ;
    //    if (arr.length > 0) {
    //        $.each(arr, function (index, value) {
    //            url+= "&ids=" +value;
    //        });
    //    }
    //   // window.open(url);
      
    //});




});








