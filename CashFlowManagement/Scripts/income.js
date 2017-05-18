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

    $(document).on("shown.bs.modal", "#create-new-income-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-income-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $('#create-new-income-modal').on('hidden.bs.modal', function (e) {
        $(this)
            .find("input[type!='hidden'],textarea,select")
            .val('')
            .end()
            .find("input[type=checkbox], input[type=radio]")
            .prop("checked", "")
            .end();
        MaskInput();
    })

    $(document).on("click", ".create-income", function () {
        RemoveMask();
        var data = $("#create-new-income-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateIncome,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-income-modal").modal("hide");
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
            data: { type: incomeType },
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

    $(document).on("click", ".update-income", function () {
        var id = $(this).data("income-id");

        $.ajax({
            url: Url.LoadIncome,
            type: "get",
            data: { incomeId: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-income-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-income", function () {
        RemoveMask();
        var data = $("#update-income-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateIncome,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-income-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-income", function () {
        var id = $(this).data("income-id");
        if (confirm("Bạn có muốn xóa thu nhập này?") === true) {
            $.ajax({
                url: Url.DeleteIncome,
                type: "POST",
                data: { incomeId: id },
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