$(document).ready(function () {
    $(document).on("click", ".reset-form", function () {
        $(this).closest("form").find("input[type!='hidden'],textarea").val('').end();
        $(this).closest("form").find("select").prop("selectedIndex", 0);
    })

    $(document).on("click", "#liability-table td", function () {
        $(this).closest("tr").find(".liability-id").trigger("click");
    })
})