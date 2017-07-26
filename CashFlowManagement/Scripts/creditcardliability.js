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

    function RefreshCreditCardLiabilityTable() {
        $.ajax({
            url: Url.CreditCardLiabilityTable,
            type: "get",
            success: function (data) {
                $("#credit-card-liability-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CreditCardLiabilityForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#credit-card-liability-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-credit-card-liability", function () {
        var model = $("#credit-card-liability-form").serialize();
        $.ajax({
            url: Url.CreditCardLiabilityForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#credit-card-liability-create-modal").modal("hide");
                    RefreshCreditCardLiabilityTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#credit-card-liability-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-credit-card-liability", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.CreditCardLiabilityUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#credit-card-liability-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-credit-card-liability", function () {
        var model = $("#credit-card-liability-update-form").serialize();
        $.ajax({
            url: Url.CreditCardLiabilityUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#credit-card-liability-update-modal").modal("hide");
                    RefreshCreditCardLiabilityTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#credit-card-liability-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-credit-card-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteCreditCardLiability,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshCreditCardLiabilityTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })
})