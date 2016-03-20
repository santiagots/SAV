var ErrorClose = false;

$(document).ready(function () {

    $("#cerrar").click(function () {
        return cerrar();
    });

    $("#agregarGasto").click(function () {
        agregarGasto();
    });

})

function cerrar()
{
    $("#newGasto").hide();
    $("form").resetValidation();
    setAsistenciaPasajerosAjax();
    setEntregaRetiraComisionAjax();
    if (!ErrorClose) {
        if (!confirmSpinner("Esta seguro que desea de cerrar el viaje?")) {
            disableSpinner();
            $("#newGasto").show();
            return false;
        }
    }
    else {
        disableSpinner();
        $("#newGasto").show();
        return false;
    }
}

function agregarGasto()
{
    if ($("form").valid()) {

        enableSpinner();

        $.ajax({
            async: true,
            url: '/Viaje/AddGasto',
            type: 'POST',
            data:
                {
                    "razonSocial": $("#RazonSocial").val(),
                    "cuit": $("#CUIT").val(),
                    "nroTicket": $("#NroTicket").val(),
                    "monto": $("#Monto").val(),
                    "comentario": $("#Comentario").val(),
                    "idViaje": $("#viajeID").val()
                },
            dataType: "html",
            success: function (data) { $('#dialog').dialog('close'); },
            error: function (error) { alert(error); disableSpinner(); }
        });
    }
}

function setAsistenciaPasajeros() {
    enableSpinner();
    setAsistenciaPasajerosAjax();
}

function setAsistenciaPasajerosAjax() {
    $.ajax({
        async: false,
        url: '/Viaje/setAsistenciaPasajeros',
        type: 'POST',
        data: $("#tablaPasajeros :input").serialize(),
        dataType: "json",
        success: function (data) { disableSpinner(); },
        error: function (error) { addError(error, "AsistenciaPasajeros", "Hubo un error al guardar la información de Pasajeros"); ErrorClose = true; }
    });
}

function setEntregaRetiraComision() {
    enableSpinner();
    setEntregaRetiraComisionAjax();
}

function setEntregaRetiraComisionAjax() {
    $.ajax({
        async: false,
        url: '/Viaje/setEntregaRetiraComision',
        type: 'POST',
        data: $("#tablaComisiones :input").serialize(),
        dataType: "json",
        success: function (data) { disableSpinner(); },
        error: function (error) { addError(error, "EntregaRetiraComision", "Hubo un error al guardar la información de Comisiones"); ErrorClose = true; }
    });
}

function addError(error, displayError, mesage) {
    if ($("#Error" + displayError).length == 0)
        $("#Error").append("<span id='Error" + displayError + "' class='field-validation-error'><p>" + mesage + "</p></span>");
}

function deleteGasto(idGasto, idViaje)
{
    $.ajax({
        async: false,
        url: '/Viaje/DeleteGasto',
        type: 'POST',
        data:   {
                "id": idGasto,
                "idViaje": idViaje
                },
        dataType: "html",
        success: function (data) { $('#partialViewGastos').html(data); disableSpinner(); },
        error: function (error) { addError(error, "deletGasto", "Hubo un error al eliminar el Gasto"); ErrorClose = true; }
    });
}

