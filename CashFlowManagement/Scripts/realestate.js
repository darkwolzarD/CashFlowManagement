function RefreshLiabilityTable() {
    $.ajax({
        url: Url._LiabilityTable,
        type: "get",
        success: function (data) {
            $("#liability-form")[0].reset();
            $("#liability-table-div").html($(data).html());
        }
    })
}

$(document).ready(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value, "MM/YYYY", true).isValid();
    }

    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }

    function MaskInput() {
        $(".input-mask").mask("000.000.000.000.000", { reverse: true });
        $(".percentage").mask("##0,00%", { reverse: true });
        $(".date-picker-with-day").datepicker({
            format: "dd/mm/yyyy",
            minViewMode: 1,
            language: "vi-VN"
        });
        $(".date-picker").datepicker({
            format: "mm/yyyy",
            minViewMode: 1,
            language: "vi-VN"
        });
    }

    function RemoveMask() {
        $(".input-mask").unmask();
        $(".percentage").unmask();
    }
    
    MaskInput();

    $(document).on("click", ".create-real-estate", function () {
        var model = $("#real-estate-form").serialize();
        $.ajax({
            url: Url.CreateRealEstate,
            type: "post",
            data: model,
            success: function (data) {
                alert(data);
            }
        });
    })

    $(document).on("click", "#IsInDept", function () {
        if ($(this).prop("checked")) {
            $("#real-estate-div").removeClass("col-md-6 col-md-offset-3").addClass("col-md-3");
            $("#liability-div").removeClass("hidden");
        }
        else {
            $("#real-estate-div").removeClass("col-md-3").addClass("col-md-6 col-md-offset-3");
            $("#liability-div").addClass("hidden");
        }
    })
})
