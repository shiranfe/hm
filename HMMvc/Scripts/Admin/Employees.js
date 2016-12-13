
$(document).ready(function () {


    crud({
        entity: "Employee",
        cntrl: "/Admin/Employee",
    });

    crud({
        entity: "EmployeePermision",
        cntrl: "/Admin/EmployeePermision",
        container: "#popUpdateEmployeePermision",
        onlyUpdate:true,
        //subEntity: "Permision"
    });


    $(".openItemPermision").live("click", function () {
 

        $.OpenGeneralPop("UpdateEmployeePermision", "/Admin/EmployeePermision", { "id": $(this).parents("tr").attr("data-id") }, false, "/Admin/EmployeePermision/Update");
   
    });
   
});








