$(document).ready(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value, "MM/YYYY", true).isValid() || moment(value, "dd/MM/YYYY", true).isValid();
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }

    function MaskInput() {
        $(".input-mask").mask("000.000.000.000.000", { reverse: true });
        $(".percentage").mask("000.000.000.000.000,00", { reverse: true });
        $(".date-picker-with-day").datepicker({
            format: "dd/mm/yyyy",
            language: "vi-VN",
            autoclose: true
        });
        $(".date-picker").datepicker({
            format: "mm/yyyy",
            minViewMode: 1,
            language: "vi-VN",
            autoclose: true
        });
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
    }

    MaskInput();

    function RefreshOtherLiabilityTable() {
        $.ajax({
            url: Url.OtherLiabilityTable,
            type: "get",
            success: function (data) {
                $("#other-liability-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.OtherLiabilityForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#other-liability-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-other-liability", function () {
        var model = $("#other-liability-form").serialize();
        $.ajax({
            url: Url.OtherLiabilityForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-liability-create-modal").modal("hide");
                    RefreshOtherLiabilityTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#other-liability-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-other-liability", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.OtherLiabilityUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#other-liability-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-other-liability", function () {
        var model = $("#other-liability-update-form").serialize();
        $.ajax({
            url: Url.OtherLiabilityUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-liability-update-modal").modal("hide");
                    RefreshOtherLiabilityTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#other-liability-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-other-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteOtherLiability,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshOtherLiabilityTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".liability-toggle-summary", function () {
        $.ajax({
            url: Url.LiabilitySummary,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#liability-summary-modal").modal("show");
                MaskInput();
            }
        });
    })
})