﻿$(document).ready(function () {
    var liabilityCount = 0;

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

    function InitiateDatePicker2() {
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

    $(document).on("shown.bs.modal", "#create-new-asset-modal, #update-asset-modal" , function () {
        MaskInput();
        if (assetType == 5) {
            InitiateDatePicker2();
        }
        else {
            InitiateDatePicker();
        }
    })

    $(document).on("shown.bs.modal", "#create-new-liability-modal, #update-liability-modal, #sell-asset-modal", function () {
        MaskInput();
        InitiateDatePicker();
    })

    $(document).on("shown.bs.modal", "#buy-new-asset-modal", function () {
        MaskInput();
        InitiateDatePicker();
        liabilityCount = 0;
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

    $('#buy-new-asset-modal').on('hidden.bs.modal', function (e) {
        $(this)
            .find("input[type!='hidden'],textarea")
            .val('')
            .end()
            .find("input[type=checkbox], input[type=radio]")
            .prop("checked", "")
            .end();
        MaskInput();
        $(this).find("#liability-table tbody tr:not(':first')").remove();
    })

    $(document).on("click", ".create-asset", function () {
        RemoveMask();
        var data = $("#create-new-asset-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateAsset,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result == -1) {
                    alert("Tên tài sản đã tồn tại, vui lòng nhập tên khác");
                }
                else if (data.result > 0) {
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

    $(document).on("click", ".buy-asset", function () {
        $.ajax({
            url: Url.CheckAvailableMoney,
            type: "get",
            success: function (data) {
                if (data.result > 0) {
                    RemoveMask();
                    var currentMoney = data.result;
                    var currentAmount = $("#buy-new-asset-modal input[name='Asset.Value']").val();
                    if (currentAmount == "") {
                        currentAmount == 0;
                    }
                    var currentLiabilities = 0;
                    $("#liability-table tbody tr:hidden").each(function (index, element) {
                        var liability = $(element).find("td input").val();
                        if (liability == "") {
                            liability == 0;
                        }
                        currentLiabilities += liability;
                    });
                    MaskInput();
                    if (currentAmount == "" || currentAmount == 0) {
                        alert("Vui lòng nhập số tiền mua nhà!");
                    }
                    else if (currentMoney < currentAmount) {
                        if (currentLiabilities == 0) {
                            alert("Không đủ tiền mặt để mua bất động sản này!");
                        }
                        else if (currentLiabilities > 0 && (currentAmount + currentLiabilities < currentMoney)) {
                            alert("Bạn chưa vay đủ tiền mặt để mua bất động sản này!");
                        }
                        else if (currentLiabilities > 0 && (currentAmount + currentLiabilities > currentMoney)) {
                            alert("Bạn đã vay dư tiền mặt để mua bất động sản này!");
                        }
                    }
                    else if (currentMoney > currentAmount) {
                        alert("Bạn đã dùng dư tiền mặt để mua bất động sản này!");
                    }
                    else {
                        RemoveMask();
                        var data = $("#buy-new-asset-modal .form-horizontal").serialize();

                        $.ajax({
                            url: Url.BuyAsset,
                            type: "post",
                            data: data,
                            success: function (data) {
                                if (data.result > 0) {
                                    $("#buy-new-asset-modal").modal("hide");
                                    LoadTable();
                                }
                                else {
                                    alert("Có lỗi xảy ra");
                                }
                            }
                        })
                        MaskInput();
                    }
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
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
        var transaction_id = $(this).data("transaction-id");
        if (typeof transaction_id == 'undefined') {
            transaction_id = 0;
        }

        $.ajax({
            url: Url.LoadAsset,
            type: "get",
            data: { assetId: id, transactionId: transaction_id },
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
                if (data.result == -1) {
                    alert("Tên tài sản đã tồn tại, vui lòng nhập tên khác");
                }
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

    $(document).on("click", ".sell-asset-toggle", function () {
        var id = $(this).data("asset-id");

        $.ajax({
            url: Url.SellAssetModal,
            type: "get",
            data: { assetId: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".sell-asset-modal").html(data);
                    $("#sell-asset-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".sell-asset", function () {
        RemoveMask();
        var data = $("#sell-asset-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.SellAsset,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#sell-asset-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".create-new-liability", function () {
        var id = $(this).data("asset-id");
        var transaction_id = $(this).data("transaction-id");
        if (typeof transaction_id == 'undefined') {
            transaction_id = 0;
        }
        var type = $(this).data("asset-type");

        $.ajax({
            url: Url.LiabilityModal,
            type: "get",
            data: { assetId: id, type: type, transactionId: transaction_id },
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

        if ($("#create-new-liability-modal .form-horizontal #AssetId").val() > 0) {
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
        }
        else {
            $("#create-new-asset-modal .form-horizontal .liability-table")
        }
    })

    $(document).on("click", ".create-buy-liability", function () {
        var name = $("#liability-table input[name='Name']").val();
        var value = $("#liability-table input[name='Value']").val();
        var interestType = $("#liability-table select[name='InterestType']").val();
        var interestRate = $("#liability-table input[name='InterestRate']").val();
        var startDate = $("#liability-table input[name='StartDate']").val();
        var endDate = $("#liability-table input[name='EndDate']").val();
        var newRow = "<tr>"; 
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].Name'>" + name + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].Value'>" + value + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].InterestType'>" + interestType + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].InterestRate'>" + interestRate + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].StartDate'>" + startDate + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].EndDate'>" + endDate + "</td>";
        newRow += "<td></td>";
        newRow += "</tr>";
        RemoveMask();
        name = $("#liability-table input[name='Name']").val();
        value = $("#liability-table input[name='Value']").val();
        interestType = $("#liability-table select[name='InterestType']").val();
        interestRate = $("#liability-table input[name='InterestRate']").val();
        var dataRow = "<tr class='hidden'>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].Name' value='" + name + "'/></td>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].Value' value='" + value + "'/></td>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].InterestType' value='" + interestType + "'/></td>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].InterestRate' value='" + interestRate + "'/></td>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].StartDate' value='" + startDate + "'/></td>";
        dataRow += "<td><input name='Asset.Liabilities[" + liabilityCount + "].EndDate' value='" + endDate + "'/></td>";
        dataRow += "<td></td>";
        dataRow += "</tr>";
        MaskInput();
        $("#liability-table tbody").append(newRow);
        $("#liability-table tbody").append(dataRow);
        liabilityCount++;
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
        if (assetType == 5) {
            $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current + collapse + 9);
        }
        else {
            $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current + collapse);
        }
    })

    $(document).on("hidden.bs.collapse", "tr[class^='detail-']", function () {
        var rs_cls = $(this).closest("tr").attr("class").split(' ')[1];
        var child_cls = $(this).closest("tr").attr("class").split(' ')[2];
        var current = $(document).find("table tbody ." + rs_cls + ":not(.collapse)").length;
        var collapse = $(document).find("table tbody ." + child_cls + ".collapse").length;
        if (assetType == 5) {
            $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current - collapse + 9);
        }
        else {
            $(document).find("table tbody tr." + rs_cls + ":first td:nth-child(1)").attr("rowspan", current - collapse);
        }
    })

    $(document).on("click", ".btn-detail-view", function () {
        var type = $(this).data("asset-type");

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
    })

    $(document).on("click", ".btn-compact-view", function () {
        var type = $(this).data("asset-type");

        $.ajax({
            url: Url.LoadCompactTable,
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
    })
})