$(document).ready(function () {
    var oldCurrentEditRow, CurrentEditRow;

    function ThousandSeparator(nStr) {
        nStr += '';
        var x = nStr.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + '.' + '$2');
        }
        return x1 + x2;
    }

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

    $('#create-new-salary-modal').on('hidden.bs.modal', function (e) {
        $(this)
          .find("input,textarea,select")
             .val('')
             .end()
          .find("input[type=checkbox], input[type=radio]")
             .prop("checked", "")
             .end();

        MaskInput();
    })

    $(document).on("click", ".create-salary", function () {
        RemoveMask();
        //var data = $("#create-new-salary-modal .form-horizontal").serialize
        var data = $("#create-new-salary input").serialize();

        $.ajax({
            url: Url.CreateSalary,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    //$("#create-new-salary-modal").modal("hide");
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
                    $(".salary-table").html(data);
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })
    }

    $(document).on("click", ".update-salary", function () {
        if (oldCurrentEditRow != null && CurrentEditRow != null) {
            CurrentEditRow.html(oldCurrentEditRow);
        }
        oldCurrentEditRow = $(this).closest("tr").html();
        CurrentEditRow = $(this).closest("tr");

        $(this).closest("tr").find("td").not(':last').each(function (index) {
            if (index == 0 || index == 2) {
                $(this).html("<input type='text' class='form-control input-mask edit' name='" + $(this).attr("name") + "' value='" + $(this).html() + "'/>");
            }
        })
        MaskInput();
        $(this).text("Lưu");
        $(this).attr("class", "btn btn-success save-salary");
        $(this).next("button").text("Hủy");
        $(this).next("button").attr("class", "btn btn-danger load-table");
    })

    $(document).on("click", ".save-salary", function () {
        RemoveMask();
        //var data = $("#update-salary-modal .form-horizontal").serialize();
        var data = $(this).closest("tr").find("input").serialize();

        $.ajax({
            url: Url.UpdateSalary,
            type: "post",
            data: data,
            success: function (data) {
                if (data.result > 0) {
                    $("#update-salary-modal").modal("hide");
                    LoadTable();
                }
                else {
                    alert("Có lỗi xảy ra");
                }
            }
        })

        MaskInput();
    })

    $(document).on("keyup", ".edit, .input", function () {
        var value = $(this).val().replace(/\./g, "");
        if (value == "") value = 0;
        $(this).closest("td").next("td").html(ThousandSeparator(value * 12));

        var total = parseInt(value);
        $(".salary-table tbody tr").not(":first").not(":last").each(function () {
            total += parseInt($(this).find("td:nth-child(3)").text().replace(/\./g, ""));
        })

        $(".salary-table tbody tr:first td:nth-child(2)").html("<strong>" + ThousandSeparator(total) + "</strong>");
        $(".salary-table tbody tr:first td:nth-child(3) ").html("<strong>" + ThousandSeparator(total * 12) + "</strong>");
    })

    $(document).on("click", ".delete-salary", function () {
        var id = $(this).closest("tr").find("td[name='Id']").text();
        if (confirm("Bạn có muốn xóa thu nhập từ lương này?") == true) {
            $.ajax({
                url: Url.DeleteSalary,
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

    $(document).on("click", ".load-table", function () {
        LoadTable();
    })
})