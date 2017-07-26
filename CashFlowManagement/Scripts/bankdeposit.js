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

    function RefreshBankDepositTable() {
        $.ajax({
            url: Url.BankDepositTable,
            type: "get",
            success: function (data) {
                $("#bank-deposit-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.BankDepositForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#bank-deposit-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-bank-deposit", function () {
        var model = $("#bank-deposit-form").serialize();
        $.ajax({
            url: Url.BankDepositForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#bank-deposit-create-modal").modal("hide");
                    RefreshBankDepositTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#bank-deposit-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-bank-deposit", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.BankDepositUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#bank-deposit-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-bank-deposit", function () {
        var model = $("#bank-deposit-update-form").serialize();
        $.ajax({
            url: Url.BankDepositUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#bank-deposit-update-modal").modal("hide");
                    RefreshBankDepositTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#bank-deposit-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-bank-deposit", function () {
        if (confirm("Bạn có muốn xóa tài khoản tiết kiệm này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteBankDeposit,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshBankDepositTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("keyup", "#StartDate, #PaymentPeriod", function () {
        RemoveMask();
        var valid = moment($("#StartDate").val(), "DD/MM/YYYY").isValid();
        if (valid) {
            var startDate = moment($("#StartDate").val(), "DD/MM/YYYY");
            var term = parseInt($("#PaymentPeriod").val());
            var endDate = startDate.add(term, 'months');
            $("#EndDate").val(endDate.format("DD/MM/YYYY"));
            MaskInput();
        }
    })
})