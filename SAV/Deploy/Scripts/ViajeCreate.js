$(document).ready(function () {

    $("#Guardar").click(function () { return ValidateForm(); });
    $("#DatosBasicosViaje_Servicio").change(function () { DisableRepecitiones(); ChangeOrigenDestino(); })

    ChangeOrigenDestino();
});

function ValidateForm() {
    var validForm = true;

    $("#Error").empty();

    if (!ValidateOrigenDestino())
        validForm = false;

    if (!ValidateRepeticiones())
        validForm = false;

    if (!ValidateFechas())
        validForm = false;

    try {
        $.validator(stopOnFailure, $("#Create"));
    }
    catch (err) {
        //Handle errors here
    }

    if (!validForm)
        disableSpinner();

    return validForm;
}

function ValidateOrigenDestino() {
    var Origen = $("#DatosBasicosViaje_SelectOrigen :selected").val();
    var Destino = $("#DatosBasicosViaje_SelectDestino :selected").val();

    if (Origen != "" && Destino != "" && Origen == Destino) {

        if (!$("#SelectOrigen").hasClass("input-validation-error"))
            $("#SelectOrigen").addClass("input-validation-error");

        if (!$("#SelectDestino").hasClass("input-validation-error"))
            $("#SelectDestino").addClass("input-validation-error");

        $("#Error").append("<span class='field-validation-error'><p>El origne y el destino deben tener valores diferentes</p></span>");
        return false;
    }
    return true;
}

function ValidateRepeticiones() {
    var checkbox = false;
    $("input:checkbox").each(function () {
        if ($(this).attr('checked'))
            checkbox = true;
    });

    if ($("#fechaRepeticion").val() == "" && checkbox) {
        $("#fechaRepeticion").addClass("input-validation-error");
        $("#Error").append("<span class='field-validation-error'><p>Debe ingresar una fecha fin de repetición</p></span>");
        return false;
    }
    if ($("#fechaRepeticion").val() != "" && !checkbox) {
        $("input:checkbox").addClass("input-validation-error");
        $("#Error").append("<span class='field-validation-error'><p>Debe ingresar uno o varios dias de repetición</p></span>");
        return false;
    }
    return true;
}

function ValidateFechas() {
    var fechaSalida = $("#fechaSalida");
    var fechaRepeticionFin = $("#fechaRepeticion");

    if ($(fechaSalida).val() != "" && $(fechaRepeticionFin).val() != "") {
        var parts = $(fechaSalida).val().split('/');
        fechaSalida = new Date(parts[2], parts[1], parts[0]);

        parts = $(fechaRepeticionFin).val().split('/');
        fechaRepeticionFin = new Date(parts[2], parts[1], parts[0]);

        if (fechaRepeticionFin <= fechaSalida) {
            $("#Error").append("<span class='field-validation-error'><p>La fecha fin repetición debe ser mayor a la fecha de salida</p></span>");

            if (!$(fechaSalida).hasClass("input-validation-error"))
                $(fechaSalida).addClass("input-validation-error");

            if (!$(fechaRepeticionFin).hasClass("input-validation-error"))
                $(fechaRepeticionFin).addClass("input-validation-error");

            return false;
        }
        return true;
    }
    return true;
}

function DisableRepecitiones()
{
    if ($("#DatosBasicosViaje_Servicio option:selected").val() == "Cerrado")
    {
        var repeticiones = $("#repeticion");
        $(repeticiones).slideUp("slow");
        $(":input", repeticiones).val("");
        $(":input", repeticiones).removeAttr('checked');
    }
    else
        $("#repeticion").slideDown("slow") 
}

function ChangeOrigenDestino() {

    var servicioAbierto = $("#ServicioAbierto");
    var servicioCerrado = $("#ServicioCerrado");

    if ($("#DatosBasicosViaje_Servicio option:selected").val() == "Cerrado") {
        $(servicioAbierto).slideUp("slow");
        $(servicioCerrado).slideDown("slow")
        $("select", servicioAbierto).val("");
    }
    else {
        $(servicioAbierto).slideDown("slow");
        $(servicioCerrado).slideUp("slow")
        $(":input", servicioAbierto).val("");
    }
}