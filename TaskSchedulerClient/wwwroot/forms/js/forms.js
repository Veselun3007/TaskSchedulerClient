$(".toggle-password").click(function () {

    $(this).toggleClass("mdi mdi-eye mdi mdi-eye-off");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
