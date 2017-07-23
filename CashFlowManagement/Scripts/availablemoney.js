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

    $(document).on("click", ".toggle-modal", function () {
        if (check === 'True') {
            alert("Tiền mặt có sẵn chỉ khai báo 1 lần duy nhất.\nVui lòng thực hiện chức năng Chỉnh sửa/Cập nhật Tiền mặt có sẵn!!!");
        }
        else {
            $.ajax({
                url: Url.AvailableMoneyForm,
                type: "get",
                success: function (data) {
                    $("#modal").html($(data));
                    $("#available-money-create-modal").modal("show");
                    MaskInput();
                }
            });
        }
    })

    $(document).on("click", ".update-available-money", function () {
        $.ajax({
            url: Url.AvailableMoneyForm,
            type: "get",
            success: function (data) {
                $("#modal").html($(data));
                $("#available-money-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-available-money", function () {
        var model = $("#available-money-form").serialize();
        $.ajax({
            url: Url.AvailableMoneyForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#available-money-create-modal").modal("hide");
                    $.ajax({
                        url: Url.AvailableMoneyTable,
                        type: "get",
                        success: function (data) {
                            $("#available-money-table-div").html($(data).html());
                            $.ajax({
                                url: Url.CheckExistAvailableMoney,
                                type: "get",
                                success: function (data) {
                                    check = data;
                                }
                            });
                        }
                    });
                }
                else if (data === "failed") {
                    alert("Error!");
                }
                else {
                    $("#available-money-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".available-money-toggle-summary", function () {
        if (check === 'False') {
            alert("Quý khách vui lòng khai báo tiền mặt có sẵn trước khi tiếp tục");
        }
        else {
            $.ajax({
                url: Url.AvailableMoneySummary,
                type: "get",
                success: function (data) {
                    $("#modal").html($(data));
                    $("#available-money-summary-modal").modal("show");
                }
            });
        }
    })
})