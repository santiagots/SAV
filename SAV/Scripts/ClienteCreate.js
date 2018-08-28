$(document).ready(function () {

    $("#ViajesEnElMismoDia").dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        height: 'auto',
        width: '1000px',
        title: "Viajes Cliente"
    }).dialog('open');

    //$("#Guardar").click(function () {
    //    validarViajesDia()
    //    if ($("#Pago").attr('checked') && $("form:first").valid())
    //        imprimirBoleto();
    //});

    $("#Pago").click(function () {
        if (this.checked)
            $("#SelectFormaPago").prop("disabled", false);
        else
            $("#SelectFormaPago").prop("disabled", true);
    });

    $("#SelectParadaAscenso").change(HabilitarDeshabilitarDomicilioAscenso);
    HabilitarDeshabilitarDomicilioAscenso();
    $("#SelectParadaDescenso").change(HabilitarDeshabilitarDomicilioDescenso);
    HabilitarDeshabilitarDomicilioDescenso();
    $("#SelectDomicilioAscenso").change(HabilitarDeshabilitarParadaAscenso);
    HabilitarDeshabilitarParadaAscenso();
    $("#SelectDomicilioDescenso").change(HabilitarDeshabilitarParadaDescenso);
    HabilitarDeshabilitarParadaDescenso();

    $('#modal').dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        height: 310,
        width: 'auto',
        title: 'Domicilio',
        acept: function (event, ui) {
            $('#myDialogId').css('overflow', 'hidden');
        }
    });

    $("#AgregarDireccion").click(function () {
        debugger;
        $.ajax({
            type: "GET",
            url: '/Domicilio/Create',
            data: { "ClienteId": $("#Id").val() },
            datatype: "json",
            success: function (data) {
                debugger;
                var dialogDiv = $('#modal');
                dialogDiv.html(data)
                $.validator.unobtrusive.parse($("#modal"));
                dialogDiv.dialog('open');
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });
});

function HabilitarDeshabilitarDomicilioAscenso() {
    debugger;
    var select = $("#SelectParadaAscenso")
    if (select.length == 0)
        return;

    if (select[0].selectedIndex == 0) {
        $("#SelectDomicilioAscenso").prop("disabled", false)
    }
    else {
        $("#SelectDomicilioAscenso").prop("disabled", true)
    }
}

function HabilitarDeshabilitarDomicilioDescenso()
{
    var select = $("#SelectParadaDescenso")
    if (select.length == 0)
        return;

    if ( select[0].selectedIndex == 0) {
        $("#SelectDomicilioDescenso").prop("disabled", false)
    }
    else {
        $("#SelectDomicilioDescenso").prop("disabled", true)
    }
}

function HabilitarDeshabilitarParadaAscenso() {
    debugger;
    var select = $("#SelectDomicilioAscenso")
    if (select.length == 0)
        return;

    if (select[0].selectedIndex == 0) {
        $("#SelectParadaAscenso").prop("disabled", false)
    }
    else {
        $("#SelectParadaAscenso").prop("disabled", true)
    }
}

function HabilitarDeshabilitarParadaDescenso() {
    var select = $("#SelectDomicilioDescenso")
    if (select.length == 0)
        return;

    if (select[0].selectedIndex == 0) {
        $("#SelectParadaDescenso").prop("disabled", false)
    }
    else {
        $("#SelectParadaDescenso").prop("disabled", true)
    }
}

function CreateDomicilioSuccess() {
    $.ajax({
        type: "GET",
        url: '/Cliente/PagingDomicilios',
        data: { "id": $("#Id").val(), "pageNumber": "1" },
        datatype: "json",
        success: function (data) {
            $('#partialViewDomicilios').html(data)
            $('#modal').dialog("close")
            ActualizarComboDomicilios($("#Id").val())
        },
        error: function () {
            alert("Error al cargar la grilla de domicilios");
            $('#modal').dialog("close")
        }
    });
}

function DeleteDomicilio(id, clienteId) {
    if (confirmSpinner('Esta seguro que desea borrar el domicilio?'))
    {
        $.ajax({
            type: "GET",
            url: '/Cliente/DeleteDomicilio',
            data: { "id": id, "ClienteId": clienteId },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#partialViewDomicilios').html(data)
                ActualizarComboDomicilios(clienteId)
                disableSpinner();
            },
            error: function () {
                alert("Error al cargar la grilla de domicilios");
            }
        });
    }
}

function UpdateDomicilio(id, clienteId) {
    $.ajax({
        type: "GET",
        url: '/Domicilio/Update',
        data: { "id": id, "ClienteId": clienteId },
        datatype: "json",
        success: function (data) {
            var dialogDiv = $('#modal');
            dialogDiv.html(data)
            $.validator.unobtrusive.parse($("#modal"));
            dialogDiv.dialog('open');
        },
        error: function () {
            alert("Error al cargar la grilla de domicilios");
        }
    });
}

function ActualizarComboDomicilios(clienteId) {
    $.ajax({
        type: "GET",
        url: '/Domicilio/GetDomicilios',
        data: { "ClienteId": clienteId },
        datatype: "json",
        success: function (json) {
            $('#SelectDomicilioAscenso').find('option').not(':first').remove().end();
            $('#SelectDomicilioAscensoViajeCerrado').find('option').not(':first').not(':nth-child(2)').remove().end();
            $('#SelectDomicilioDescenso').find('option').not(':first').remove().end();

            $.each(json, function (i, obj) {
                $('#SelectDomicilioAscenso').append($('<option>').text(obj.Value).attr('value', obj.Key));
            });
            $.each(json, function (i, obj) {
                $('#SelectDomicilioAscensoViajeCerrado').append($('<option>').text(obj.Value).attr('value', obj.Key));
            });
            $.each(json, function (i, obj) {
                $('#SelectDomicilioDescenso').append($('<option>').text(obj.Value).attr('value', obj.Key));
            });
        },
        error: function (json) {
            alert("Error al actualizar los domicilios");
        }
    });
}