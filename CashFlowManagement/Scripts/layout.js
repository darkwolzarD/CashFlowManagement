$(document).ready(function () {
    $(document).on("click", ".reset-form", function () {
        RefreshLiabilityForm();
        RefreshLiabilityTable()
    })

    $(document).on("click", "#liability-table-div #liability-table td", function () {
        //$(this).closest("tr").find(".liability-id").trigger("click");
        var row = $(this).closest("tr");
        var id = row.find(".liability-id").val();
        $("#liability-table tr").not(row).removeClass("active");
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
})