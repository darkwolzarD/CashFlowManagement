$(document).ready(function () {
    function MaskInput() {
        $(".input-mask").mask("000,000,000,000,000", { reverse: true });
        $(".percentage").mask("##0,00%", { reverse: true });
        //$('.date').mask('00/00/0000');
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
        //$(".date").unmask();
    }

    MaskInput();

    $('#create-new-stock-modal, #create-new-transaction-modal').on('hidden.bs.modal', function (e) {
        $(this)
          .find("input,textarea,select")
             .val('')
             .end()
          .find("input[type=checkbox], input[type=radio]")
             .prop("checked", "")
             .end();
        MaskInput();
    })

    $(document).on("click", ".create-stock", function () {
        RemoveMask();
        var data = $("#create-new-stock-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateStockCode,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-stock-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".create-transaction", function () {
        RemoveMask();
        var data = $("#create-new-transaction-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.CreateTransaction,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#create-new-transaction-modal").modal("hide");
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
                    $(".stock-table").html(data);
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    }

    $(document).on("click", ".update-stock", function () {
        var id = $(this).data("stock-id");

        $.ajax({
            url: Url.LoadStockCode,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-modal").html(data);
                    $("#update-stock-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-stock", function () {
        RemoveMask();
        var data = $("#update-stock-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateStockCode,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-stock-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-stock", function () {
        var id = $(this).data("stock-id");
        if (confirm("Do you really want to delete this stock code?") == true) {
            $.ajax({
                url: Url.DeleteStockCode,
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

    $(document).on("click", ".create-new-transaction", function () {
        var id = $(this).data("stock-id");

        $.ajax({
            url: Url.TransactionModal,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".transaction-modal").html(data);
                    $("#create-new-transaction-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".update-transaction", function () {
        var id = $(this).data("transaction-id");

        $.ajax({
            url: Url.LoadTransaction,
            type: "get",
            data: { id: id },
            contentType: "html",
            success: function (data) {
                if (data.length > 0) {
                    $(".update-transaction-modal-div").html(data);
                    $("#update-transaction-modal").modal("show");
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    })

    $(document).on("click", ".save-transaction", function () {
        RemoveMask();
        var data = $("#update-transaction-modal .form-horizontal").serialize();

        $.ajax({
            url: Url.UpdateTransaction,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-transaction-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
        MaskInput();
    })

    $(document).on("click", ".delete-transaction", function () {
        var id = $(this).data("transaction-id");
        if (confirm("Do you really want to delete this transaction?") == true) {
            $.ajax({
                url: Url.DeleteTransaction,
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