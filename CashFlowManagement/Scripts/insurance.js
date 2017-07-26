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

    function RefreshInsuranceTable() {
        $.ajax({
            url: Url.InsuranceTable,
            type: "get",
            success: function (data) {
                $("#insurance-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.InsuranceForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#insurance-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-insurance", function () {
        var model = $("#insurance-form").serialize();
        $.ajax({
            url: Url.InsuranceForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#insurance-create-modal").modal("hide");
                    RefreshInsuranceTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#insurance-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-insurance", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.InsuranceUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#insurance-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-insurance", function () {
        var model = $("#insurance-update-form").serialize();
        $.ajax({
            url: Url.InsuranceUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#insurance-update-modal").modal("hide");
                    RefreshInsuranceTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#insurance-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-insurance", function () {
        if (confirm("Bạn có muốn xóa bảo hiểm này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteInsurance,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshInsuranceTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })
})