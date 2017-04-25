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
            format: "dd/mm/yyyy",
            language: "vi-VN"
        });
    }

    function isValidDate(dateString) {
        // First check for the pattern
        var regex_date = /^\d{2}\/\d{4}$/;

        if (!regex_date.test(dateString)) {
            return false;
        }
        else {
            var parts = dateString.split("/");
            var month = parseInt(parts[0], 10);
            var year = parseInt(parts[1], 10);

            if (year < 1000 || year > 3000 || month === 0 || month > 12) {
                return false;
            }
        }
        return true;
    }

    InitiateDatePicker();

    function LoadTable() {
        $.ajax({
            url: Url.LoadTable,
            type: "get",
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

    $(document).on("click", ".create-log", function () {
        RemoveMask();
        var data = $("#log-form").serialize();

        $.ajax({
            url: Url.CreateLog,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })
})