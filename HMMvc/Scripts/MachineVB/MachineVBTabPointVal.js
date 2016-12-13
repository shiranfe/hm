
$(document).ready(function () {


   



    function timeConverter(UNIX_timestamp) {
        var date = new Date()
        
        var yourDateObject = date.getTime();

        var a = new Date(UNIX_timestamp);
  
        var year = a.getFullYear();
        var month =a.getMonth()+1;
        var date = a.getDate();
                   
        var time = date + "/" + month + "/" + year;
        
        return time;
    }

    /*****************         UI FUNCTIONS          ******************/




    $("#StartDate").datepicker({
        defaultDate: "-1y",
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        dateFormat: "dd/mm/yy",
        onClose: function (selectedDate) {
            $("#EndDate").datepicker("option", "minDate", selectedDate);
        }
    });

    $("#EndDate").datepicker({
        defaultDate: 0,
        showButtonPanel: true,
        showOn: "both",
        buttonImageOnly: false,
        buttonText: "",
        maxDate: 0,
        dateFormat: "dd/mm/yy",
        onClose: function (selectedDate) {
            $("#StartDate").datepicker("option", "maxDate", selectedDate);
        }
    });



    function showTooltip(x, y, contents) {
        $("<div id='tooltip' dir='ltr' class='qtip qtip-default qtip-Black qtip-rounded qtip-shadow qtip-pos-bc'>" +
            contents + "</div>").css({
                position: "absolute",
                display: "none",
                top: y - 30,
                left: x + 5,
                //border: "1px solid #fdd",
                padding: "5px",
                //"background-color": "#fee",
                opacity: 0.80
            }).appendTo("body").fadeIn(200);
    }

    var previousPoint = null;
    $("#placeholder").bind("plothover", function (event, pos, item) {


        if (item) {
            if (previousPoint != item.dataIndex) {

                previousPoint = item.dataIndex;

                $("#tooltip").remove();
                var val = item.datapoint[1].toFixed(2);
                //var a = new Date( item.datapoint[1].toFixed(2));

                var dt = timeConverter(item.datapoint[0]);

                //dt=item.datapoint[1].toFixed(2);
                showTooltip(item.pageX, item.pageY,
                     dt + " , " + val);
            }
        } else {
            $("#tooltip").remove();
            previousPoint = null;
        }

    });


    $(".PointValueRow").unbind().click(function () {
        $(this).toggleClass("Selected");
        $.LoadPointValue($(this).attr("data-id"));
        //$.Scroll("#VbTabPointVal");
    });
   
    $(".DatePicker").unbind().change(function () {
        var id = $(".PointValueRow.Selected").attr("data-id");

        $.LoadPointValue(id);
        //$.Scroll("#VbTabPointVal");

    });


});








