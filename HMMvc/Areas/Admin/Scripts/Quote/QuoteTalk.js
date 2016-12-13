

$(document).ready(function () {

   

    var ops = crud({
        entity: "QuoteTalk",
        cntrl: "/Admin/QuoteTalk",
        container: ".talks-container",
        onlyUpdate: true,
        success: function (message) {
            //console.log(message);
            if (message.deleted) {
                $('[data-id="' + message.id + '"').remove();

            }
            else {
                addMessageToDom(message);

                $("#Message").val("");
            }


        }
    });

  
    function addMessageToDom(message) {



        var template = //$(".past .talk-row").first().clone();
            '<div class="talk-row" data-id="' + message.QuoteTalkID + '">' +
                '<div class="talk-div corn_3 ">' + 
                    '<div class="talk-name">' +
                        '<span class="creator">' + message.Creator + ' </span>' +
                        '<span class="talkDate">' + message.TalkDate + '</span>' +
                        '<span class="deleteItem linktext"> X </span>' +
                    '</div>' + 
                    '<div class="b">' + 
                       ' <pre class="message">' + message.Message + '</pre>' +
                   ' </div>' + 
                '</div>' + 
            '</div>';

        //$(template).find(".creator").text(message.Creator);
        //$(template).find(".talkDate").text(message.TalkDate);
        //$(template).find(".message").text(message.Message);

        $(".past").prepend(template);
    }
    //dontFilter
    /*****************         UI FUNCTIONS          ******************/


    //$(".talk-div.new input").live("click", function () {
    //    addMessage($("#NewMsg").val());
    //});

    $(".talk-row .deleteItem").live("click", function () {
        var id = $(this).parents("[data-id]").attr('data-id');
        ops.remove(id);
    });
});






