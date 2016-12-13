$(document).ready(function () {


    var ops = {
        entity: "Refubrish",
        cntrl: "/Admin/Refubrish",
       // loadContent: true,
        itemUrl: "/Admin/#/Job/Details?jobType=Refubrish&JobID=",
        indexFilters: [["CreatorID", "-1"], ["RefubrishStatusID", "-1"]]
    }

   
     var actions = crud(ops);

     function loadContent() {
         actions.loadContent();
     }



    /** for mahcine jobs page*/
     function openUpdateRefubrish  (JobID) {

         location.href = ops.itemUrl + JobID;
         //$.OpenGeneralPop('Job', 'Admin', { "JobID": JobID, "jobType": "Refubrish" }, false);
     }

     $.OpenRefubrishPage = function (JobRefubrishPartID) {

         location.href = "/Admin/#/Refubrish/" + JobRefubrishPartID;
     };

     $.DeleteRefubrish = function (JobID) {
         // $("#PointSelectedDiv").html(ajax_load);
         var pg = "/Admin/Refubrish/DeleteRefubrish";
         $.post(pg, {
             "JobID": JobID,
         }, function (data) {
             partialLoad(true);
         }).fail(function () {
             $.BugAlert("העדכון לא בוצע");
         });

     }; 

    /*****************         UI FUNCTIONS          ******************/


     //$("#AddRefubrishBtn").live("click", function () {
    //   openUpdateRefubrish(null);
     //});

     $("#AddJobRequestBtn").click( function () {
         location.href = "/Admin/#/Job/JobRequest";
     });

     $("#openReturningJobBtn").click( function () {
         $.OpenGeneralPop("AddReturningJob", "Admin/Job", null, false);
    });

    

    /** for mahcine jobs page*/
     $(".edit").click(function () {
         var id = $(this).attr("data-id");
         openUpdateRefubrish(id);
     });


     $(".Refubrishname").click(function () {
         var id = $(this).attr("data-id");
         $.OpenRefubrishPage(id);
     });

     $(".deleteRefubrish").click(function () {
         if (confirm("האם אתה בטוח שיהנך מעוניים למחוק את המשתמש?")) {
             var id = $(this).attr("data-id");
             $.DeleteRefubrish(id);

         }
     });


});
