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

    $('#create-new-asset-modal').on('hidden.bs.modal', function (e) {
        $(this)
          .find("input,textarea,select")
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
            data: { assetId : id },
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

    $(document).on("click", ".delete-bank-deposit", function () {
        var id = $(this).closest("tr").find(".bank-deposit-id").text();
        if (confirm("Do you really want to delete this bank deposit?") == true) {
            $.ajax({
                url: Url.DeleteBankDeposit,
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