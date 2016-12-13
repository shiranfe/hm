
$(document).ready(function () {

    $("#UserMenuAccordion").accordion({
        heightStyle: "content"
        //collapsible: true
    });

    $("#UserMenuAccordion").accordion("option", "active", 0);

    var url = window.location.pathname;//.toLowerCase();
    $("#UserMenuAccordion").find("a[href='" + url + "']").addClass("current");
    /*****************         UI FUNCTIONS          ******************/

});

//jQuery(document).ready(function ($) {
//    jQuery('#accordion').dcAccordion();
//});







