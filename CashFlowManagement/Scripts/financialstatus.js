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

    $("#available-money-create-modal").modal("show");
    MaskInput();

    $(document).on("click", ".toggle-available-money", function () {
        $.ajax({
            url: Url.AvailableMoneyForm,
            type: "get",
            success: function (data) {
                $("#modal").html($(data));
                $("#available-money-create-modal").modal("show");
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
                    location.reload();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#available-money-create-modal").html($(data).html());
                }
            }
        });
    })
})