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

function RefreshBusinessTable() {
    $.ajax({
        url: Url.BusinessTable,
        type: "get",
        success: function (data) {
            $("#business-table").html($(data).html());
            MaskInput();
        }
    })
}

function LoadLiabilityForm() {
    $(".modal-dialog").css("width", "1000px");
    $("#business-form").addClass("hidden");
    $("#liability-div").removeClass("hidden");
    var html = "<button type='button' class='btn btn-default return-business-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin kinh doanh</button>";
    html += "<button type='button' class='btn btn-primary business-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin kinh doanh</button>";;
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

    $(document).on("click", ".create-business", function () {
        var model = $("#business-form").serialize();
        $.ajax({
            url: Url.SaveBusiness,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#business-create-modal").modal("hide");
                    RefreshBusinessTable();
                }
                else {
                    $("#business-div").html($(data).html());
                    var html = "<button type='button' class='btn btn-success create-business'><span class='glyphicon glyphicon-ok'></span>Tạo kinh doanh</button>";
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
            var html = "<button type='button' class='btn btn-success create-business'><span class='glyphicon glyphicon-ok'></span>Tạo kinh doanh</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CreateBusiness,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#business-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-liability-info", function () {
        var model = $("#business-form").serialize();
        $.ajax({
            url: Url.CreateBusiness,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $(".modal-dialog").css("width", "1000px");
                    $("#business-form").addClass("hidden");
                    $("#liability-div").removeClass("hidden");
                    var html = "<button type='button' class='btn btn-default return-business-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin kinh doanh</button>";
                    html += "<button type='button' class='btn btn-primary business-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin kinh doanh</button>";;
                    $(".modal-footer").html(html);
                }
                else {
                    $("#business-div").html($(data).html());
                    MaskInput();
                }
            }
        });
    })

    $(document).on("click", ".return-business-info", function () {
        $(".modal-dialog").css("width", "400px");
        $("#business-form").removeClass("hidden");
        $("#liability-div").addClass("hidden");
        if ($("#IsInDebt").prop("checked")) {
            var html = "<button type='button' class='btn btn-default create-liability-info'><span class='glyphicon glyphicon-arrow-right'></span>Tạo các khoản vay</button>";
            $(".modal-footer").html(html);
        }
        else {
            var html = "<button type='button' class='btn btn-success create-business'><span class='glyphicon glyphicon-ok'></span>Tạo kinh doanh</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".business-summary", function () {
        $.ajax({
            url: Url.Confirm,
            type: "get",
            success: function (data) {
                $("#liability-div").addClass("hidden");
                $("#confirm-business-info").html($(data));
                $("#confirm-business-info").removeClass("hidden");
                var html = "<button type='button' class='btn btn-default return-business-liability-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin các khoản vay</button>";
                html += "<button type='button' class='btn btn-success create-business'><span class='glyphicon glyphicon-ok'></span>Tạo kinh doanh</button>";;
                $(".modal-footer").html(html);
            }
        });
    })

    $(document).on("click", ".return-business-liability-info", function () {
        $("#liability-div").removeClass("hidden");
        $("#confirm-business-info").addClass("hidden");
        var html = "<button type='button' class='btn btn-default return-business-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin kinh doanh</button>";
        html += "<button type='button' class='btn btn-success business-summary'><span class='glyphicon glyphicon-ok'></span>Tạo kinh doanh</button>";;
        $(".modal-footer").html(html);
    })

    $(document).on("click", ".remove-business-liability", function () {
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

    $(document).on("click", ".delete-business-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteLiability,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshBusinessTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".delete-business", function () {
        if (confirm("Bạn có muốn xóa kinh doanh này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteBusiness,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshBusinessTable();
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

    $(document).on("click", ".update-business", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.BusinessUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#business-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-business", function () {
        var model = $("#business-update-form").serialize();
        $.ajax({
            url: Url.BusinessUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#business-update-modal").modal("hide");
                    RefreshBusinessTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#business-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-business-liability", function () {
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

    $(document).on("click", ".save-update-business-liability", function () {
        var model = $("#liability-update-form").serialize();
        $.ajax({
            url: Url.LiabilityUpdateForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-update-modal").modal("hide");
                    RefreshBusinessTable();
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

    $(document).on("click", ".add-business-liability", function () {
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

    $(document).on("click", ".save-create-business-liability", function () {
        var model = $("#liability-form").serialize();
        $.ajax({
            url: Url.LiabilityForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-create-modal").modal("hide");
                    RefreshBusinessTable();
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
