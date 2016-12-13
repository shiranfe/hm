
function showPopup(id) {
    $(id + ' #cropImg').val(3);

    $(id).bPopup();
    //$(id).show();
}

var cord;


//   end document ready
var jcrop_apis = [];


var cropPic = function (options) {

    var bounds;
    var jcrop_api, boundx, boundy;

    var popContainer = options.popId;
    var imageToCrop = options.imageToCrop;
    var imageCrop = $(imageToCrop);
    var cropedContainer = options.popId + " .jcrop-holder";
    //var hfX = $(popContainer + " #hfX");
    //var hfY = $(popContainer + " #hfY");
    //var hfHeight = $(popContainer + " #hfHeight");
    //var hfWidth = $(popContainer + " #hfWidth");

    //function releaseCheck() {
    //    jcrop_api.setOptions({
    //        allowSelect: true,
    //        allowResize: true,
    //        allowMove: true

    //    });
    //    jcrop_api.setSelect([0, 0, 500, 500]);
    //    //jcrop_api.setSelect(jcrop_api.getBounds());

    //};

    //function updateCoords(c) {
    //    //hfX.val(c.x);
    //    //hfY.val(c.y);
    //    //hfHeight.val(c.h);
    //    //hfWidth.val(c.w);

    //    cord.x = c.x;
    //    cord.y = c.y;
    //    cord.w = c.w;
    //    cord.h = c.h;

    //    console.log(c.h);
    //    if (!jcrop_api)
    //           throw "jcrop_api was not found for "  + imageToCrop; 

    //    bounds = jcrop_api.getBounds();
    //    boundx = bounds[0];
    //    boundy = bounds[1];

    //}

    //if (typeof $(imageToCrop)[0].naturalWidth != "undefined") {
    //    //  NO AVATAR UPLOADED
    //    $(imageToCrop)[0].src = "";
    //}


    //function setCropVals() {
       


    //    jcrop_api = jcrop_apis[imageToCrop];

    //    if (!jcrop_api)
    //        throw "jcrop_api was not found for "  + imageToCrop;

    //    jcrop_api.setImage(options.path);

    //    // Use the API to get the real image size
    //    bounds = jcrop_api.getBounds();
    //    boundx = imageCrop.width();
    //    boundy = imageCrop.height();

    //    // Store the API in the jcrop_api variable

    //    //jcrop_api.animateTo([0, 0, 415, 415]);
    //    //  console.log(jcrop_api);


    //    //options.imgLoaded();

    //    //if (hfX.val() == 'NaN') {
    //    // jcrop_api.animateTo([270, 120.5, 415, 415]);
    //    jcrop_api.animateTo([0, 0, 2000, 2000]);
    //    //}
    //    //else {
    //    //  jcrop_api.animateTo([hfX.val(), hfY.val(), hfWidth.val(), hfHeight.val()]);
    //    //}

    //    showPopup(options.popId);
    //}

    function getImgRatio(type) {

        switch (type) {
            case "ClientLogos":
            case "PointResualt":
            case "Job":
                return  0 ;
            default:
               return  4 / 3 ;
        }
    }


    imageCrop.cropper({
        aspectRatio: getImgRatio(options.ObjType),
        autoCropArea:1,
        crop: function (c) {
            cord.x = parseInt(c.x);
            cord.y = parseInt(c.y);
            cord.w = parseInt(c.width);
            cord.h = parseInt(c.height);
            showPopup(options.popId);
        }
    });

    jcrop_apis[imageToCrop] = imageCrop;
    //var naturalHeight = imageCrop[0].naturalHeight,
    //    naturalWidth = imageCrop[0].naturalWidth;

    //console.log(naturalHeight);


    /** create crop and pp*/
    //imageCrop.Jcrop({
    //    onSelect: updateCoords,
    //    onChange: updateCoords,
    //    //onMove: updateCoords,
    //    onRelease: releaseCheck,
    //    touchSupport:true,
    //aspectRatio: getImgRatio(options.ObjType),
    //    allowResize: true
    // //   trueSize: [naturalWidth, naturalHeight]
    //    //boxWidth:  $(imageToCrop).width(),
    //    //boxHeight:  $(imageToCrop).height()
    //}, function () {

    //    // var paddingRight = (556 - boundx) / 2;
    //    //var newW = 556 - paddingRight;
    //    // var paddingTop = (381 - boundy) / 2;
    //    // var newH = 381 - paddingTop;


    ////    this.setOptions(opt);


     //   jcrop_apis[imageToCrop] = this;
    //    setCropVals();


    //});


    /////** re open popup and crop*/
    //if (jcrop_apis[imageToCrop]) {
    //    return;
    //}

    $(popContainer + ' .UploadAndCropBtn').click(function () {
        options.updateAvatar();

    });

    $(popContainer + ' .CancelUploadBtn').click( function () {
        options.delTempPic();
    });




}



