

$(document).ready(function () {

    var NewMergeID;
    var OldMergeID;
    function mergeClients() {
        rest.update(
               {
                   entity: "Client",
                   cntrl: "/Admin/Client/MergeClients",
                   data: {
                       "NewMergeID": NewMergeID,
                       "OldMergeID": OldMergeID,
                   },
                   success: function (data) {
                       $(".ClientMerge[data-id='" + OldMergeID + "']").remove();

                       closePopup("popMergeClients");
                       refreshPage();

                       $(".ClientMerge").removeClass("selected").removeClass("disabled");
                       clearFilterClients();
                       $.SuccessAlert("מיזוג בוצע בהצלחה");

                       //NewMergeID = OldMergeID = null;
                       //$("#NewText").text("(בחר לקוח)");
                       //$("#OldText").text("(בחר לקוח)");

                      
                   }
               }
            );

    }


    function filterClients(that) {
        var jo = $(that).next().find(".ClientMerge");
        var srchText = sanitize($(that).val());

        typewatch(that, function () {
            if (this.value == "") {
                return jo.show();
            }
            //hide all the rows
            jo.hide();

            //Recusively filter the jquery object to get results.
            jo.filter(function (i, row) {
                return sanitize($(row).text()).indexOf(srchText) > -1;
            }).show();

        }, 200);
    }

    function clearFilterClients() {
        $(".filter-merge").val("");
        $(".ClientMerge").show();
    }

    /*****************         UI FUNCTIONS          ******************/


    $("#OldMacs .ClientMerge").click(function () {
        if ($(this).hasClass("disabled"))
            return;
        $("#OldMacs .ClientMerge").removeClass("selected");
        $(this).addClass("selected");

        OldMergeID = this.getAttribute("data-id");
        $("#NewMacs .ClientMerge").removeClass("disabled");
        $("#NewMacs .ClientMerge[data-id='" + OldMergeID + "']").addClass("disabled");


        $("#OldText").text($(this).text());
    });

    $("#NewMacs .ClientMerge").click(function () {
        if ($(this).hasClass("disabled"))
            return;

        $("#NewMacs .ClientMerge").removeClass("selected");
        $(this).addClass("selected");

        NewMergeID = this.getAttribute("data-id");
        $("#OldMacs .ClientMerge").removeClass("disabled");
        $("#OldMacs .ClientMerge[data-id='" + NewMergeID + "']").addClass("disabled");

        $("#NewText").text($(this).text());
    });

    $("#mergeClientsBtnConfirm").click(function () {
        if (confirm("האם אתה בטוח כי הינך מעוניין למזג את הלקוחות?")) {
            mergeClients();
        }
    });


    $(".filter-merge").keyup(function () {
        filterClients(this);
    });
});








