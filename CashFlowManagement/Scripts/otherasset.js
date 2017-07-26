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

function RefreshOtherAssetTable() {
    $.ajax({
        url: Url.OtherAssetTable,
        type: "get",
        success: function (data) {
            $("#other-asset-table").html($(data).html());
            MaskInput();
        }
    })
}

function LoadLiabilityForm() {
    $(".modal-dialog").css("width", "1000px");
    $("#other-asset-form").addClass("hidden");
    $("#liability-div").removeClass("hidden");
    var html = "<button type='button' class='btn btn-default return-other-asset-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin tài sản</button>";
    html += "<button type='button' class='btn btn-primary other-asset-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin tài sản</button>";;
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

    $(document).on("click", ".create-other-asset", function () {
        var model = $("#other-asset-form").serialize();
        $.ajax({
            url: Url.SaveOtherAsset,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-asset-create-modal").modal("hide");
                    RefreshOtherAssetTable();
                }
                else {
                    $("#other-asset-div").html($(data).html());
                    var html = "<button type='button' class='btn btn-success create-other-asset'><span class='glyphicon glyphicon-ok'></span>Tạo tài sản</button>";
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
            var html = "<button type='button' class='btn btn-success create-other-asset'><span class='glyphicon glyphicon-ok'></span>Tạo tài sản</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CreateOtherAsset,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#other-asset-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-liability-info", function () {
        var model = $("#other-asset-form").serialize();
        $.ajax({
            url: Url.CreateOtherAsset,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $(".modal-dialog").css("width", "1000px");
                    $("#other-asset-form").addClass("hidden");
                    $("#liability-div").removeClass("hidden");
                    var html = "<button type='button' class='btn btn-default return-other-asset-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin tài sản</button>";
                    html += "<button type='button' class='btn btn-primary other-asset-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin tài sản</button>";;
                    $(".modal-footer").html(html);
                }
                else {
                    $("#other-asset-div").html($(data).html());
                    MaskInput();
                }
            }
        });
    })

    $(document).on("click", ".return-other-asset-info", function () {
        $(".modal-dialog").css("width", "400px");
        $("#other-asset-form").removeClass("hidden");
        $("#liability-div").addClass("hidden");
        if ($("#IsInDebt").prop("checked")) {
            var html = "<button type='button' class='btn btn-default create-liability-info'><span class='glyphicon glyphicon-arrow-right'></span>Tạo các khoản vay</button>";
            $(".modal-footer").html(html);
        }
        else {
            var html = "<button type='button' class='btn btn-success create-other-asset'><span class='glyphicon glyphicon-ok'></span>Tạo tài sản</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".other-asset-summary", function () {
        $.ajax({
            url: Url.Confirm,
            type: "get",
            success: function (data) {
                $("#liability-div").addClass("hidden");
                $("#confirm-other-asset-info").html($(data));
                $("#confirm-other-asset-info").removeClass("hidden");
                var html = "<button type='button' class='btn btn-default return-other-asset-liability-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin các khoản vay</button>";
                html += "<button type='button' class='btn btn-success create-other-asset'><span class='glyphicon glyphicon-ok'></span>Tạo tài sản</button>";;
                $(".modal-footer").html(html);
            }
        });
    })

    $(document).on("click", ".return-other-asset-liability-info", function () {
        $("#liability-div").removeClass("hidden");
        $("#confirm-other-asset-info").addClass("hidden");
        var html = "<button type='button' class='btn btn-default return-other-asset-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin tài sản</button>";
        html += "<button type='button' class='btn btn-success other-asset-summary'><span class='glyphicon glyphicon-ok'></span>Tạo tài sản</button>";;
        $(".modal-footer").html(html);
    })

    $(document).on("click", ".remove-other-asset-liability", function () {
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

    $(document).on("click", ".delete-other-asset-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteLiability,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshOtherAssetTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".delete-other-asset", function () {
        if (confirm("Bạn có muốn xóa tài sản này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteOtherAsset,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshOtherAssetTable();
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

    $(document).on("click", ".update-other-asset", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.OtherAssetUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#other-asset-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-other-asset", function () {
        var model = $("#other-asset-update-form").serialize();
        $.ajax({
            url: Url.OtherAssetUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#other-asset-update-modal").modal("hide");
                    RefreshOtherAssetTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#other-asset-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-other-asset-liability", function () {
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

    $(document).on("click", ".save-update-other-asset-liability", function () {
        var model = $("#liability-update-form").serialize();
        $.ajax({
            url: Url.LiabilityUpdateForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-update-modal").modal("hide");
                    RefreshOtherAssetTable();
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

    $(document).on("click", ".add-other-asset-liability", function () {
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

    $(document).on("click", ".save-create-other-asset-liability", function () {
        var model = $("#liability-form").serialize();
        $.ajax({
            url: Url.LiabilityForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-create-modal").modal("hide");
                    RefreshOtherAssetTable();
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

    $(document).on("click", ".asset-toggle-summary", function () {
        $.ajax({
            url: Url.AssetSummary,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#asset-summary-modal").modal("show");
                MaskInput();
            }
        });
    })
})
