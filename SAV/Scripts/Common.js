$(document).ready(function () {

    $(".fecha").click(function ()
    {
        $(this).val("");
    })

    $(".fecha").datepicker({
        buttonText: "Seleccione fecha",
        changeMonth: true,
        changeYear: true,
        showOn: "button",
        buttonImage: "../../img/calendar.png",
        buttonImageOnly: true,
        dateFormat: "dd/mm/yy"
    });

    $(".disableValidation").click(function () {
        $('input, select').prop('disabled', true);
    });
});