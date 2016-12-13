
$(document).ready(function () {

  


    /*****************         UI FUNCTIONS          ******************/

    //$("#tags").autocomplete({
    //    source: JobTemplateNotesIL,
    //});
  


    $("#GeneralNoteIL, #GeneralNoteEN").editInPlace({
        field_type: "textarea",
        textarea_rows: 2,
        afterCreateEdior: function () {
            var Lang = $(this).attr("id").replace("GeneralNote", "");
            var nots = "#" + $(this).attr("id");
            $(nots + " textarea").autocomplete({
                source: Lang == "IL" ? JobTemplateNotesIL : JobTemplateNotesEN,
             });
        },
        postclose: function () {
            if( $(this).text()=="")
                $(this).text("לחץ להוספה");
        },
        callback: function (unused, enteredText) {


            var Lang = $(this).attr("id").replace("GeneralNote", "");
            $.post("/Admin/VB/UpdateVBGeneralNote",
                {
                    "MachineID": MachineID,
                    "JobID": JobID,
                    "GeneralNote": enteredText,
                    "Lang": Lang,
                },
            function (data) {
               
            });
            return enteredText;


        },
        // url: "./server.php",
        bg_over: "#cff"

    });
    

  
});








