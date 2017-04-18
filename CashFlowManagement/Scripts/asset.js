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

    $(document).on("shown.bs.modal", "#create-new-asset-modal, #create-new-liability-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#update-asset-modal, #update-liability-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $('#create-new-asset-modal').on('hidden.bs.modal', function (e) {
        $(this)
            .find("input[type!='hidden'],textarea,select")
            .val('')
            .end()
            .find("input[type=checkbox], input[type=radio]")
            .prop("checked", "")
            .end();
        MaskInput();
    })

    $(document).on("click", ".create-asset", function () {
        RemoveMask();
        var data = $("#create-new-asset-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateAsset,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-asset-modal").modal("hide");
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
            data: { type: assetType },
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

    $(document).on("click", ".update-asset", function () {
        var id = $(this).data("asset-id");

        $.ajax({
            url: Url.LoadAsset,
            type: "get",
            data: { assetId: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-asset-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-asset", function () {
        RemoveMask();
        var data = $("#update-asset-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateAsset,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-asset-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-asset", function () {
        var id = $(this).data("asset-id");
        if (confirm("Bạn có muốn xóa tài sản này?") === true) {
            $.ajax({
                url: Url.DeleteAsset,
                type: "POST",
                data: { assetId: id },
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

    $(document).on("click", ".create-new-liability", function () {
        var id = $(this).data("asset-id");
        var type = $(this).data("asset-type");

        $.ajax({
            url: Url.LiabilityModal,
            type: "get",
            data: { assetId: id, type: type },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".liability-modal").html(data);
                    $("#create-new-liability-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

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
        var trigger = $(this).data("trigger");

        $.ajax({
            url: Url.LoadLiability,
            type: "get",
            data: { id: id, trigger: trigger },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-liability-modal-div").html(data);
                    $("#update-liability-modal").modal("show");
                    if (trigger == "edit-rate") {
                        $(".update-liability-modal-div").find("input[name='StartDate']").attr("data-date-start-date", $(".update-liability-modal-div").find("input[name='StartDate']").val());
                        $(".update-liability-modal-div").find("input[name='EndDate']").attr("data-date-end-date", $(".update-liability-modal-div").find("input[name='EndDate']").val());
                    }
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
        if ($(this).data("trigger") == "save-no-rate") {
            if (confirm("Thay đổi kì hạn của khoản vay sẽ hủy bỏ các kì hạn con trước đó. Bạn có muốn tiếp tục?")) {
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
            }
            else {
                alert("Không có dữ liệu nào bị thay đổi");
            }
        }
        else {
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
        }
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

    $(document).on("changeDate", ".with-rate .date-picker", function () {
        RemoveMask();
        var startDate = $(this).closest("#update-liability-modal").find("input[name='StartDate']").val();
        var endDate = $(this).closest("#update-liability-modal").find("input[name='EndDate']").val();
        if (isValidDate(startDate) && isValidDate(endDate)) {
            var data = $("#update-liability-modal .form-horizontal").serialize();

            $.ajax({
                url: Url.PaymentPerMonth,
                type: "post",
                data: data,
                success: function (data) {
                    $(".payments-per-month-table").html($(data).find(".modal-body").html());
                }
            })
        }
        MaskInput();
    })

    $(document).on("change", ".with-rate #InterestRate", function () {
        RemoveMask();
        var startDate = $(this).closest("#update-liability-modal").find("input[name='StartDate']").val();
        var endDate = $(this).closest("#update-liability-modal").find("input[name='EndDate']").val();
        if (isValidDate(startDate) && isValidDate(endDate)) {
            var data = $("#update-liability-modal .form-horizontal").serialize();

            $.ajax({
                url: Url.PaymentPerMonth,
                type: "post",
                data: data,
                success: function (data) {
                    $(".payments-per-month-table").html($(data).find(".modal-body").html());
                }
            })
        }
        MaskInput();
    })

    $(document).on("show.bs.collapse", "tr[class^='detail-']", function () {
        var rs_cls = $(this).closest("tr").attr("class").split(' ')[1];
        var child_cls = $(this).closest("tr").attr("class").split(' ')[2];
        var current = $(document).find("table tbody ." + rs_cls + ":not(.collapse)").length + 1;
        var collapse = $(document).find("table tbody ." + child_cls + ".collapse").length;
        $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current + collapse);
    })

    $(document).on("hidden.bs.collapse", "tr[class^='detail-']", function () {
        var rs_cls = $(this).closest("tr").attr("class").split(' ')[1];
        var child_cls = $(this).closest("tr").attr("class").split(' ')[2];
        var current = $(document).find("table tbody ." + rs_cls + ":not(.collapse)").length;
        var collapse = $(document).find("table tbody ." + child_cls + ".collapse").length;
        $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current - collapse);
    })

})