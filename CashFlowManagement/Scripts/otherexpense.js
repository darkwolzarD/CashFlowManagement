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

    function RefreshOtherExpenseTable() {
        $.ajax({
            url: Url.OtherExpenseTable,
            type: "get",
            success: function (data) {
                $("#other-expense-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.OtherExpenseForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#other-expense-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-other-expense", function () {
        var model = $("#other-expense-form").serialize();
        $.ajax({
            url: Url.OtherExpenseForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-expense-create-modal").modal("hide");
                    RefreshOtherExpenseTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#other-expense-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-other-expense", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.OtherExpenseUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#other-expense-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-other-expense", function () {
        var model = $("#other-expense-update-form").serialize();
        $.ajax({
            url: Url.OtherExpenseUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-expense-update-modal").modal("hide");
                    RefreshOtherExpenseTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#other-expense-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-other-expense", function () {
        if (confirm("Bạn có muốn xóa chi tiêu này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteOtherExpense,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshOtherExpenseTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".expense-toggle-summary", function () {
        $.ajax({
            url: Url.ExpenseSummary,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#expense-summary-modal").modal("show");
                MaskInput();
            }
        });
    })
})