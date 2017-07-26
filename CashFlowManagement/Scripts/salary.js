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

    function RefreshSalaryTable() {
        $.ajax({
            url: Url.SalaryTable,
            type: "get",
            success: function (data) {
                $("#salary-table-div").html($(data).html());
            }
        })
    }

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.SalaryForm,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#salary-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-salary", function () {
        var model = $("#salary-form").serialize();
        $.ajax({
            url: Url.SalaryForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#salary-create-modal").modal("hide");
                    RefreshSalaryTable();
                }
                else if (data === "failed"){
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#salary-create-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-salary", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.SalaryUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#salary-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-salary", function () {
        var model = $("#salary-update-form").serialize();
        $.ajax({
            url: Url.SalaryUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#salary-update-modal").modal("hide");
                    RefreshSalaryTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#salary-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".delete-salary", function () {
        if (confirm("Bạn có muốn xóa thu nhập lương này?")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteSalary,
                type: "get",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshSalaryTable();
                    }
                    else if (data === "failed") {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".salary-toggle-summary", function () {
        $.ajax({
            url: Url.SalarySummary,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#salary-summary-modal").modal("show");
                MaskInput();
            }
        });
    })
})