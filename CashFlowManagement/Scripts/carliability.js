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

    function RefreshCarLiabilityTable() {
        $.ajax({
            url: Url.CarLiabilityTable,
            type: "get",
            success: function (data) {
                $("#car-liability-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CarLiabilityForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#car-liability-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-car-liability", function () {
        var model = $("#car-liability-form").serialize();
        $.ajax({
            url: Url.CarLiabilityForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#car-liability-create-modal").modal("hide");
                    RefreshCarLiabilityTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#car-liability-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-car-liability", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.CarLiabilityUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#car-liability-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-car-liability", function () {
        var model = $("#car-liability-update-form").serialize();
        $.ajax({
            url: Url.CarLiabilityUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#car-liability-update-modal").modal("hide");
                    RefreshCarLiabilityTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#car-liability-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-car-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteCarLiability,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshCarLiabilityTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })
})