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

function LiabilityCreateSuccess(data) {
    if (data === "success") {
        RefreshLiabilityForm();
        RefreshLiabilityTable();
    }
    else {
        $("#liability-form").replaceWith($(data).html());
        MaskInput();
    }
}

function LiabilityUpdateSuccess(data) {
    if (data === "success") {
        RefreshLiabilityForm();
        RefreshLiabilityTable();
    }
    else {
        $("#liability-form").replaceWith($(data).html());
        MaskInput();
    }
}

function RefreshLiabilityTable() {
    $.ajax({
        url: Url._LiabilityTable,
        type: "get",
        success: function (data) {
            $("#liability-table-div").html($(data).html());
        }
    })
}

function RefreshLiabilityForm() {
    $.ajax({
        url: Url._LiabilityForm,
        type: "get",
        success: function (data) {
            $("#liability-form").replaceWith($(data).html());
            MaskInput();
        }
    })
}

function RefreshStockTable() {
    $.ajax({
        url: Url.StockTable,
        type: "get",
        success: function (data) {
            $("#stock-table").html($(data).html());
            MaskInput();
        }
    })
}

function LoadLiabilityForm() {
    $(".modal-dialog").css("width", "1000px");
    $("#stock-form").addClass("hidden");
    $("#liability-div").removeClass("hidden");
    var html = "<button type='button' class='btn btn-default return-stock-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin cổ phiếu</button>";
    html += "<button type='button' class='btn btn-primary stock-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin cổ phiếu</button>";;
    $(".modal-footer").html(html);
}


$(document).ready(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value, "MM/YYYY", true).isValid() || moment(value, "dd/MM/YYYY", true).isValid();
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
    }

    MaskInput();

    $(document).on("click", ".create-stock", function () {
        var model = $("#stock-form").serialize();
        $.ajax({
            url: Url.SaveStock,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#stock-create-modal").modal("hide");
                    RefreshStockTable();
                }
                else {
                    $("#stock-div").html($(data).html());
                    var html = "<button type='button' class='btn btn-success create-stock'><span class='glyphicon glyphicon-ok'></span>Tạo cổ phiếu</button>";
                    $(".modal-footer").html(html);
                }
            }
        });
    })

    $(document).on("click", "#IsInDebt", function () {
        if ($(this).prop("checked")) {
            var html = "<button type='button' class='btn btn-default create-liability-info'><span class='glyphicon glyphicon-arrow-right'></span>Tạo các khoản vay</button>";
            $(".modal-footer").html(html);
        }
        else {
            var html = "<button type='button' class='btn btn-success create-stock'><span class='glyphicon glyphicon-ok'></span>Tạo cổ phiếu</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CreateStock,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#stock-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-liability-info", function () {
        var model = $("#stock-form").serialize();
        $.ajax({
            url: Url.CreateStock,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $(".modal-dialog").css("width", "1000px");
                    $("#stock-form").addClass("hidden");
                    $("#liability-div").removeClass("hidden");
                    var html = "<button type='button' class='btn btn-default return-stock-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin cổ phiếu</button>";
                    html += "<button type='button' class='btn btn-primary stock-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin cổ phiếu</button>";;
                    $(".modal-footer").html(html);
                }
                else {
                    $("#stock-div").html($(data).html());
                    MaskInput();
                }
            }
        });
    })

    $(document).on("click", ".return-stock-info", function () {
        $(".modal-dialog").css("width", "400px");
        $("#stock-form").removeClass("hidden");
        $("#liability-div").addClass("hidden");
        if ($("#IsInDebt").prop("checked")) {
            var html = "<button type='button' class='btn btn-default create-liability-info'><span class='glyphicon glyphicon-arrow-right'></span>Tạo các khoản vay</button>";
            $(".modal-footer").html(html);
        }
        else {
            var html = "<button type='button' class='btn btn-success create-stock'><span class='glyphicon glyphicon-ok'></span>Tạo cổ phiếu</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".stock-summary", function () {
        $.ajax({
            url: Url.Confirm,
            type: "get",
            success: function (data) {
                $("#liability-div").addClass("hidden");
                $("#confirm-stock-info").html($(data));
                $("#confirm-stock-info").removeClass("hidden");
                var html = "<button type='button' class='btn btn-default return-stock-liability-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin các khoản vay</button>";
                html += "<button type='button' class='btn btn-success create-stock'><span class='glyphicon glyphicon-ok'></span>Tạo cổ phiếu</button>";;
                $(".modal-footer").html(html);
            }
        });
    })

    $(document).on("click", ".return-stock-liability-info", function () {
        $("#liability-div").removeClass("hidden");
        $("#confirm-stock-info").addClass("hidden");
        var html = "<button type='button' class='btn btn-default return-stock-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin cổ phiếu</button>";
        html += "<button type='button' class='btn btn-success stock-summary'><span class='glyphicon glyphicon-ok'></span>Tạo cổ phiếu</button>";;
        $(".modal-footer").html(html);
    })

    $(document).on("click", ".remove-stock-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này không")) {
            var id = $(this).closest("tr").find(".liability-id").val();
            $.ajax({
                url: Url.DeleteTempLiability,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshLiabilityTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".delete-stock-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteLiability,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshStockTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".delete-stock", function () {
        if (confirm("Bạn có muốn xóa cổ phiếu này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteStock,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshStockTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".liability-id", function () {
        if ($(this).prop("checked")) {
            var id = $(this).closest("tr").find(".liability-id").val();
            $.ajax({
                url: Url.LiabilityUpdateForm,
                type: "get",
                data: { id: id },
                success: function (data) {
                    $("#liability-form-div").html($(data).html());
                    MaskInput();
                }
            });
        }
        else {
            RefreshLiabilityForm();
        }
    })

    $(document).on("keyup", "form input", function () {
        $("form .field-validation-error").text("");
    })

    $(document).on("click", ".update-stock", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.StockUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#stock-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-stock", function () {
        var model = $("#stock-update-form").serialize();
        $.ajax({
            url: Url.StockUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#stock-update-modal").modal("hide");
                    RefreshStockTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#stock-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-stock-liability", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.LiabilityUpdateForm2nd,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#liability-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-stock-liability", function () {
        var model = $("#liability-update-form").serialize();
        $.ajax({
            url: Url.LiabilityUpdateForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-update-modal").modal("hide");
                    RefreshStockTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#liability-update-modal").html($(data).html());
                    MaskInput();
                }
            }
        });
    })

    $(document).on("click", ".add-stock-liability", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.LiabilityForm2nd,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#liability-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-create-stock-liability", function () {
        var model = $("#liability-form").serialize();
        $.ajax({
            url: Url.LiabilityForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-create-modal").modal("hide");
                    RefreshStockTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#liability-create-modal").html($(data).html());
                    MaskInput();
                }
            }
        });
    })
})
