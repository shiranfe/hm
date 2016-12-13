

$(document).ready(function () {
   

    var clientAddId = -2;
    var IsAdminZone = area == 'Admin';

    $.LoadClient = function (ClientID) {
        var url = IsAdminZone ? "/Admin/Home/ChangeAdminClient" : "/User/ChangeClient";

        $('.sitesubcontent').html(ajax_load);
        $.post(url, {
            "ClientID": ClientID

        }, function (data) {
            //location.reload();
            //partialLoad();
            refreshPage();
        });
    };

    function addQuickClient(clientName,btn ) {
        $(btn).addClass("disabled");

        rest.update(
               {
                   entity: "Client",
                   cntrl: "/Admin/Client",
                   data: {
                       ClientName: clientName,
                       ClientID: clientAddId
                   },
                   success: function (client) {
                       $.addNewClientToTree(client);
                       $(btn).removeClass("disabled");
                       $.SuccessAlert("לקוח נוצר בהצלחה");
                   }
               }
            );
    }


    function SetSelectedTree(pop, curClientID) {
        var container = $(pop +" .JobClientDrop");
        var selectedClient = $(container).find("a#node-" + curClientID).text();
      //  log(selectedClient);
        setSrchClient(selectedClient);
        setTimeout(function () {
            $(container).find('.ClientTreeDiv').scrollTo($('#node-' + curClientID), 700);
        }, 750);

    }



    function setSrchClient( str) {
        var container = $(".JobClientDrop").last();
        $(container).find('#srchClient').val('');
        $(container).find('#srchClient').attr('placeholder', str);
    }


    $.initTree = function (container, id) {
       
        SetSelectedTree(container, id);
    };

    /*****************         UI FUNCTIONS          ******************/
 
       
  

    $("#t li a:not(.disabled)").live('click', function () {
        var clientID = $(this).attr('data-id');
     
        if (clientID > 0) {
            setSrchClient($(this).text());
            $(this).parents('.list').hide();
        }
        else if (clientID == clientAddId) {
            var newClientName = $("#srchClient").val();
            /** add client btn from filter list*/
            if (allowNotInList)
                addQuickClient(newClientName, "#" +$(this).parents("form").attr("id")  + " #node-"+clientAddId);
            else 
                $.openUpdateClient({ ClientName:newClientName });
        }
     

    });

    $("#ClientTreeMenu #t li a").live('click', function () {
        var clientID = $(this).attr('data-id');
       // if (clientID > 0)
             $.LoadClient(clientID);
    });

    //var listFocused = false;
    //$(".JobClientDrop .list").live('click', function () {
    //    listFocused = true;
    //}).blur(function (e) {
    //    console.log("blured");
    //    listFocused = false;
    //});

    //$('#srchClient').live('blur', function (e) {
    //    var container = $(this).parents(".filterBox");

    //    setTimeout(function () {
    //        if (!listFocused) {
    //            $.autoCompSelected(container);
    //        }
    //        else {
    //            listFocused = false;
    //        }

    //    }, 200);

    //});



    $('#srchClient').live('keyup', function () {
        var srch = $(this).val();
        $(this).parents('.filterBox').find('.list').show();
        typewatch(this,function () {
            filterClients(srch);
        }, 200);
    


    });


    function filterClients(srch) {
        showSmallLoading($('#srchClient'));
        var lis = '#t li';
        if (srch == '') {
            $('#t').jstree("close_all", -1).jstree("open_node", '#node-0');
            return $(lis).show();
        }

        var str = sanitize(srch);

        
        $(lis).hide();
        $('#t').jstree("open_all", -1);
        var rootLi = $(lis).find("[data-id=0]").parent();
        if (rootLi) {
            rootLi.show();/** allways show root*/
        }
 

        $(lis).filter(function (index) {
           // var san = ; 
            //if (san.indexOf(str) > -1)
            //    console.log(san);
            return sanitize($(this).find('> a').data('name')).indexOf(str) > -1;
        }).show().find("li").show(); /**show also all child in tree*/

        /** show no resualts or add (^ - begins with "-")*/
        if ($(lis + ":visible").length == 0)
            $(lis + " [data-id=-1]").parent().show(); 

        if ($('#srchClient').val())
            $(lis + " [data-id=-2]").parent().show();

        hideSmallLoading($('#srchClient'));
    }


}); 