$(document).ready(function () {

    if ($("#same").prop("checked")) {
        $("#shipping_address").hide(1000);
    }
    else {
        $("#shipping_address").show(1000);
    }

    $("#same").click(function () {
        if ($("#same").prop("checked")) {
            $("#shipping_address").hide(1000);
        }
        else {
            $("#shipping_address").show(1000);
        }
    });
});