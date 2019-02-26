$(document).ready(function () {

    $("#DatosBasicosViaje_Servicio").change(function () { ChangeOrigenDestino(); });

    ChangeOrigenDestino();

    $('#modal').dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        height: '500',
        width: '1000',
        title: 'Boleto',
        acept: function (event, ui) {
            $('#modal').css('overflow', 'hidden');
        }
    });
});

function ChangeOrigenDestino() {
    var servicioAbierto = $("#ServicioAbierto");
    var servicioCerrado = $("#ServicioCerrado");

    if ($("#DatosBasicosViaje_Servicio option:selected").val() == "Cerrado") {
        $(servicioAbierto).slideUp("slow");
        $(servicioCerrado).slideDown("slow")
    }
    else {
        $(servicioAbierto).slideDown("slow");
        $(servicioCerrado).slideUp("slow")
    }
}

function Validar()
{
    debugger;
    $("#Error").empty();

    var asientos = $("#DatosBasicosViaje_Asientos").val();
    var clientes = $("#tablaPasajeros Tr").length - 1; //se resta 1 por la cabezara 

    if (asientos < clientes) {

        if (!$("#DatosBasicosViaje_Asientos").hasClass("input-validation-error"))
            $("#DatosBasicosViaje_Asientos").addClass("input-validation-error");

        $("#Error").append("<span class='field-validation-error'><p>La cantidad de asientos no puede ser menor a la canidad de pasajeros agregados al viaje.</p></span>");
        return false;
    }
    else if($(".field-validation-error").length == 0)
    {
        enableSpinner();
    }
}

function abrirBoleto(src) {
    var dialogDiv = $('#modal');
    $("#frame").attr("src", src);
    dialogDiv.dialog('open');
}

function abrirModal(Url) {
    var $dialog = $('<div style="overflow: hidden"; "></div>')
                   .html('<iframe style="border: 0px; " src="' + Url + '" width="100%" height="100%" scrolling="no"></iframe>')
                   .dialog({
                       autoOpen: false,
                       modal: true,
                       resizable: false,
                       height: 200,
                       width: 1010,
                       title: "Datos programación turística"
                   });
    $dialog.dialog('open');
}