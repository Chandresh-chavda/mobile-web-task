$(document).ready(function () {
    // Handle the button click event
    $("#myBtn").click(function () {
        $("#myModal").modal();
    });

    // Handle the submit event
    $("#submitBtn").click(function () {
        var message = $("#myInput").val();
        // Do something with the message, such as storing it in a variable or submitting it to a server using AJAX
    });
});
