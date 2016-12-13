
$(document).ready(function () {

    var pageNumber = 1;
    //jss["MachineVBTab"] = true;
    var direction = false;
    var sort; //hasNextPage,hasPrevPage, pageCount;
    
    
     function getData() {
        //Direction = Sort != $('#SortByCombo').val() ? false : !Direction;
        $("#CurrentStsContent").html(ajax_load);
        var pg = "/VB/CurrentStsContent";
        $.get(pg, {
            "Sort": $("#SortByCombo").val(),
            "Direction": direction,
            "Filter": $("#SrchBox").val(),
            "Page": pageNumber
        }, function (data) {
            $("#CurrentStsContent").html(data);
            hasNextPage === "True" ? $(".NextBtn").show() : $(".NextBtn").hide();
            hasPrevPage === "True" ? $(".PrevBtn").show() : $(".PrevBtn").hide();
           
            //$("#PageCount ").html(pageCount);
        }).fail(function () { $.BugAlert(); });
        sort = $("#SortByCombo").val();        
    }

    getData();


    function changePage() {
        $(".PageNum").val(pageNumber);
        getData();
    }
    /*****************         UI FUNCTIONS          ******************/

    $(".SrchBtn").live("click", function () {
        pageNumber = 1;
        getData();
    });

    $(".SearchTextbox").live("change", function () {
        pageNumber = 1;
        getData();
    });

    $(".NextBtn").live("click", function () {
        changePage(pageNumber++);
       
    });

    $(".PrevBtn").live("click", function () {
        changePage(pageNumber--);
       
    });

    $(".PageNum").live("change", function () {
        var newPage = parseInt($(".PageNum").val());
        if (newPage <= parseInt($("#PageCount").text()) && newPage > 0) {
            pageNumber = newPage;
            changePage();
        } else {
            $(".PageNum").val(pageNumber);
        }
           
    });

    $("#SortByCombo").live("change", function () {  
        getData();
    });

    $(".OpenMacVbBtn").live("click", function () {
        var url = "../Machine/MachinePage?ActiveTab=VB&MachineID=" + this.attributes["data-id"].value;
        location.href =url;
    });

   

   

    //$(document).ready(refreshContent());
});








