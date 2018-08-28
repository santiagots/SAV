$('#modal').dialog({
    autoOpen: false,
    modal: true,
    resizable: false,
    height: 310,
    width: 'auto',
    title: 'Nuevo Gasto',
    acept: function (event, ui) {
        $('#myDialogId').css('overflow', 'hidden');
    }
});

$("#alta").click(function () {
    debugger;
    $.ajax({
        type: "GET",
        url: '/Gasto/Create',
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

function CreateGastoSuccess() {
    debugger;
    $.ajax({
        type: "GET",
        url: '/Gasto/GastosPaging',
        data: { "Comentario": $("#Comentario").val(), "fechaAlta": $("#fecha").val(), "monto": $("#Monto").val(), "tipoGasto": $("#SelectTipoGasto").val(), "usuarioAlta": $("#SelectUsuarioAlta").val(), "pageNumber": "1" },
        datatype: "json",
        success: function (data) {
            $('#gastosTable').html(data)
            $('#modal').dialog("close")
        },
        error: function () {
            alert("Error al cargar la grilla de gastos");
            $('#modal').dialog("close")
        }
    });
}