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

    function RefreshFamilyExpenseTable() {
        $.ajax({
            url: Url.FamilyExpenseTable,
            type: "get",
            success: function (data) {
                $("#family-expense-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.FamilyExpenseForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#family-expense-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-family-expense", function () {
        var model = $("#family-expense-form").serialize();
        $.ajax({
            url: Url.FamilyExpenseForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#family-expense-create-modal").modal("hide");
                    RefreshFamilyExpenseTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#family-expense-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-family-expense", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.FamilyExpenseUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#family-expense-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-family-expense", function () {
        var model = $("#family-expense-update-form").serialize();
        $.ajax({
            url: Url.FamilyExpenseUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#family-expense-update-modal").modal("hide");
                    RefreshFamilyExpenseTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#family-expense-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-family-expense", function () {
        if (confirm("Bạn có muốn xóa chi tiêu này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteFamilyExpense,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshFamilyExpenseTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })
})