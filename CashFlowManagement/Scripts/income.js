var CashFlowModal = function (data) {
    if(data == -1) {
        $("#status").html("Xin kết thúc giai đoạn trước trước khi tạo giai đoạn mới");
    }
    else if (data == -2) {
        $("#status").html("Thu nhập đã tồn tại trước đó");
    }
    else {
        $("#modal").html(data);
        $("#cashflow-modal").modal("show");
    }
}

$(document).ready(function () {
    function InitializeSelect(clear) {
        var $select = $(".select").selectize({
            create: true,
            sortField: 'text',
            render: {
                option_create: function (data, escape) {
                    return '<div class="create">Tạo <strong>' + escape(data.input) + '</strong>&hellip;</div>';
                }
            },
        });
        if (clear) {
            $select[0].selectize.setValue("");
        }
    }

    InitializeSelect(true);

    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value, "MM/YYYY", true).isValid();
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }
    $(".input-mask").mask("000.000.000.000.000", { reverse: true });
    $(".percentage").mask("##0,00%", { reverse: true });

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
    }

    function RefreshIncomeForm(id, msg) {
        $.ajax({
            url: Url.IncomeForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#income-form").html($(data).html());
                $("#status").html(msg);
                $.validator.unobtrusive.parse("#income-form");
                if (id > 0) {
                    InitializeSelect(false);
                }
                else {
                    InitializeSelect(true);
                }
            }
        })
    }

    function RefreshAvailableMoney() {
        $(".available-money").submit();
    }

    //function InitiateDatePicker() {
    //    $(".date-picker").datepicker({
    //        format: "mm/yyyy",
    //        minViewMode: 1,
    //        language: "vi-VN"
    //    });
    //}

    //MaskInput();
    //InitiateDatePicker();

    $(document).on("click", "input[type='checkbox']", function () {
        var row = $(this).closest("tr");
        if ($(this).is(":checked")) {
            $(".id").not(this).attr("checked", false);
            $(".asset-table tr").removeClass("success");
            $(this).closest("tr").addClass("success");
            var id = $(this).val();
            RefreshIncomeForm(id, null);
        }
        else {
            $(".asset-table tr").removeClass("success");
            RefreshIncomeForm();
        }
    })

    $(document).on("click", ".process-income", function () {
        var income = $("#income-form input,select").serialize();
        $.ajax({
            url: Url.ProcessIncome,
            type: "post",
            data: income,
            success: function (data) {
                $("#cashflow-modal").modal("hide");
                RefreshIncomeForm(0, data);
                RefreshAvailableMoney();
                $(".text-danger").html("");
                $("#table-ajax").submit();
            }
        })
    })

    $(document).on("click", ".delete-income-info", function () {
        var id = $(this).closest("tr").find(".id").val();
        $(".id").not(this).attr("checked", false);
        $(this).closest("tr").find(".id").prop("checked", true);
        $(".asset-table tr").removeClass("success");
        $(this).closest("tr").addClass("success");
        $.ajax({
            url: Url.CashflowDetail,
            type: "post",
            data: { incomeId: id },
            success: function (data) {
                $("#modal").html(data);
                $("#cashflow-modal").modal("show");
            }
        })
    })

    $(document).on("click", ".delete-income", function () {
        var id = $(".asset-table tr[class='success']").find(".id").val();
        $.ajax({
            url: Url.DeleteIncome,
            type: "post",
            data: { incomeId: id },
            success: function (data) {
                $("#cashflow-modal").modal("hide");
                RefreshIncomeForm(0, data);
                RefreshAvailableMoney();
                $("#table-ajax").submit();
            }
        })
    })

    $(document).on("click", ".create-income", function () {
        $(".id").attr("checked", false);
        $("#table-ajax tr").removeClass("success");
        $.ajax({
            url: Url.IncomeForm,
            type: "get",
            success: function (data) {
                $(".text-danger").html("");
                RefreshIncomeForm();
            }
        })
    })

    $(document).on("change", "#Name", function () {
        var name = $(this).find("option:selected").val();
        $.ajax({
            url: Url.GetStartDate,
            type: "post",
            data: { name: name },
            success: function (data) {
                $("#StartDate").val(data);
                var startDate = new moment(data, "MM/YYYY").toDate();
                $("#StartDate").datepicker("destroy");
                $("#StartDate").datepicker({
                    format: "mm/yyyy",
                    minViewMode: 1,
                    language: "vi-VN",
                    startDate: startDate
                });
            }
        })
    })

    $(document).on("click", ".toggle-form", function () {
        if ($(this).hasClass("glyphicon-circle-arrow-down")) {
            $(this).removeClass("glyphicon-circle-arrow-down").addClass("glyphicon-circle-arrow-up");
        }
        else {
            $(this).removeClass("glyphicon-circle-arrow-up").addClass("glyphicon-circle-arrow-down");
        }
    })

    $(document).on("click", ".toggle-confirmation", function () {;
        $.ajax({
            url: Url.InitializeModal,
            type: "get",
            success: function (data) {
                $("#confirmation-modal").html(data);
                $("#income-confirmation").modal("show");
            }
        })
    })
})