var picUpload = function (options) {

    cord = {
        x: null,
        y: null,
        w: null,
        h: null
    };

    //console.log(options.htmlId);
    var defualts = {
        ObjID: null,
        ObjType: null,
        FilePre: null,
        SubMode: false,
        htmlId: null
    };

    var ops = $.extend({}, defualts, options);


    var restOps = {

        entity: "Pic",
        cntrl: "/Pic",
        data: ops
    }


    var container = ops.htmlId ? "." + ops.htmlId : "";
    var picElem = container + ".uplodable img";
    var deleteBtn = container + " .delete";//".DelAvatarBtn";
    var imgContainer = ".uplodable, .pic-container";
    if (ops.hasPic)
        setHasPic(picElem, ops.hasPic);
  
    var progreesBar = container + " .qq-progress-bar";

    var AvPath = ops.SubMode ?
         ops.ObjType + "\\" + ops.ObjID + "\\" + ops.FilePre + ".jpg" :
        "/Pics/" + ops.ObjType + "/" + ops.FilePre + ops.ObjID + ".jpg";
    var TempPath = AvPath.replace(ops.FilePre, "Temp_" + ops.FilePre);
    var popName = "popImg" + ops.htmlId;
    var popId = "#" + popName;

    var imageToCrop = popId + " .ImgFullImage";
    //var editBtn = container + ".EditPicBtn";

    function freeshPic(path) {
        return path + '?' + new Date().getTime();
    }


    function setPicElement(path) {

        if ($(".step-pics")[0]) {
            /** step pics - add to carosel*/
            var img = $(".step-pics [src^='" + path + "']");

            if ($(img).is("*"))
                img.attr("src", freeshPic(path));
            else
                addStepImage(freeshPic(path));
        }
        else {
            /** entity single pic - replace. cant click to view...*/
            var elem = $(picElem)[0];
            path = path || '/Pics/MachinePicture/MacType_1.jpg';

            if (elem) {
                $(picElem).attr('src', freeshPic(path));
                setHasPic(picElem, 'True');
            }

            //    elem.src =;

        }

        closeCrop();
    };

   
    function setHasPic(picElem, value) {
        $(picElem).parents(imgContainer).attr('data-haspic', value);
    }

    function openPreview(src, name) {
        var popPicPreview = "#popPicPreview";
        var imgId = popPicPreview + " .ImgFullImage";
        var cropBtns = ".pic-preview .CropBtns";

        var popContent =
            '<div class="pic-preview">' +

                '<div class="menu">' +
                    '<div class="rec-icon action rotate clock"></div>' +
                    '<div class="rec-icon action rotate counter"></div>' +
                     '<div class="rec-icon action crop-icon"></div>' +
                '</div>' +
                '<div class="img-container">' +
                  '<div class="img-container-inner">' +
                    '<img class="picZone ImgFullImage" data-deg="0" src="' + src + '"/>' +
                    '</div>' +
               '</div>' +
                //'<div class="picZone" data-deg="0" style="background-image:url(' + src + ')"></div>' +
                '<div class="CropBtns">' +
                    ' <input type="button" class="buttondefault red CancelUploadBtn" value="ביטול" />' +
                    '<input type="button" class="buttondefault UploadAndCropBtn" value="שמור">' +
                '</div>'
        '</div>';

        $.createAndOpenPop("PicPreview", popContent);

        $(" .rotate").click(function () {


            var img = $(".pic-preview .picZone");
            //  var imgSrc = $(img).css('background-image').split('"')[1];
            var imgSrc = $(imgId).attr("src");
            var number = getImgNumber(imgSrc);

            var isCounter = $(this).is(".counter");

            rotatePic(number, isCounter, setEditedPic );


        });

        $(popPicPreview + " .crop-icon").click(function () {

            var hideCrop = $(cropBtns).is(":visible");


            if (hideCrop) {
                return cancelCrop();

            }


            //var img = $(".pic-preview .picZone");
            //var imgSrc = $(img).css('background-image').split('"')[1];
            var imgSrc = $(imgId).attr("src");
          
            cropPic(
                {
                    path: imgSrc,
                    updateAvatar: updateAvatar,
                    ObjType: ops.ObjType,
                    imageToCrop: imgId,
                    popId: popPicPreview
                }

             );

            $(cropBtns).show();


        });

        $(popPicPreview + ' .CancelUploadBtn').click(function () {
            cancelCrop();
        });

        $(popPicPreview + ' .UploadAndCropBtn').click( function () {
            $(popId + ' #cropImg').val(3);
            restOps.data.Number = getImgNumber($(popPicPreview + " .ImgFullImage").attr("src"));

            updateAvatar(setEditedPic);
        });

        function setEditedPic(data) {
            /** preview image rotated*/
            //$(img).css('background-image', 'url(' + data.path + ')');
            $(imgId).attr("src", data.path);

            /** source image rotated*/
            $(picPreviewSrc).attr('src', data.path);

            cancelCrop();
        }

        function cancelCrop() {
            if (!jcrop_apis[imgId])
                return;

            $(cropBtns).hide();
            jcrop_apis[imgId].cropper('destroy');
        }
    }


    function updateAvatar(success) {

        //if ($(popId + ' #cropImg').val() != 3) {
        //    return closeCrop();
        //}

        //var x, y, w, h;

        //x = parseInt($(popId + ' #hfX').val());
        //y = parseInt($(popId + ' #hfY').val());
        //h = parseInt($(popId + ' #hfWidth').val());
        //w = parseInt($(popId + ' #hfHeight').val());

        // var pg = '/Pic/UpdateAvatar';
        //(string path, int X, int Y, int Width, int Height

        rest.update(
            $.extend(true, restOps, {
                data:cord,
                success: function (data) {
                   
                    setPicElement(data.path);
                    if (success) {
                        data.path = freeshPic(data.path);
                        success(data);
                    }
                   // $(deleteBtn).show();
                }
            })
         );

    }


    function deletePic(n, success) {
        rest.remove(
           $.extend(true, restOps, {
               data: {
                   "Number": n
               },
               success: function () {

                   success();
               }
           })
        );

    }

    function rotatePic(n, isCounter, success) {
        var ops = $.extend(true, restOps, {
            data: {
                "Number": n,
                "isCounter": isCounter
            },
            success: success
        });

        ops.url = ops.cntrl + "/Rotate";

        rest.post(ops);

    }



    /** deletePic should make this unrelvant*/
    function delPic() {
        if (FilePre != 'MacType_') {
            var pg = '/Pic/DelAvatar';
            //(string path, int X, int Y, int Width, int Height
            $.post(pg, ops, function (data) {
                if (data.sts == 1) {
                    setPicElement(data.path);
                   // $(deleteBtn).hide();
                }
            });
        }
        else {
            alert('לא ניתן למחוק תמונת ברירת מחדל');
        }

    }


    /** deletePic should make this unrelvant*/
    function delTempPic() {

        var pg = '/Pic/DelTempAvatar';
        $.post(pg, ops, function (data) {
            if (data.sts == 1) {
                closeCrop();
            }
        });

    }

    function closeCrop() {
        closePopup(popName, true);
        setTimeout(function () {
            $(popId + " .crop").html('<img class="ImgFullImage" src="" />');
        }, 300);

    }


    //$(container + " .FileUpload").click(function () {
    //    $(imageToCrop)[0].src =  $(picElem).attr('src');
    //    showPopup(popId);
    //});

    /** upload btn*/
    $(container + " .FileUpload").fineUploader({
        request: {
            endpoint: "/Pic/Add?" +
                "ObjID=" + ops.ObjID +
                "&ObjType=" + ops.ObjType +
                "&FilePre=" + ops.FilePre +
                "&SubMode=" + ops.SubMode
        },
        validation: {
            allowedExtensions: ["jpeg", "jpg", "gif", "png"]
            //sizeLimit: 2048000 // 2MG = 2000 * 1024 bytes
        },
        text: {
            uploadButton: "<span class=\"LinkText\"></span>"
        },
        messages: {
            typeError: "סוג קובץ לא מתאים{extensions}",
            sizeError: "גודל לא מתאים"
        },
        multiple: false
        //callbacks: {
        //    onProgress: function (id, name, uploadedBytes, totalBytes) {
        //        console.log(uploadedBytes / totalBytes);
        //    },
        //    onError: function (id, fileName, reason) {
        //        console.log(fileName + " | reason: " + reason);
        //        $.BugAlert(reason);
        //    }
        //}
        //debug: true
    })
        .on("complete", function (event, id, fileName, responseJson) {
            if (responseJson.success) {
                /** open crop windows, upload after crop*/
                $(progreesBar).hide();
                $(".qq-progress-bar").text("");

                ops.Number = responseJson.number;
                var path = responseJson.path + "?" + new Date().getTime();
                //if (!$(imageToCrop)[0])
                //    return logRed("no found img " + imageToCrop);


                updateAvatar();

                /*
                $(imageToCrop)[0].src = path;

                cropPic(
                    {
                        path: path,
                        updateAvatar: updateAvatar,
                        delTempPic: delTempPic,
                        //TempPath: TempPath,
                        ObjType: ops.ObjType,
                        imageToCrop: imageToCrop,
                        popId: popId
                    }

                 );
                 */
            }
        })
        .on("error", function (event, id, fileName, response) {
            logRed(response.msg);
            $.BugAlert(response.msg);
        })
         .on("progress", function (event, id, name, uploadedBytes, totalBytes) {

             var prec = (uploadedBytes / totalBytes) * 100;
             $(progreesBar).show().width(prec + "%");
             if (prec == 100) {
                 $(".qq-progress-bar").text("מעבד תמונה...");
             }
         })
    ;


    //$(deleteBtn).live("click", function () {
    //    if ($(imageToCrop)[0].src.indexOf("Temp_") > -1) {
    //        delTempPic();
    //    }
    //    else {
    //        delPic();
    //    }

    //});

    // if (ops.SubMode) {

    function getImgName(imgSrc) {
        var href = imgSrc.replace(location.origin, "")
       
        var url = href.split(".")[0];
        var urlSplit = url.split("/");
        var index = urlSplit.length - 1;

        return urlSplit[index];
    }

    function getImgNumber(imgSrc) {
        var name = getImgName(imgSrc);

        var number = name.replace(restOps.data.FilePre, "");

        if (number == "0")
            throw "number wast not found for " + imgSrc + " | FilePre -" + restOps.data.FilePre;

        return number;
    }

    /** delete pic in galery*/
    $(deleteBtn).live("click", function () {
        var that = this;
        // var jobId = $("#JobID").text();
        var img = $(that).parents(imgContainer).find('img');
        var imgSrc = img[0].src;
        //var picName = imgSrc.split($("#JobID").text() + "/")[1]
        var number = getImgNumber(imgSrc);

        if ($(that).parents(".step-pics")[0]) {
            deletePic(number, function () {
                $(that).parents(".MachineImg").remove();
            });
        }
        else {
            deletePic(number, function () {
                $(img).attr('src', $(img).attr('data-def'));
                setHasPic(that, 'False');
               // $(deleteBtn).hide();
            });
        }


    });



    function onImgClick(that) {
        picPreviewSrc = that;
        var imageSrc = picPreviewSrc[0].src;
        var popName = getImgName(imageSrc);

        /** pretest5, */
        openPreview(imageSrc, popName);
    }

    var picPreviewSrc;

    /** need to be live for adding new pic to job card step*/
    $(".step-pics img ").live('click', function () {

        onImgClick( $(this));
    });

    $(container + "[data-haspic=True] img").live('click',function () {
        onImgClick($(this));
    });

    //  }



    //$(editBtn).live("click", function () {
    //    $('#ImgFullImage')[0].src = AvPath;

    //    jcrop_api.setImage(AvPath);
    //    loadCropFrame();

    //    showPopup($('#ImgFullImage')[0].src, 1);
    //});


    //  jQuery(function ($) {




};


/** run all crops was wating to uploac.pic.js to be ready*/
function UploadPic_ready() {

    for (var i = 0; i < waitingUploadCrop.length; i++) {
        //console.log("run on ready " + waitingUploadCrop[i]);
        window[waitingUploadCrop[i]]();
    }
    waitingUploadCrop = []
   
    
}