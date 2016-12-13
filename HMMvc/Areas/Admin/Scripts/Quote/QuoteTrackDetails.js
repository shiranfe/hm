

$(document).ready(function () {
    $("#FollowDate").datetimepicker({
        allowTimes: true,
        //value:'0',
        minDate: '0',//yesterday is minimum date(for today use 0 or -1970/01/01)
    });

    //$("#FollowDate").datepicker({
    //    defaultDate: "+3d",
    //    showButtonPanel: true,
    //    showOn: "both",
    //    buttonImageOnly: false,
    //    buttonText: "",
    //    dateFormat: "dd/mm/yy",

    //});

    // $('#Versions').attr('disabled', 'disabled');

    $(".FileUpload").fineUploader({
        request: {
            endpoint: "/QuoteTrack/UploadAttachment?id=" + $("#QuoteID").val()
        },
        text: {
            uploadButton: "<span class=\"LinkText\">העלה מסמך</span>"
        },
        multiple: false,
        debug: true
    }).on("complete", function (event, id, fileName, responseJson) {
        if (responseJson.success) {
            $("#orderAcceptedError").hide();
            $("#orderAttachmentLink")
                .attr("href", responseJson.folder + responseJson.fileName)
                .text(responseJson.fileName)
                .removeClass("dn");
            $("#OrderAttachment").val(responseJson.fileName);
            $("#deleteAttach").removeClass("dn");
            $.SuccessAlert("העלאה בוצעה בהצלחה");

        }
        else {
            $.BugAlert("something went wrong... try again later");
            console.log(responseJson);
        }
    });


    $("#Message").autosize();

    $("#QuoteTalkDate").datetimepicker({
        allowTimes: true,
        value: '0',
    });
});



