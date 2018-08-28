$(document).ready(function () {
    debugger;
    $(document).on('change', '#SelectProvincia', function () {
        debugger;
        if ($(this).val() == "") {
            $("#SelectLocalidad").html("<option value=''>Elija una opción</option>");
            return;
        }
        $.ajax({
            url: '/Domicilio/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvincia > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidad")); },
            error: function (error) { addLocalidadesError(error);}
        });
    });
});

function validateForm() {
    return $('#form0').validate().form();
}

function addLocalidades(localidades, control) {
    if ($("#ErrorLocalidades").length)
        $("#ErrorLocalidades").remove();

    var item = "<option value=''>Elija una opción</option>";
    $.each(localidades, function (i, provincia) {
        item += "<option value='" + provincia.Value + "'>" + provincia.Text + "</option>";
    });
    $(control).html(item);
}


function addLocalidadesError(error) {
    if ($("#ErrorLocalidades").length == 0)
        $("#Error").append("<span id='ErrorLocalidades' class='field-validation-error'><p>Hubo un error al recuperar las localidades</p></span>");
}