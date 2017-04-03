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

    $('#create-new-business-modal').on('hidden.bs.modal', function (e) {
        $(this)
          .find("input,textarea,select")
             .val('')
             .end()
          .find("input[type=checkbox], input[type=radio]")
             .prop("checked", "")
             .end();
        MaskInput();
    })

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
})