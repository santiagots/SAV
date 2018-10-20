$(document).ready(function () {

    $(".fecha").click(function ()
    {
        $(this).val("");
    })
    debugger;
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

function AbrirPopUp(Url, Titulo, height, width, modal) {
    var $dialog = $('<div style="overflow: hidden"; "></div>')
                   .html('<iframe style="border: 0px; " src="' + Url + '" width="100%" height="100%" scrolling="no"></iframe>')
                   .dialog({
                       autoOpen: false,
                       modal: modal,
                       resizable: false,
                       height: height,
                       width: width,
                       title: Titulo
                   });
    $dialog.dialog('open');
}