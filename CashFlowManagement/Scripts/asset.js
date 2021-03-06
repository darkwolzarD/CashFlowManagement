﻿$(document).ready(function () {
    var liabilityCount = 0;
    var oldRow = "";

    function MaskInput() {
        $(".input-mask").mask("000.000.000.000.000", { reverse: true });
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

        $(".date-picker-2").datepicker({
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

    $(document).on("shown.bs.modal", "#create-new-asset-modal, #update-asset-modal", function () {
        MaskInput();
        InitiateDatePicker();
        if (assetType == 5 || assetType == 3) {
            var stock = $("#update-asset-modal #Asset_AssetName").val();
            if (stock != null) {
                $.ajax({
                    url: Url.CheckRemainedStock,
                    type: "post",
                    data: { stock: stock },
                    success: function (data) {
                        if (data.result > 0) {
                            $("#update-asset-modal #RemainedStock").val(data.result);
                        }
                        else if (data.result === -1) {
                            $("#update-asset-modal #RemainedStock").val(0);
                        }
                        else {
                            alert("Có lỗi xảy ra");
                        }
                        $("#update-asset-modal #RemainedStock").unmask();
                    }
                })
            }
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

    $(document).on("change", "#buy-new-asset-modal #Transaction_TransactionDate", function () {
        var date = $(this).val();
        if (moment(date, "dd/MM/yyyy").isValid()) {
            $.ajax({
                url: Url.CheckAvailableMoney,
                type: "post",
                data: { date: date },
                success: function (data) {
                    $("#buy-new-asset-modal #CurrentAvailableMoney").val(data);
                    $("#buy-new-asset-modal #CurrentAvailableMoney").unmask();
                }
            });
        }
        else {
            $("#buy-new-asset-modal #CurrentAvailableMoney").val(0);
            $("#buy-new-asset-modal #CurrentAvailableMoney").unmask();
        }
    })

    $(document).on("change", "#buy-new-asset-modal #Asset_StartDate", function () {
        var date = $(this).val();
        if (moment(date, "dd/MM/yyyy").isValid()) {
            $.ajax({
                url: Url.CheckAvailableMoney,
                type: "post",
                data: { date: date },
                success: function (data) {
                    $("#buy-new-asset-modal #CurrentAvailableMoney").val(data);
                }
            });
        }
        else {
            $("#buy-new-asset-modal #CurrentAvailableMoney").val(0);
        }
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
            .find("input[type!='hidden']input[name!='Transaction.TransactionDate']input[name!='CurrentAvailableMoney'],textarea")
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
        RemoveMask();
        var currentMoney = $("#buy-new-asset-modal #CurrentAvailableMoney").val();
        var assetValue = 0;
        if (assetType == 4) {
            assetValue = $("#buy-new-asset-modal input[name='Asset.Value']").val();
        }
        else {
            assetValue = parseInt($("#buy-new-asset-modal input[name='Transaction.NumberOfShares']").val() * $("#buy-new-asset-modal input[name='Transaction.SpotPrice']").val() * 1.0015);
        }
        if (assetType == 4 || assetType == 5) {
            var currentAmount = parseFloat($("#buy-new-asset-modal input[name='BuyAmount']").val());
            if (currentAmount == "") {
                currentAmount == 0;
            }
            var currentLiabilities = 0;
            $("#liability-table tbody tr:hidden").each(function (index, element) {
                var liability = parseFloat($(element).find("td:nth-child(2) input").val());
                if (liability == "") {
                    liability == 0;
                }
                currentLiabilities += liability;
            });
            MaskInput();
            if (currentAmount == "" || currentAmount == 0) {
                alert("Vui lòng nhập số tiền mua tài sản!");
            }
            else if (currentAmount + currentLiabilities == assetValue) {
                if (currentMoney < currentAmount) {
                    if (currentLiabilities == 0) {
                        alert("Không đủ tiền mặt để mua tài sản này!");
                    }
                    else if (currentLiabilities > 0 && (currentAmount + currentLiabilities < currentMoney)) {
                        alert("Bạn chưa vay đủ tiền mặt để mua tài sản này!");
                    }
                    else if (currentLiabilities > 0 && (currentAmount + currentLiabilities > currentMoney)) {
                        alert("Bạn đã vay dư tiền mặt để mua bất động sản này!");
                    }
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
            else if (currentAmount + currentLiabilities < assetValue) {
                alert("Bạn chưa vay đủ tiền mặt để mua tài sản này!");
            }
            else if (currentAmount + currentLiabilities > assetValue) {
                alert("Bạn đã vay dư tiền mặt để mua bất động sản này!");
            }
        }
        else if (assetType == 3) {
            var assetValue = parseInt($("#buy-new-asset-modal input[name='Asset.Value']").val());
            var currentMoney = parseInt($("#buy-new-asset-modal #CurrentAvailableMoney").val());
            if (assetValue > currentMoney) {
                alert("Tài khoản vượt quá số tiền sẵn có!");
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
        var transaction_id = $(this).data("transaction-id");
        if (typeof transaction_id == 'undefined') {
            transaction_id = 0;
        }

        var message = "Bạn có muốn xóa tài sản này?";
        if (transaction_id > 0) {
            message = "Bạn có muốn xóa giao dịch này?";
        }

        if (confirm(message) === true) {
            $.ajax({
                url: Url.DeleteAsset,
                type: "POST",
                data: { assetId: id, transactionId: transaction_id },
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
        if (typeof id == 'undefined') {
            id = 0;
        }

        $.ajax({
            url: Url.SellAssetModal,
            type: "get",
            data: { assetId: id, assetType: assetType },
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
                if (data.result == -1) {
                    alert("Cổ phiếu này không tồn tại");
                }
                else if (data.result == -2) {
                    alert("Không đủ số lượng để bán");
                }
                else if (data.result > 0) {
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
        var newRow = "<tr class='" + liabilityCount + "'>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].Name'>" + name + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].Value'>" + value + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].InterestType'>" + interestType + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].InterestRate'>" + interestRate + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].StartDate'>" + startDate + "</td>";
        newRow += "<td name='Asset.Liabilities[" + liabilityCount + "].EndDate'>" + endDate + "</td>";
        newRow += "<td class='text-center' width='180'><button type='button' class='btn btn-success edit-lib'><span class='glyphicon glyphicon-pencil'></span>Cập nhật</button><button type='button' class='btn btn-danger delete-lib'><span class='glyphicon glyphicon-remove'></span>Xóa</button></td>";
        newRow += "</tr>";
        RemoveMask();
        name = $("#liability-table input[name='Name']").val();
        value = $("#liability-table input[name='Value']").val();
        interestType = $("#liability-table select[name='InterestType']").val();
        interestRate = $("#liability-table input[name='InterestRate']").val();
        var dataRow = "<tr class='" + liabilityCount + " hidden'>";
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

    $(document).on("click", ".edit-lib", function () {
        var row = $(this).closest("tr");
        oldRow = row.html();
        var currentCount = row.attr("class");
        var dataRow = $("#liability-table ." + currentCount + ":last");
        var html = "<td><input class='form-control input-sm'value='" + dataRow.find("td:nth-child(1) input").val() + "'/></td>";
        html += "<td><input class='form-control input-sm input-mask' value='" + dataRow.find("td:nth-child(2) input").val() + "'/></td>";
        if (dataRow.find("td:nth-child(3) input").val() == 1) {
            html += "<td><select class='form-control input-sm'><option value='1' selected>Cố định</option><option value='2'>Giảm dần</option></select></td>";
        }
        else {
            html += "<td><select class='form-control input-sm'><option value='1' selected>Cố định</option><option value='2' selected>Giảm dần</option></select></td>";
        }
        html += "<td><input class='form-control input-sm percentage' value='" + dataRow.find("td:nth-child(4) input").val() + "'/></td>";
        html += "<td><input class='form-control input-sm date-picker' value='" + dataRow.find("td:nth-child(5) input").val() + "'/></td>";
        html += "<td><input class='form-control input-sm date-picker' value='" + dataRow.find("td:nth-child(6) input").val() + "'/></td>";
        html += "<td class='text-center' width='180'><button type='button' class='btn btn-success save-edit-lib'><span class='glyphicon glyphicon-save'></span>Lưu</button><button type='button' class='btn btn-danger cancel-edit-lib'><span class='glyphicon glyphicon-remove'></span>Hủy</button></td>";
        row.html(html);
        InitiateDatePicker();
    })

    $(document).on("click", ".cancel-edit-lib", function () {
        $(this).closest("tr").html(oldRow);
        oldRow = "";
    })

    $(document).on("click", ".save-edit-lib", function () {
        $(this).closest("tr").html(oldRow);
        oldRow = "";
    })

    $(document).on("click", ".delete-lib", function () {
        var currentCount = $(this).closest("tr").attr("class");
        $("#liability-table ." + currentCount).remove();
        liabilityCount -= 1;
        var count = 0;
        $("#liability-table tbody tr").each(function (idx, row) {
            var cls = $(row).attr("class");
            var rowClass = $(row).attr("class");
            if (rowClass != undefined) {
                rowClass = rowClass.split(" ")[0];
                $(row).find("td").each(function (index, element) {
                    if ($(row).hasClass("hidden")) {
                        var name = $(element).find("input").attr("name");
                        if (name != undefined) {
                            name = name.replace(rowClass, count);
                            $(element).find("input").attr("name", name);
                            $(row).removeClass().addClass("" + count).addClass("hidden");
                        }
                    }
                    else {
                        var name = $(element).attr("name");
                        if (name != undefined) {
                            name = name.replace(rowClass, count);
                            $(element).attr("name", name);
                            $(row).removeClass(rowClass).addClass("" + count);
                        }
                    }
                })
                if ($(row).hasClass("hidden")) {
                    count++;
                }
            }
        });
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

    $(document).on("keyup", "#sell-asset-modal #Asset_AssetName", function () {
        var stock = $(this).val();

        $.ajax({
            url: Url.CheckRemainedStock,
            type: "post",
            data: { stock: stock },
            success: function (data) {
                if (data.result > 0) {
                    $("#sell-asset-modal #RemainedStock").val(data.result);
                }
                else if (data.result === -1) {
                    $("#sell-asset-modal #RemainedStock").val(0);
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })


    $(document).on("keyup", "#buy-new-asset-modal #BuyAmount", function () {
        RemoveMask();
        var numberOfShares = $("#buy-new-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#buy-new-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0015);
        $("#buy-new-asset-modal #BuyValue").val(value);
        var currentAmount = $("#buy-new-asset-modal input[name='BuyAmount']").val();
        $("#buy-new-asset-modal #AfterMoney").val($("#buy-new-asset-modal #CurrentAvailableMoney").val() - $("#buy-new-asset-modal #BuyAmount").val());

        if (currentAmount == "") {
            currentAmount = 0;
        }
        else {
            currentAmount = parseInt(currentAmount);
        }

        var currentLiabilities = 0;
        $("#liability-table tbody tr:hidden").each(function (index, element) {
            var liability = parseFloat($(element).find("td:nth-child(2) input").val());
            if (liability == "") {
                liability == 0;
            }
            currentLiabilities += liability;
        });

        if (value >= currentAmount + currentLiabilities) {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(value - currentAmount - currentLiabilities);
        }
        else {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(0);
        }

        MaskInput();
    })

    $(document).on("keyup", "#sell-asset-modal #Transaction_NumberOfShares, #sell-asset-modal #Transaction_SpotPrice", function () {
        RemoveMask();
        var numberOfShares = $("#sell-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#sell-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0025);
        $("#sell-asset-modal #SellValue").val(value);

        MaskInput();
    })

    $(document).on("keyup", "#buy-new-asset-modal #Transaction_NumberOfShares, #buy-new-asset-modal #Transaction_SpotPrice", function () {
        RemoveMask();
        var numberOfShares = $("#buy-new-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#buy-new-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0015);
        var availableMoney = $("#buy-new-asset-modal #CurrentAvailableMoney").val();

        $("#buy-new-asset-modal #Transaction_Value").val(value);

        if (value <= availableMoney) {
            $("#buy-new-asset-modal input[name='BuyAmount']").val(value);
            $("#buy-new-asset-modal #AfterMoney").val(availableMoney - $("#buy-new-asset-modal #BuyAmount").val());
            var currentAmount = $("#buy-new-asset-modal input[name='BuyAmount']").val();
            if (currentAmount == "" || currentAmount == 0) {
                currentAmount = 0;
            }
            else {
                currentAmount = parseInt(currentAmount);
            }

            var currentLiabilities = 0;
            $("#liability-table tbody tr:hidden").each(function (index, element) {
                var liability = parseFloat($(element).find("td:nth-child(2) input").val());
                if (liability == "") {
                    liability == 0;
                }
                currentLiabilities += liability;
            });

            if (availableMoney < currentAmount + currentLiabilities) {
                $("#liability-table tbody tr:first td:nth-child(2) input").val(currentAmount + currentLiabilities - availalbleMoney);
            }
            else {
                $("#liability-table tbody tr:first td:nth-child(2) input").val(0);
            }
        }
        else {
            $("#buy-new-asset-modal input[name='BuyAmount']").val(availableMoney);
            $("#buy-new-asset-modal #AfterMoney").val(0);
            $("#liability-table tbody tr:first td:nth-child(2) input").val(value - availableMoney);
        }

        MaskInput();
    })

    $(document).on("keyup", "#update-asset-modal #Transaction_NumberOfShares, #update-asset-modal #Transaction_SpotPrice", function () {
        RemoveMask();
        var numberOfShares = $("#update-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#update-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0015);
        var availableMoney = $("#update-asset-modal #CurrentAvailableMoney").val();

        if (value <= availableMoney) {
            $("#update-asset-modal #Transaction_Value").val(value);
            $("#update-asset-modal #Transaction_Assets1_Value").val(value);
            $("#update-asset-modal #AfterMoney").val(availableMoney - $("#update-asset-modal #Transaction_Assets1_Value").val());
            var currentAmount = $("#update-asset-modal input[name='BuyAmount']").val();
            if (currentAmount == "" || currentAmount == 0) {
                currentAmount = 0;
            }
            else {
                currentAmount = parseInt(currentAmount);
            }

            var currentLiabilities = 0;
            $("#liability-table tbody tr:hidden").each(function (index, element) {
                var liability = parseFloat($(element).find("td:nth-child(2) input").val());
                if (liability == "") {
                    liability == 0;
                }
                currentLiabilities += liability;
            });

            var availalbleMoney = $("#update-asset-modal #CurrentAvailableMoney").val();

            if (availalbleMoney < currentAmount + currentLiabilities) {
                $("#liability-table tbody tr:first td:nth-child(2) input").val(currentAmount + currentLiabilities - availalbleMoney);
            }
            else {
                $("#liability-table tbody tr:first td:nth-child(2) input").val(0);
            }
        }
        else {
            $("#update-asset-modal #Transaction_Value").val(availableMoney);
            $("#update-asset-modal #AfterMoney").val(0);
            $("#liability-table tbody tr:first td:nth-child(2) input").val(value - availableMoney);
        }

        MaskInput();
    })

    $(document).on("keyup", "#update-asset-modal #Transaction_Assets1_Value", function () {
        RemoveMask();
        var numberOfShares = $("#update-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#update-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0015);
        $("#update-asset-modal #Transaction_Value").val(value);

        $("#update-asset-modal #AfterMoney").val($("#update-asset-modal #CurrentAvailableMoney").val() - $("#update-asset-modal #Transaction_Assets1_Value").val());
        var currentAmount = $("#update-asset-modal input[name='BuyAmount']").val();
        if (currentAmount == "" || currentAmount == 0) {
            currentAmount = 0;
        }
        else {
            currentAmount = parseInt(currentAmount);
        }

        var currentLiabilities = 0;
        $("#liability-table tbody tr:hidden").each(function (index, element) {
            var liability = parseFloat($(element).find("td:nth-child(2) input").val());
            if (liability == "") {
                liability == 0;
            }
            currentLiabilities += liability;
        });

        var availalbleMoney = $("#update-asset-modal #CurrentAvailableMoney").val();

        if (availalbleMoney < currentAmount + currentLiabilities) {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(currentAmount + currentLiabilities - availalbleMoney);
        }
        else {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(0);
        }

        MaskInput();
    })


    $(document).on("keyup", "#update-asset-modal #Transaction_NumberOfShares, #update-asset-modal #Transaction_SpotPrice", function () {
        RemoveMask();
        var numberOfShares = $("#update-asset-modal #Transaction_NumberOfShares").val();
        if (numberOfShares == "") {
            numberOfShares = 0;
        }
        var spotPrice = $("#update-asset-modal #Transaction_SpotPrice").val();
        if (spotPrice == "") {
            spotPrice = 0;
        }

        var value = parseInt(numberOfShares * spotPrice * 1.0015);
        $("#update-asset-modal #Transaction_Value").val(value);

        $("#update-asset-modal #Transaction_Assets1_Value").val(value);
        $("#update-asset-modal #AfterMoney").val($("#update-asset-modal #CurrentAvailableMoney").val() - $("#update-asset-modal #Transaction_Assets1_Value").val());
        var currentAmount = $("#update-asset-modal input[name='BuyAmount']").val();
        if (currentAmount == "" || currentAmount == 0) {
            currentAmount = 0;
        }
        else {
            currentAmount = parseInt(currentAmount);
        }

        var currentLiabilities = 0;
        $("#liability-table tbody tr:hidden").each(function (index, element) {
            var liability = parseFloat($(element).find("td:nth-child(2) input").val());
            if (liability == "") {
                liability == 0;
            }
            currentLiabilities += liability;
        });

        var availalbleMoney = $("#update-asset-modal #CurrentAvailableMoney").val();

        if (availalbleMoney < currentAmount + currentLiabilities) {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(currentAmount + currentLiabilities - availalbleMoney);
        }
        else {
            $("#liability-table tbody tr:first td:nth-child(2) input").val(0);
        }

        MaskInput();
    })

    $(document).on("keyup", "#buy-new-asset-modal #Asset_Value", function () {
        RemoveMask();
        $("#buy-new-asset-modal #AfterMoney").val($("#buy-new-asset-modal #CurrentAvailableMoney").val() - $("#buy-new-asset-modal #Asset_Value").val());
        MaskInput();
    })

    //$(document).on("keyup", "#update-asset-modal #Transaction_NumberOfShares, #update-asset-modal #Transaction_SpotPrice", function () {
    //    RemoveMask();
    //    var transactionType = $("#update-asset-modal #Transaction_TransactionType").val();

    //    var numberOfShares = $("#update-asset-modal #Transaction_NumberOfShares").val();
    //    if (numberOfShares == "") {
    //        numberOfShares = 0;
    //    }
    //    var spotPrice = $("#update-asset-modal #Transaction_SpotPrice").val();
    //    if (spotPrice == "") {
    //        spotPrice = 0;
    //    }

    //    if (transactionType == 2) {

    //    }
    //    else if (transactionType == 3) {
    //        var value = parseInt(numberOfShares * spotPrice * 1.0025);
    //        $("#update-asset-modal #Transaction_Value").val(value); 
    //    } 

    //    MaskInput();
    //})

    $(document).on("change", "#create-new-asset-modal #Asset_StartDate, #create-new-asset-modal #Asset_Term", function () {
        RemoveMask();
        var startDate = moment($("#create-new-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY");
        var valid = moment($("#create-new-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY").isValid();
        if (valid) {
            var term = parseInt($("#create-new-asset-modal #Asset_Term").val());
            var endDate = startDate.add(term, 'months');
            $("#create-new-asset-modal #Asset_EndDate").val(endDate.format("DD/MM/YYYY"));
            MaskInput();
        }
    })

    $(document).on("change", "#update-asset-modal #Asset_StartDate, #update-asset-modal #Asset_Term", function () {
        RemoveMask();
        var valid = moment($("#update-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY").isValid();
        if (valid) {
            var startDate = moment($("#update-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY");
            var term = parseInt($("#update-asset-modal #Asset_Term").val());
            var endDate = startDate.add(term, 'months');
            $("#update-asset-modal #Asset_EndDate").val(endDate.format("DD/MM/YYYY"));
            MaskInput();
        }
    })

    $(document).on("change", "#buy-new-asset-modal #Asset_StartDate, #buy-new-asset-modal #Asset_Term", function () {
        RemoveMask();
        var valid = moment($("#buy-new-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY").isValid();
        if (valid) {
            var startDate = moment($("#buy-new-asset-modal #Asset_StartDate").val(), "DD/MM/YYYY");
            var term = parseInt($("#buy-new-asset-modal #Asset_Term").val());
            var endDate = startDate.add(term, 'months');
            $("#buy-new-asset-modal #Asset_EndDate").val(endDate.format("DD/MM/YYYY"));
            MaskInput();
        }
    })

    $(document).on("click", ".toggle-confirmation", function () {;
        $.ajax({
            url: Url.InitializeModal,
            type: "get",
            success: function (data) {
                $("#confirmation-modal").html(data);
                $("#asset-confirmation").modal("show");
            }
        })
    })
})