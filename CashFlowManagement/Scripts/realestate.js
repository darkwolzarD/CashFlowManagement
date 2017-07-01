$(document).ready(function () {
    var count = 0;
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

    $(document).on("click", ".create-real-estate-liability", function () {
        var source = $("#Source").val();
        var value = $("#LiabilityValue").val();
        var interestType = $("#InterestType :selected").text();
        var interestRate = $("#InterestRate").val();
        var startDate = $("#StartDate").val();
        var endDate = $("#EndDate").val();
        var html = "<tr>";
        html += "<td>" + source + "</td>";
        html += "<td class='text-right'>" + value + "</td>";
        html += "<td>" + interestType + "</td>";
        html += "<td class='text-right'>" + interestRate + "</td>";
        html += "<td class='text-right'>" + startDate + "</td>";
        html += "<td class='text-right'>" + endDate + "</td>";
        html +="<td class='text-center'><button type='button' class='btn btn-danger'>Xóa nợ</button></td>";
        html += "</tr>";
        $("#liability-table tbody").append(html);
        count += 1;
    })

    $(document).on("click", "#IsInDept", function () {
        if ($(this).prop("checked")) {
            $("#real-estate-table").removeClass("col-md-4 col-md-offset-4").addClass("col-md-3");
            $("#liability-div").removeClass("hidden");
        }
        else {
            $("#real-estate-table").removeClass("col-md-3").addClass("col-md-4 col-md-offset-4");
            $("#liability-div").addClass("hidden");
        }
    })
})
