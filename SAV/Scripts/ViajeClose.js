var ErrorClose = false;

$(document).ready(function () {

    $("#cerrar").click(function () {
        return cerrar();
    });

    $("#agregarGasto").click(function () {
        agregarGasto();
    });

    $("#modificar").click(function () {
        ModificarViaje();
    });

    $("#tablaPasajeros [class='check-box']").change(function () {
        actualizarInfoCliente();
    });

})

function cerrar()
{
    $("#newGasto").hide();
    $("form").resetValidation();
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
            success: function (data) { debugger; $('#partialViewGastos').html(data); $('#dialog').dialog('close'); disableSpinner(); },
            error: function (error) { alert(error); disableSpinner(); }
        });
    }

    $('#Patente, #Interno').prop('disabled', false);
}

function ModificarViaje() {

    if ($("form").valid()) {
        enableSpinner();

        $.ajax({
            async: true,
            url: '/Viaje/Actualizar',
            type: 'POST',
            data:
                {
                    "patente": $("#Patente").val(),
                    "interno": $("#Interno").val(),
                    "idViaje": $("#viajeID").val()
                },
            dataType: "html",
            success: function (data) {
                disableSpinner()
            },
            error: function (error) {
                addError(error, "ModificarViaje", "Hubo un error al modificar la información de Viaje"); disableSpinner();
            }
        });
    }
    $("#RazonSocial, #CUIT, #NroTicket, #Monto, #Comentario").prop('disabled', false);
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
        error: function (error) { debugger; addError(error, "EntregaRetiraComision", "Hubo un error al guardar la información de Comisiones"); ErrorClose = true; }
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

function actualizarInfoCliente()
{

    //Tr perteneciente al pasajero a modificar
    var contexto = $(event.target).closest("tr");

    //Obtengo la informacion del pasajero que se esta actualizando
    var datos = {
        "clienteViajeID": $("[name*='ClienteViajeID']", contexto).val(),
        "pago": $("[name*='Pago']:checked", contexto).length,
        "presente": $("[name*='Presente']:checked", contexto).length,
    };

    $.ajax({
        async: false,
        url: '/Viaje/setAsistenciaPasajeros',
        type: 'POST',
        data: datos,
        dataType: "html",
        success: function (data) { disableSpinner(); },
        error: function (error) { addError(error, "EntregaRetiraComision", "Hubo un error al registrar el pago o la asistencia del pasajero."); ErrorClose = true; }
    });
    $(this).closest("tr")
}

