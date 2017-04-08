$(document).ready(function () {
    function MaskInput() {
        $(".input-mask").mask("000,000,000,000,000", { reverse: true });//for cho nhung element HTML co class = input-mask
        $(".percentage").mask("##0,00%", { reverse: true });            //for cho nhung element HTML co class = percentage
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
        //$(".date").unmask();
    }

    MaskInput();        //window.MaskInput();

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

            var parts = dateString.split("/");
            var month = parseInt(parts[0], 10);
            var year = parseInt(parts[1], 10);

            if (year < 1000 || year > 3000 || month == 0 || month > 12) {
                return false;
            }
        }

        return true;
    }

    $(document).on("shown.bs.modal", "#create-new-real-estate-modal, #create-new-loan-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-real-estate-modal, #update-loan-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("click", ".create-real-estate", function () {           //nhung element co class la "create-real-estate" - <button type="button" class="btn btn-success create-real-estate">Create</button>
        RemoveMask();
        var _data = $("#create-new-real-estate-modal .form-horizontal").serialize();     //chuoi parameters

        $.ajax({
            url: Url.CreateRealEstate,
            type: "post",
            data: _data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-real-estate-modal").modal("hide");
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
                    $(".real-estate-table").html(data);
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    }

    $(document).on("click", ".update-real-estate", function () {
        var id = $(this).closest("tr").find(".real-estate-id").text();

        $.ajax({
            url: Url.LoadRealEstate,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-real-estate-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-real-estate", function () {
        RemoveMask();
        var data = $("#update-real-estate-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateRealEstate,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-real-estate-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-real-estate", function () {
        var id = $(this).closest("tr").find(".real-estate-id").text();
        if (confirm("Do you really want to delete this real estate?") == true) {
            $.ajax({
                url: Url.DeleteRealEstate,
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
        var count = $(document).find("table tbody tr:not(.collapse)").length;
        $(document).find("table tbody tr:nth-child(1) td:nth-child(2)").attr("rowspan", count);
    })

    $(document).on("hidden.bs.collapse", "tr[class^='detail-']", function () {
        var count = $(document).find("table tbody tr:not(.collapse)").length - 2;
        $(document).find("table tbody tr:nth-child(1) td:nth-child(2)").attr("rowspan", count);
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
    })
})
