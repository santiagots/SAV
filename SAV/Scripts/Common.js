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

    //manejo de tabs

    debugger;
    $('#tabs').find('div.tabContent').hide();
    $('#tabs').find('div.tabContent:first').show();

    $('#tabs .tabItem').click(function () {

        $('#tabs .tabItem').removeClass('selected');
        $(this).addClass('selected');

        $('#tabs div.tabContent').hide();

        var tabActivo = $(this).find('a').attr('href');
        $(tabActivo).show();
    })
});