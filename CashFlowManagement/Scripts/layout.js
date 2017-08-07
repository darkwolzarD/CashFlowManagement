$(document).ready(function () {
    var isModalShowing = false;

    $(document).on("click", ".reset-form", function () {
        RefreshLiabilityForm();
        RefreshLiabilityTable()
    })

    $(document).on("click", "#liability-table-div #liability-table td", function () {
        var row = $(this).closest("tr");
        var id = row.find(".liability-id").val();
        row.siblings().removeClass("success");
        if (row.hasClass("success")) {
            row.removeClass("success");
        }
        else {
            row.addClass("success");
        }
        if (row.hasClass("success")) {
            $.ajax({
                url: Url.LiabilityUpdateForm,
                type: "get",
                data: { id: id },
                success: function (data) {
                    $("#liability-form-div").html($(data).html());
                    MaskInput();
                }
            });
        }
        else {
            RefreshLiabilityForm();
        }
    })

    $(document).on("click", ".input-group-addon", function () {
        if ($(this).children().hasClass("glyphicon-calendar") && $(this).siblings().attr("readonly") != "readonly") {
            $(this).siblings().datepicker("show");
        }
    })

    $(document).on("click", ".modal-toggle", function (e) {
        isModalShowing = true;
    })

    $('.modal').on('hidden.bs.modal', function (e) {
        isModalShowing = false;
    })
})