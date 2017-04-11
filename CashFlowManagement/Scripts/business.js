$(document).ready(function () {
    function MaskInput() {
        $(".input-mask").mask("000,000,000,000,000", { reverse: true });
        $(".percentage").mask("##0,00%", { reverse: true });
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
    }

    MaskInput();

    function InitiateDatePicker() {
        $(".date-picker").datepicker({
            format: "mm/yyyy",
            minViewMode: 1,
            language: "vi-VN"
        });
    }

    $(document).on("shown.bs.modal", "#create-new-business-modal, #create-new-loan-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-business-modal, #update-loan-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    function isValidDate(dateString) {
        // First check for the pattern
        var regex_date = /^\d{2}\/\d{4}$/;

        if (!regex_date.test(dateString)) {
            return false;

            var parts = dateString.split("/");
            var month = parseInt(parts[0], 10);
            var year = parseInt(parts[1], 10);

            if (year < 1000 || year > 3000 || month == 0 || month > 12) {
                return false;
            }
        }

        return true;
    }

    $(document).on("click", ".create-business", function () {
        RemoveMask();
        var data = $("#create-new-business-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateBusiness,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-business-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".create-loan", function () {
        RemoveMask();
        var data = $("#create-new-loan-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateLoan,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-loan-modal").modal("hide");
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
            type: "get",
            dataType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".business-table").html(data);
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    }

    $(document).on("click", ".update-business", function () {
        var id = $(this).closest("tr").find(".business-id").text();

        $.ajax({
            url: Url.LoadBusiness,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-business-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-business", function () {
        RemoveMask();
        var data = $("#update-business-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateBusiness,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-business-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-business", function () {
        var id = $(this).closest("tr").find(".business-id").text();
        if (confirm("Do you really want to delete this business?") == true) {
            $.ajax({
                url: Url.DeleteBusiness,
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

    $(document).on("click", ".create-new-loan", function () {
        var id = $(this).data("real-estate-id");

        $.ajax({
            url: Url.LoanModal,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".loan-modal").html(data);
                    $("#create-new-loan-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".update-loan", function () {
        var id = $(this).data("loan-id");
        var trigger = $(this).data("trigger");

        if (trigger == "edit-no-rate") {
            $.ajax({
                url: Url.LoadLoan,
                type: "get",
                data: { id: id },
                contentType: "html",
                success: function (data) {
                    if (data.length > 0) {
                        $(".update-loan-modal-div").html(data);
                        $("#update-loan-modal").modal("show");
                    }
                    else {
                        alert("Có lỗi xảy ra");
                    }
                }
            })
        }
        else {
            $.ajax({
                url: Url.LoadLoanWithRate,
                type: "get",
                data: { id: id },
                contentType: "html",
                success: function (data) {
                    if (data.length > 0) {
                        $(".update-loan-modal-div").html(data);
                        $(".update-loan-modal-div").find("input[name='StartDate']").attr("data-date-start-date", $(".update-loan-modal-div").find("input[name='StartDate']").val());
                        $(".update-loan-modal-div").find("input[name='EndDate']").attr("data-date-end-date", $(".update-loan-modal-div").find("input[name='EndDate']").val());
                        $("#update-loan-modal").modal("show");
                    }
                    else {
                        alert("Có lỗi xảy ra");
                    }
                }
            })
        }
    })

    $(document).on("click", ".save-loan", function () {
        RemoveMask();
        var data = $("#update-loan-modal .form-horizontal").serialize();
        if ($(this).data("trigger") == "save-no-rate") {
            if (confirm("Thay đổi kì hạn của khoản vay sẽ hủy bỏ các kì hạn con trước đó. Bạn có muốn tiếp tục?")) {
                $.ajax({
                    url: Url.UpdateLoan,
                    type: "post",
                    data: data,
                    success: function (data) {
                        if (data.result > 0) {
                            $("#update-loan-modal").modal("hide");
                            LoadTable();
                        }
                        else {
                            alert("Có lỗi xảy ra");
                        }
                    }
                })
                MaskInput();
            }
            else {
                alert("Không có dữ liệu nào bị thay đổi");
            }
        }
        else {
            $.ajax({
                url: Url.UpdateLoan,
                type: "post",
                data: data,
                success: function (data) {
                    if (data.result > 0) {
                        $("#update-loan-modal").modal("hide");
                        LoadTable();
                    }
                    else {
                        alert("Có lỗi xảy ra");
                    }
                }
            })
            MaskInput();
        }
    })

    $(document).on("click", ".delete-loan", function () {
        var id = $(this).data("loan-id");
        if (confirm("Do you really want to delete this loan?") == true) {
            $.ajax({
                url: Url.DeleteLoan,
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
        var _data = parseInt($(this).data("loan-id"));

        $.ajax({
            url: Url.PaymentsPerMonth,
            type: "get",
            data: { loanId: _data },
            dataType: "html",
            success: function (data) {
                $(".interest-modal-div").html(data);
                $("#interest-modal").modal("show");
            }
        })
    })

    $(document).on("show.bs.collapse", "tr[class^='detail-']", function () {
        var rs_cls = $(this).closest("tr").attr("class").split(' ')[1];
        var child_cls = $(this).closest("tr").attr("class").split(' ')[2];
        var current = $(document).find("table tbody ." + rs_cls + ":not(.collapse)").length + 1;
        var collapse = $(document).find("table tbody ." + child_cls + ".collapse").length;
        $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(2)").attr("rowspan", current + collapse);
    })

    $(document).on("hidden.bs.collapse", "tr[class^='detail-']", function () {
        var rs_cls = $(this).closest("tr").attr("class").split(' ')[1];
        var child_cls = $(this).closest("tr").attr("class").split(' ')[2];
        var current = $(document).find("table tbody ." + rs_cls + ":not(.collapse)").length;
        var collapse = $(document).find("table tbody ." + child_cls + ".collapse").length;
        $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(2)").attr("rowspan", current - collapse);
    })

    $(document).on("changeDate", ".with-rate .date-picker", function () {
        RemoveMask();
        var startDate = $(this).closest("#update-loan-modal").find("input[name='StartDate']").val();
        var endDate = $(this).closest("#update-loan-modal").find("input[name='EndDate']").val();
        if (isValidDate(startDate) && isValidDate(endDate)) {
            var data = $("#update-loan-modal .form-horizontal").serialize();

            $.ajax({
                url: Url.PaymentsPerMonth,
                type: "post",
                data: data,
                success: function (data) {
                    $(".payments-per-month-table").html($(data).find(".modal-body").html());
                }
            })
        }
        MaskInput();
    })
})