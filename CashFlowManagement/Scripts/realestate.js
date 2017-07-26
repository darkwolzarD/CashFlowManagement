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

function RefreshRealEstateTable() {
    $.ajax({
        url: Url.RealEstateTable,
        type: "get",
        success: function (data) {
            $("#real-estate-table").html($(data).html());
            MaskInput();
        }
    })
}

function LoadLiabilityForm() {
    $(".modal-dialog").css("width", "1000px");
    $("#real-estate-form").addClass("hidden");
    $("#liability-div").removeClass("hidden");
    var html = "<button type='button' class='btn btn-default return-real-estate-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin bất động sản</button>";
    html += "<button type='button' class='btn btn-primary real-estate-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin bất động sản</button>";;
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

    $(document).on("click", ".create-real-estate", function () {
        var model = $("#real-estate-form").serialize();
        $.ajax({
            url: Url.SaveRealEstate,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#real-estate-create-modal").modal("hide");
                    RefreshRealEstateTable();
                }
                else {
                    $("#real-estate-div").html($(data).html());
                    var html = "<button type='button' class='btn btn-success create-real-estate'><span class='glyphicon glyphicon-ok'></span>Tạo bất động sản</button>";
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
            var html = "<button type='button' class='btn btn-success create-real-estate'><span class='glyphicon glyphicon-ok'></span>Tạo bất động sản</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".toggle-modal", function () {
        $.ajax({
            url: Url.CreateRealEstate,
            type: "get",
            success: function (data) {
                $("#modal").html(data);
                $("#real-estate-create-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".create-liability-info", function () {
        var model = $("#real-estate-form").serialize();
        $.ajax({
            url: Url.CreateRealEstate,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $(".modal-dialog").css("width", "1000px");
                    $("#real-estate-form").addClass("hidden");
                    $("#liability-div").removeClass("hidden");
                    var html = "<button type='button' class='btn btn-default return-real-estate-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin bất động sản</button>";
                    html += "<button type='button' class='btn btn-primary real-estate-summary'><span class='glyphicon glyphicon-ok'></span>Xác nhận thông tin bất động sản</button>";;
                    $(".modal-footer").html(html);
                }
                else {
                    $("#real-estate-div").html($(data).html());
                    MaskInput();
                }
            }
        });
    })

    $(document).on("click", ".return-real-estate-info", function () {
        $(".modal-dialog").css("width", "400px");
        $("#real-estate-form").removeClass("hidden");
        $("#liability-div").addClass("hidden");
        if ($("#IsInDebt").prop("checked")) {
            var html = "<button type='button' class='btn btn-default create-liability-info'><span class='glyphicon glyphicon-arrow-right'></span>Tạo các khoản vay</button>";
            $(".modal-footer").html(html);
        }
        else {
            var html = "<button type='button' class='btn btn-success create-real-estate'><span class='glyphicon glyphicon-ok'></span>Tạo bất động sản</button>";
            $(".modal-footer").html(html);
        }
    })

    $(document).on("click", ".real-estate-summary", function () {
        $.ajax({
            url: Url.Confirm,
            type: "get",
            success: function (data) {
                $("#liability-div").addClass("hidden");
                $("#confirm-real-estate-info").html($(data));
                $("#confirm-real-estate-info").removeClass("hidden");
                var html = "<button type='button' class='btn btn-default return-real-estate-liability-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin các khoản vay</button>";
                html += "<button type='button' class='btn btn-success create-real-estate'><span class='glyphicon glyphicon-ok'></span>Tạo bất động sản</button>";;
                $(".modal-footer").html(html);
            }
        });
    })

    $(document).on("click", ".return-real-estate-liability-info", function () {
        $("#liability-div").removeClass("hidden");
        $("#confirm-real-estate-info").addClass("hidden");
        var html = "<button type='button' class='btn btn-default return-real-estate-info'><span class='glyphicon glyphicon-arrow-left'></span>Thông tin bất động sản</button>";
        html += "<button type='button' class='btn btn-success real-estate-summary'><span class='glyphicon glyphicon-ok'></span>Tạo bất động sản</button>";;
        $(".modal-footer").html(html);
    })

    $(document).on("click", ".remove-real-estate-liability", function () {
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

    $(document).on("click", ".delete-real-estate-liability", function () {
        if (confirm("Bạn có muốn xóa khoản nợ này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteLiability,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshRealEstateTable();
                    }
                    else {
                        swal("Thất bại", "Có lỗi xảy ra!", "error");
                    }
                }
            });
        }
    })

    $(document).on("click", ".delete-real-estate", function () {
        if (confirm("Bạn có muốn xóa bất động sản này không")) {
            var id = $(this).data("value");
            $.ajax({
                url: Url.DeleteRealEstate,
                type: "post",
                data: { id: id },
                success: function (data) {
                    if (data === "success") {
                        RefreshRealEstateTable();
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

    $(document).on("click", ".update-real-estate", function () {
        var id = $(this).data("value");
        $.ajax({
            url: Url.RealEstateUpdateForm,
            type: "get",
            data: { id: id },
            success: function (data) {
                $("#modal").html(data);
                $("#real-estate-update-modal").modal("show");
                MaskInput();
            }
        });
    })

    $(document).on("click", ".save-update-real-estate", function () {
        var model = $("#real-estate-update-form").serialize();
        $.ajax({
            url: Url.RealEstateUpdateForm,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#real-estate-update-modal").modal("hide");
                    RefreshRealEstateTable();
                }
                else if (data === "failed") {
                    swal("Thất bại", "Có lỗi xảy ra!", "error");
                }
                else {
                    $("#real-estate-update-modal").html($(data).html());
                }
            }
        });
    })

    $(document).on("click", ".update-real-estate-liability", function () {
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

    $(document).on("click", ".save-update-real-estate-liability", function () {
        var model = $("#liability-update-form").serialize();
        $.ajax({
            url: Url.LiabilityUpdateForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-update-modal").modal("hide");
                    RefreshRealEstateTable();
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

    $(document).on("click", ".add-real-estate-liability", function () {
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

    $(document).on("click", ".save-create-real-estate-liability", function () {
        var model = $("#liability-form").serialize();
        $.ajax({
            url: Url.LiabilityForm2nd,
            type: "post",
            data: model,
            success: function (data) {
                if (data === "success") {
                    $("#liability-create-modal").modal("hide");
                    RefreshRealEstateTable();
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
