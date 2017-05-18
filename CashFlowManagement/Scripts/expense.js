$(document).ready(function () {
    function MaskInput() {
        $(".input-mask").mask("000,000,000,000,000", { reverse: true });
        $(".percentage").mask("##0,00%", { reverse: true });
        $('.date').mask('00/00/0000');
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
        $(".date").unmask();
    }

    MaskInput();

    function InitiateDatePicker() {
        $(".date-picker").datepicker({
            format: "mm/yyyy",
            minViewMode: 1,
            language: "vi-VN"
        });
    }

    $(document).on("shown.bs.modal", "#create-new-expense-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-expense-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $('#create-new-expense-modal').on('hidden.bs.modal', function (e) {
        $(this)
            .find("input[type!='hidden'],textarea,select")
            .val('')
            .end()
            .find("input[type=checkbox], input[type=radio]")
            .prop("checked", "")
            .end();
        MaskInput();
    })

    $(document).on("click", ".create-expense", function () {
        RemoveMask();
        var data = $("#create-new-expense-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateExpense,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-expense-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    function LoadTable() {
        $.ajax({
            url: Url.LoadTable,
            type: "post",
            data: { type: expenseType },
            dataType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".background").html($(data).find(".background").children());
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    }

    $(document).on("click", ".update-expense", function () {
        var id = $(this).data("expense-id");

        $.ajax({
            url: Url.LoadExpense,
            type: "get",
            data: { expenseId: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-expense-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-expense", function () {
        RemoveMask();
        var data = $("#update-expense-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateExpense,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-expense-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-expense", function () {
        var id = $(this).data("expense-id");
        if (confirm("Bạn có muốn xóa khoản chi tiêu này?") === true) {
            $.ajax({
                url: Url.DeleteExpense,
                type: "POST",
                data: { expenseId: id },
                success: function (data) {
                    if (data.result > 0) {
                        LoadTable();
                    }
                    else {
                        alert("Có lỗi xảy ra");
                    }
                }
            })
        }
    })

})