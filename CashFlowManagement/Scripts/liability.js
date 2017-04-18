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

    $(document).on("shown.bs.modal", "#create-new-liability-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-liability-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    function LoadTable() {
        $.ajax({
            url: Url.LoadTable,
            type: "post",
            data: { type: liabilityType },
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

    $(document).on("click", ".create-liability", function () {
        RemoveMask();
        var data = $("#create-new-liability-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateLiability,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-liability-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".update-liability", function () {
        var id = $(this).data("liability-id");

        $.ajax({
            url: Url.LoadLiability,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-liability-modal-div").html(data);
                    $("#update-liability-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-liability", function () {
        RemoveMask();
        var data = $("#update-liability-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateLiability,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-liability-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-liability", function () {
        var id = $(this).data("liability-id");
        if (confirm("Bạn có muốn xóa khoản nợ này?") == true) {
            $.ajax({
                url: Url.DeleteLiability,
                type: "POST",
                data: { id: id },
                success: function (data) {
                    if (data.result > 0) {
                        alert("Success!");
                        LoadTable();
                    }
                    else {
                        alert("Có lỗi xảy ra");
                    }
                }
            })
        }
    })

    $(document).on("click", ".interest-info", function () {
        var _data = parseInt($(this).data("liability-id"));

        $.ajax({
            url: Url.PaymentPerMonth,
            type: "get",
            data: { id: _data },
            dataType: "html",
            success: function (data) {
                $(".interest-modal-div").html(data);
                $("#interest-modal").modal("show");
            }
        })
    })
})