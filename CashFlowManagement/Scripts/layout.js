$(document).ready(function () {
    //var table = $("table").DataTable({
    //    dom:
    //    "<'row'<'col-sm-10'B><'col-sm-2'f>>" +
    //    "<'row'<'col-sm-12'tr>>" +
    //    "<'row'<'col-sm-5'i><'col-sm-7'p>>",
    //    buttons: [
    //        {
    //            text: "New income",
    //            className: "btn btn-primary create-new-income"
    //        }
    //    ],
    //    "pageLength": 25
    //});

    //table.button(0).nodes().attr('data-toggle', 'modal');
    //table.button(0).nodes().attr('data-target', '#create-new-income-modal');
    
   
    $(document).on("click", ".navigation-bar > li > a", function () {
        if ($(this).find(".fa").attr("class") == "fa fa-angle-left") {
            $(this).find(".fa").removeClass("fa-angle-left").addClass("fa-angle-down");
            $(this).parent().find(".second-navbar").show(300);
        }
        else {
            $(this).find(".fa").removeClass("fa-angle-down").addClass("fa-angle-left");
            $(this).parent().find(".second-navbar").hide(300);
        }
    })

    $(document).on("click", ".minimize-sidebar", function () {
        if ($(".menu").width() > 0) {
            $(".menu").css("width", "0");
            $(".content").css("cssText", "padding-left: 20px !important");
        }
        else {
            $(".menu").css("width", "160px");
            $(".content").css("cssText", "padding-left: 180px !important");
        }
    })
})