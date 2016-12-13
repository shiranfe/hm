$(document).ready(function () {


    var ops = {
        entity: "Outside",
        cntrl: "/Admin/Outside",
       // loadContent: true,
        itemUrl: "/Admin/#/Job/Details?jobType=Outside&JobID=",
        indexFilters: [["CreatorID", "-1"], ["Zone", "-1"]]
    }

   
     var actions = crud(ops);

    // function loadContent() {
    //     actions.loadContent();
    // }


     

    ///** for mahcine jobs page*/
    // function openUpdateOutside  (JobID) {

    //     location.href = ops.itemUrl + JobID;
    //     //$.OpenGeneralPop('Job', 'Admin', { "JobID": JobID, "jobType": "Outside" }, false);
    // }

    // $.OpenOutsidePage = function (JobOutsidePartID) {

    //     location.href = "/Admin/#/Outside/" + JobOutsidePartID;
    // };

    // $.DeleteOutside = function (JobID) {
    //     // $("#PointSelectedDiv").html(ajax_load);
    //     var pg = "/Admin/Outside/DeleteOutside";
    //     $.post(pg, {
    //         "JobID": JobID,
    //     }, function (data) {
    //         partialLoad(true);
    //     }).fail(function () {
    //         $.BugAlert("העדכון לא בוצע");
    //     });

    // }; 

    /*****************         UI FUNCTIONS          ******************/

     //$(".task").live("click", function () {
     //    var id = $(this).data("id");
     //    console.log(id);
     //});

     //$("#AddOutsideBtn").live("click", function () {
    //   openUpdateOutside(null);
     //});

    // $("#AddJobRequestBtn").click( function () {
    //     location.href = "/Admin/#/Job/JobRequest";
    // });

    // $("#openReturningJobBtn").click( function () {
    //     $.OpenGeneralPop("AddReturningJob", "Admin/Job", null, false);
    //});

    

    ///** for mahcine jobs page*/
    // $(".edit").click(function () {
    //     var id = $(this).attr("data-id");
    //     openUpdateOutside(id);
    // });


    // $(".Outsidename").click(function () {
    //     var id = $(this).attr("data-id");
    //     $.OpenOutsidePage(id);
    // });

    // $(".deleteOutside").click(function () {
    //     if (confirm("האם אתה בטוח שיהנך מעוניים למחוק את המשתמש?")) {
    //         var id = $(this).attr("data-id");
    //         $.DeleteOutside(id);

    //     }
    // });


});
