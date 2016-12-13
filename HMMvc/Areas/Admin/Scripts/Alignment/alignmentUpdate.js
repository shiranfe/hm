
$(document).ready(function () {

    var ops = {
        entity: "AlignmentPart",
        cntrl: "/Admin/AlignmentPart",
    }

  
    crud(ops);

    function updateJob() {
        
        rest.update({
            entity: "Alignment",
            cntrl: "/Admin/Alignment",
            data: {
                JobID:$("#JobID").text(),
                OpenNotes: CKEDITOR.instances.OpenNotes.getData()
            }
        }
       );
    }

    function updatePart(data) {
        rest.update(
           $.extend({}, {
               data: data
           }, ops)
       );
    }


    /*****************         UI FUNCTIONS          ******************/


    $("#Parts").change( function () {
        var partID = $(this).val();

        location.href = "/Admin/#/Alignment/Update?PartID=" + partID;
    });
    
    CKEDITOR.inline("OpenNotes");

    for (var i in CKEDITOR.instances) {
        CKEDITOR.instances[i].on('blur', updateJob);
    }

    var form = "#UpdateAlignmentPartForm";
    $(form + " [name]").live("blur", function () {
        if ($(form).valid()) {
            updatePart($(form).serialize());
        }
    });

});


