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

$("#SelectConcepto").change(function () {
    debugger;
    if ($(this).val() == "") {
        $("#SelectTipoGasto").html("<option value=''>Elija una opción</option>");
        return;
    }
    $.ajax({
        url: '/Gasto/GetGastosPorConcepto',
        type: 'POST',
        data: { "Concepto": $("#SelectConcepto > option:selected").attr("value") },
        dataType: "json",
        success: function (data) { addOptions(data, $("#SelectTipoGasto")); },
    });
});

$("#SelectConceptoCreate").change(function () {
    debugger;
    if ($(this).val() == "") {
        $("#SelectTipoGastoCreate").html("<option value=''>Elija una opción</option>");
        return;
    }
    $.ajax({
        url: '/Gasto/GetGastosPorConcepto',
        type: 'POST',
        data: { "Concepto": $("#SelectConceptoCreate > option:selected").attr("value") },
        dataType: "json",
        success: function (data) { addOptions(data, $("#SelectTipoGastoCreate")); },
    });
});


$("#alta").click(function () {
    $.ajax({
        type: "GET",
        url: '/Gasto/Create',
        datatype: "json",
        success: function (data) {
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

function addOptions(options, control) {
    var item = "<option value=''>Elija una opción</option>";
    $.each(options, function (i, option) {
        item += "<option value='" + option.Value + "'>" + option.Text + "</option>";
    });
    $(control).html(item);
}