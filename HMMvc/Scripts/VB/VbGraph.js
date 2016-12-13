

var i = 0;
$.each(datasets, function (key, val) {
    val.color = i;
    ++i;
});

//debugger;

var choiceContainer = $("#PointSelectedDiv"); // $("#PointChecks");

/*

$.each(datasets, function (key, val) {
    var b = val.label == '@Model.ValueName' ? 'b' : '';
    var vsel = val.label == '@Model.ValueName'
                     ? "checked='checked' disabled='disabled'" : "";
    choiceContainer.append(
        '<div class="GraphNavigator">' +
        '<input type="checkbox" ' + vsel + ' name="' + key + '"  id="id' + key + '"/><span class=' + b + '>' + val.label + '</span>' +
        '</div>');

});


choiceContainer.find("input").click(plotAccordingToChoices);
*/
function plotAccordingToChoices() {

    var data = [];

    //choiceContainer.find("input:checked").each(function () {
    //    var key = $(this).attr("name");

    choiceContainer.find(".PointValueRow.Selected .PointValuesName").each(function () {
        var key = $(this).text();

        if (key && datasets[key]) {
            data.push(datasets[key]);
        }

    });
    // debugger;
    if (data.length > 0) {
        $.plot($("#placeholder"), data, {
            xaxis: {
                mode: "time",
                timezone: "browser",
                minTickSize: [1, "month"],
                //timeformat:  "%y,%m,%d"
                /* min: (new Date(1999, 0, 1)).getTime(),
                 max: (new Date(2000, 0, 1)).getTime() */
            },
            series: {
                lines: {
                    show: true,

                },
                points: {
                    show: true
                },

            },
            grid: {
                hoverable: true,
                clickable: true
            },
        });
    }
}

plotAccordingToChoices();

$("#PointSelectedDiv .PointValueRow").click(function () {
    plotAccordingToChoices();

});