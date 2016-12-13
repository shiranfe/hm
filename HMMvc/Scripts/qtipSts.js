
$(document).ready(function () {
    $(".MacStsIcon").qtip({
        content: {
            text: function (api) {
                return $(this).attr("data-txt");
            }
        },
    });
});