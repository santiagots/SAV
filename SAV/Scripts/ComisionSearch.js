$(document).ready(function () {
    addEventToEnvio();
    $("#envio").click(function () { refrescarTab('SearchPagingComisionEnvios', 'envioTable') });
    $("#pendiente").click(function () { refrescarTab('SearchPagingComisionEnProgreso', 'pendienteTable'); });
    $("#finalizada").click(function () { refrescarTab('SearchPagingComisionFinalizadas', 'finalizadaTable') });

    $('#modal').dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        height: 'auto',
        width: 'auto',
        title: 'Comisiones',
        acept: function (event, ui) {
            $('#myDialogId').css('overflow', 'hidden');
        },
    });
});

function addEventToEnvio() {
    $("#envioTable [class='check-box']").change(function () {
        addEnvio();
    });

    disableSpinner();
}

function addEventToEnProgreso() {
    $(".pagar").click(function () {
        pagar();
    });

    disableSpinner();
}
function ConfigurarPago(ID, numero, Contacto, Servicio, Accion, FechaAlta, FechaEnvio, FechaEntrega, FechaPago, Costo, IdResponsable, IdCuentaCorriente, numeroPagina)
{
    enableSpinner();

    var datos = {
        "ID":ID,
        "numero": numero,
        "Contacto": Contacto,
        "Servicio": Servicio,
        "Accion": Accion,
        "FechaAlta": FechaAlta,
        "FechaEnvio":FechaEnvio,
        "FechaEntrega":FechaEntrega,
        "FechaPago":FechaPago,
        "Costo":Costo,
        "IdResponsable":IdResponsable,
        "IdCuentaCorriente": IdCuentaCorriente,
        "pageNumber": numeroPagina
    };

    $.ajax({
        type: "GET",
        url: '/Comision/ConfigurarPago',
        data: datos,
        datatype: "json",
        success: function (data) {
            debugger;
            var dialogDiv = $('#modal');
            dialogDiv.html(data)
            $.validator.unobtrusive.parse($("#modal"));
            dialogDiv.dialog('open');
        },
        error: function (error) {
            debugger;
            alert("Dynamic content load failed.");
        }
    });
}

function addEnvio() {
    //Tr perteneciente al pasajero a modificar
    var contexto = $(event.target).closest("tr");

    //Obtengo la informacion del pasajero que se esta actualizando
    var datos = {
        "idComision": $("[name*='item.ID']", contexto).val(),
        "Enviar": $("[name*='item.ParaEnviar']:checked", contexto).length,
    };

    $.ajax({
        async: false,
        url: '/Comision/setEnvio',
        type: 'POST',
        data: datos,
        dataType: "html",
        success: function (data) { disableSpinner(); },
        error: function (error) { addError(error, "EntregaRetiraComision", "Hubo un error al registrar el pago o la asistencia del pasajero."); ErrorClose = true; }
    });
    $(this).closest("tr")
}

function actualizarPago(accion, seccionActualizacion,IDFormaPago, MontoPago, ID, numero, Contacto, Servicio, Accion, FechaAlta, FechaEnvio, FechaEntrega, FechaPago, Costo, IdResponsable, IdCuentaCorriente, numeroPagina) {
    enableSpinner();
    var datos = {
        "ID": ID,
        "IdFormaPago": IDFormaPago,
        "MontoPago" : MontoPago,
        "numero": numero,
        "Contacto": Contacto,
        "Servicio": Servicio,
        "Accion": Accion,
        "FechaAlta": FechaAlta,
        "FechaEnvio": FechaEnvio,
        "FechaEntrega": FechaEntrega,
        "FechaPago": FechaPago,
        "Costo": Costo,
        "IdResponsable": IdResponsable,
        "IdCuentaCorriente": IdCuentaCorriente,
        "pageNumber": numeroPagina
    };
    
    $.ajax({
        async: true,
        url: '/Comision/pagar',
        type: 'POST',
        data: datos,
        dataType: "html",
        success: function (data) { $("#" + seccionActualizacion).html(data); disableSpinner(); $('#modal').dialog("close") },
        error: function (error) { disableSpinner(); $('#modal').dialog("close"); alert("Hubo un error al actualizar el estado de la comisión. Vuelva a intentar más tarde."); ErrorClose = true; }
    });
}

function actualizarEstadoComision(accion,seccionActualizacion,ID, numero, Contacto, Servicio, Accion, FechaAlta, FechaEnvio, FechaEntrega, FechaPago, Costo, IdResponsable, IdCuentaCorriente, numeroPagina)
{
    enableSpinner();
    debugger;
    var datos = {
        "ID":ID,
        "numero": numero,
        "Contacto": Contacto,
        "Servicio": Servicio,
        "Accion": Accion,
        "FechaAlta": FechaAlta,
        "FechaEnvio":FechaEnvio,
        "FechaEntrega":FechaEntrega,
        "FechaPago":FechaPago,
        "Costo":Costo,
        "IdResponsable":IdResponsable,
        "IdCuentaCorriente": IdCuentaCorriente,
        "pageNumber": numeroPagina
    };

    $.ajax({
        async: true,
        url: '/Comision/' + accion,
        type: 'POST',
        data: datos,
        dataType: "html",
        success: function (data) { $("#" + seccionActualizacion).html(data); disableSpinner(); },
        error: function (error) { debugger; disableSpinner(); alert("Hubo un error al actualizar el estado de la comisión. Vuelva a intentar más tarde."); ErrorClose = true; }
    });
}

function refrescarTab(accion, seccionActualizacion)
{
    enableSpinner();

    var datos = {
        "numero": $("#ID").val(),
        "Contacto": $("#Contacto").val(),
        "Servicio": $("#Servicio").val(),
        "Accion": $("#Accion").val(),
        "FechaAlta": $("#fechaAlta").val(),
        "FechaEnvio": $("#fechaEnvio").val(),
        "FechaEntrega": $("#fechaEntrega").val(),
        "FechaPago": $("#fechaPago").val(),
        "Costo": $("#Costo").val(),
        "IdResponsable": $("#SelectResponsable").val(),
        "IdCuentaCorriente": $("#SelectCuentaCorriente").val(),
        "pageNumber": 1
    };

    $.ajax({
        async: true,
        url: '/Comision/' + accion,
        type: 'POST',
        data: datos,
        dataType: "html",
        success: function (data) {
            $("#" + seccionActualizacion).html(data);
            disableSpinner();
        },
        error: function (error) { disableSpinner(); alert("Hubo un error al obtener los datos. Vuelva a intentar más tarde."); ErrorClose = true; }
    });
}

function Actualizar() {
    setTimeout(function () {
        window.location.reload(1);
    }, 5000);
}