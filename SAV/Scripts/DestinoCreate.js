$(document).ready(function () {
    $("#SelectProvincia").change(function () {
        $.ajax({
            url: '/Destinos/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvincia > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addItemsToCombo(data, $("#SelectLocalidad"), "SelectProvincia"); disableSpinner(); },
            error: function (error) { addLocalidadesError(error, "SelectProvincia"); disableSpinner();}
        });
    });

    $("#SelectLocalidad").change(function () {
        $.ajax({
            url: '/Destinos/GetParadas',
            type: 'POST',
            data: { "IdLocalidad": $("#SelectLocalidad > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addItemsToCombo(data, $("#SelectParada"), "SelectLocalidad"); disableSpinner(); },
            error: function (error) { addLocalidadesError(error, "SelectLocalidad"); disableSpinner(); }
        });
    });

    $("#SelectParada").change(function () {
        $("#ErrorSelectParada").remove();
        $("#NuevaParada").val($("#SelectParada > option:selected").text());
    })

    $("#AgregarParada").click(function () {
        var valid = ValidateForm();
        valid = ValidateFormNuevaParada() && valid;
        if (!valid)
            return false;
        AgregarParada();
    });

    $("#ModificarParada").click(function () {
        var valid = ValidateForm();
        valid = ValidateFormModificarEliminarParada() && valid;
        if (!valid)
            return false;
        ModificarParada();
    });

    $("#EliminarParada").click(function () {
        var valid = ValidateForm();
        valid = ValidateFormModificarEliminarParada() && valid;
        if (!valid)
            return false;
        if (confirm('Esta seguro que desea eliminar la parada?\nEn caso de estar siendo utilizada se aliminaran los viajes que hagan uso de esta.'))
            EliminarParada();
        else
            disableSpinner();
    });
});


function addLocalidadesError(error, displayError) {
    if ($("#Error" + displayError).length == 0)
        $("#Error").append("<span id='Error" + displayError + "' class='field-validation-error'><p>Hubo un error al recuperar las informacion</p></span>");
}

function addItemsToCombo(data, control, displayError) {
    if ($("#Error" + displayError).length)
        $("#Error" + displayError).remove();

    var item = "";
    $.each(data, function (i, dataItem) {
        item += "<option value='" + dataItem.Value + "'>" + dataItem.Text + "</option>";
    });
    $(control).html(item);
}

function ValidateForm()
{
    var valid = true;
    var valueProvincia = $("#SelectProvincia > option:selected").attr("value")
    var valueLocalidad = $("#SelectLocalidad > option:selected").attr("value")

    if (valueProvincia == undefined) {
        if ($("#ErrorSelectProvincia").length == 0)
            $("#Error").append("<span id='ErrorSelectProvincia' class='field-validation-error'><p>Debe seleccionar una Provincia</p></span>");
        valid = false;
    }
    else
        $("#ErrorSelectProvincia").remove();

    if (valueLocalidad == undefined) {
        if($("#ErrorSelectLocalidad").length == 0)
            $("#Error").append("<span id='ErrorSelectLocalidad' class='field-validation-error'><p>Debe seleccionar una Localidad</p></span>");
        valid = false;
    }
    else
        $("#ErrorSelectLocalidad").remove();

    if (!valid)
        disableSpinner();

    return valid;
}

function ValidateFormNuevaParada() {
    var valid = true;

    $("#ErrorSelectParada").remove();

    if ($("#NuevaParada").val() == "") {
        if ($("#ErrorNuevaParada").length == 0)
            $("#Error").append("<span id='ErrorNuevaParada' class='field-validation-error'><p>Debe ingresar una nueva parada</p></span>");
        valid = false;
    }
    else
        $("#ErrorNuevaParada").remove();

    if (!valid)
        disableSpinner();

    return valid;
}


function ValidateFormModificarEliminarParada() {
    var valid = true;
    var valueParada = $("#SelectParada > option:selected").attr("value")

    $("#ErrorNuevaParada").remove();

    if (valueParada == undefined) {
        if($("#ErrorSelectParada").length == 0)
            $("#Error").append("<span id='ErrorSelectParada' class='field-validation-error'><p>Debe seleccionar una Parada</p></span>");
        valid = false;
    }
    else
        $("#ErrorSelectParada").remove();

    if (!valid)
        disableSpinner();

    return valid;
}

function AgregarParada()
{
    $.ajax({
        url: '/Destinos/AddDestino',
        type: 'POST',
        data: { "IdLocalidad": $("#SelectLocalidad > option:selected").attr("value"), "NombreParada": $("#NuevaParada").val() },
        dataType: "json",
        success: function (data) { addItemsToCombo(data, $("#SelectParada")); disableSpinner(); },
        error: function (error) { addLocalidadesError(error); disableSpinner(); }
    });

    $("#NuevaParada").val("");
}

function ModificarParada() {
    $.ajax({
        url: '/Destinos/ModifyDestino',
        type: 'POST',
        data: { "IdLocalidad": $("#SelectLocalidad > option:selected").attr("value"), "IdParada": $("#SelectParada > option:selected").attr("value"), "NombreParada": $("#NuevaParada").val() },
        dataType: "json",
        success: function (data) { addItemsToCombo(data, $("#SelectParada"), "SelectParada"); disableSpinner(); },
        error: function (error) { addLocalidadesError(error, "SelectParada"); disableSpinner(); }
    });

    $("#NuevaParada").val("");
}

function EliminarParada() {
    $.ajax({
        url: '/Destinos/RemoveDestino',
        type: 'POST',
        data: { "IdLocalidad": $("#SelectLocalidad > option:selected").attr("value"), "IdParada": $("#SelectParada > option:selected").attr("value") },
        dataType: "json",
        success: function (data) { addItemsToCombo(data, $("#SelectParada"), "SelectParada"); disableSpinner(); },
        error: function (error) { addLocalidadesError(error, "SelectParada"); disableSpinner();}
    });

    $("#NuevaParada").val("");
}