$(document).ready(function () {

    

  

    /*****************         UI FUNCTIONS          ******************/

   
    function setSumPrice(val) {
        var prc = toNis(val);
        $("#sumPrice").text(prc);

        if (!$("th#sumCol")[0])
            return;
        sumCol = $("th#sumCol")[0].cellIndex + 1;//6
        var tds = $("#componenTable td:nth-child(" + sumCol + ")");
        var totalSum = 0;
        $(tds).each(function () {
            var v = strCurrencyToFloat($(this).text())
            totalSum += v;
        });

        $("#totalSum").text(toNis(totalSum));
    }

    function srcChanged(frstVal) {
        $("#ComponentSrcID").val(frstVal.val());

        var cost = frstVal.attr("data-cost");
        var price = frstVal.attr("data-price");

        if (!price)
            price = cost;
        //change cost
        $("#ComponentCost").text(toNis(cost));
        $("#ComponentPrice").val(price);
        setSumPrice(price);
        $("#Quantity").val(1);
    }


    $("#ComponentTypeID").change(function () {
        
        var typeID = $(this).val();
        var jo = $("#ComponentSrcID").find("option");
        if (this.value == "") {
            jo.show();
            return;
        }
        //hide all the rows
        jo.hide();

        //Recusively filter the jquery object to get results.
        jo.filter(function (i, v) {
            return $(this).attr("data-type") == typeID;
        }).show();


        /// change ComponentSrcID
        var frstVal = $("#ComponentSrcID option:visible:first");
        srcChanged(frstVal);

        $("#ComponentSrcID").parent().find(".SelectVal").text(frstVal.text());//doesnt changed by itself...


    });



    $("#ComponentSrcID").change(function () {
        var optionSelected = $("option:selected", this);
    
        srcChanged(optionSelected);
    });

    $("#ComponentPrice, #Quantity").keyup( function () {
        setSumPrice($("#ComponentPrice").val() * $("#Quantity").val());

    });
    


   

});
