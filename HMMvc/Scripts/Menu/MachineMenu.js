

$(document).ready(function () {

    //window.location.pathname === "/" ? "/VB/CurrentSts" : 
    var url = getPath(true,true);
    
    var currentNav = $("#SideMenuAccordion").find("a[href='" + url + "']");
    $(currentNav).addClass("current");
    //$(".mobileMenuBtns h1").text($(currentNav).text());
    var uiNode = $(currentNav).parents("ul");

    
    var index = $("#SideMenuAccordion ul").index(uiNode);


        $("#SideMenuAccordion").accordion({
            heightStyle: "content",
            navigation: true,
            active: index
        });


});








