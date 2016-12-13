$(document).ready(function () {

   
    //$.showRow = function ($row, srchText) {
    //    var srchTerm = $row.text().toLowerCase().indexOf(srchText.toLowerCase()) > -1;

    //    return srchTerm;

    //};
 
    crud({
        entity: "CatalogItem",
        cntrl: "/Admin/CatalogItem",
        indexFilters:true
    });

   

    /*****************         UI FUNCTIONS          ******************/


   

  


    


    //$('.AddRow').live('click', function () {
    //    var lastRow = $('#componenTable:last');
    //    var odd = $(lastRow).hasClass("even") ? "odd" : "even";

    //    var typePick = '<select name="ComponentTypeID">' +
    //                       '<option value="1" selected="selected">Material</option>' +
    //                        '<option value="2">Outsource</option>' +
    //                        '<option value="3">Personnel</option>' +
    //                    '</select>';
    //    var srcPick = '';
    //    var scrCost = '';
    //    var defPrice = scrCost * 1.2;



    //    var rowHtml =
    //        '<tr class="' + odd + '">' +
    //        '    <td><span>' + typePick + '</span></td>' +
    //        '    <td><span>' + srcPick + '</span></td>' +
    //        '    <td nowrap>' + scrCost + '</td>' +
    //        '    <td nowrap><input type="text" data-val="true" data-val-required="(*)" class="" value="' + defPrice + '" name="ComponentPrice"></td>' +
    //        '    <td><span><input type="text" data-val="true" data-val-required="(*)" class="" value="1" name="Quantity"></span> </td>' +
    //        '    <td><b>' + defPrice + '</b></td>' +
    //        '    <td><span class="linktext addComponent">הוסף</span></td>' +
    //        '</tr>';

    //    $('#componenTable').append(rowHtml);

    //});




});
