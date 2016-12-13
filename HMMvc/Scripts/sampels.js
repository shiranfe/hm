$.DoBCAction = function (meth, arr) {
    $.ajax({
        type: "POST",
        url: "/Contact/" + meth,
        dataType: "json",
        traditional: true,
        data: {
            BlockID: $("#BlockID").val(),
            ids: arr
        }

    }).done(function (data) {

        if (data.res == "1") {
            // do something
        }
    });;
};


$(".bc_div").toggleClass("bcthumb").toggleClass("bclistview");



$(".ContStageCardPic").each(function (index) {
    var src = $(this).attr("src");
    src = (s == "s1") ? src.replace("s1", "s3") : src.replace("s3", "s1");
    $(this).attr("src", src);
});



var stsid = $(this).attr("id").replace("StsAction-", ""); // get id from div here id ="StsAction-35"

// go to another page (with session in history)
var url = "Block/Block?BlockID=" + blockid;
location.href =url;

//refresh page
location.reload();