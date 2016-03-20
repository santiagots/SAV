var url = "";

$(document).ready(function () {

    $("#SelectProvinciaRetirar").change(function () {
        $.ajax({
            url: '/Comision/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvinciaRetirar > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidadRetirar")); },
            error: function (error) { addLocalidadesError(error); }
        });
    });

    $("#SelectProvinciaEntregar").change(function () {
        $.ajax({
            url: '/Comision/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvinciaEntregar > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidadEntregar")); },
            error: function (error) { addLocalidadesError(error); }
        });
    });

    ShowServicioData();

    $("#Servicio, #Accion").change(function () {
        $("#Create").resetValidation();
        ShowServicioData();
    });

    $("#AgregarResponsable").live("click", function (e) {
        $("#dialog").dialog('open');
        SpinnerPopUp();
        return false;
    });

        $("#dialog").dialog({
            title: 'Agregar Responsable',
            autoOpen: false,
            resizable: false,
            width: 1000,
            position: ['center',20] ,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                $(".ui-dialog-titlebar-close").hide();
                $(this).load("/Comision/ResponsableAdd?time=" + event.timeStamp);
            }
        });
});

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

function ShowServicioData()
{
    if ($("#Servicio > option:selected").attr("value") == "Directo") {

        $("#servicioDirecto").slideDown("slow");
        $("#servicioPuertaEntrega").slideUp("slow");
        $("#servicioPuertaRetirar").slideUp("slow");

        if ($("#Accion > option:selected").attr("value") == "Entregar") {
            $("#servicioDirectoRetirar").slideUp("slow");
            $("#servicioDirectoEntregar").slideDown("slow")
        }
        if ($("#Accion > option:selected").attr("value") == "Retirar") {
            $("#servicioDirectoRetirar").slideDown("slow");
            $("#servicioDirectoEntregar").slideUp("slow")
        }
        if ($("#Accion > option:selected").attr("value") == "EntregarRetirar") {
            $("#servicioDirectoRetirar").slideDown("slow");
            $("#servicioDirectoEntregar").slideDown("slow")
        }
    }
    else {
        $("#servicioDirecto").slideUp("slow");

        if ($("#Accion > option:selected").attr("value") == "Entregar") {
            $("#servicioPuertaRetirar").slideUp("slow");
            $("#servicioPuertaEntrega").slideDown("slow")
        }
        if ($("#Accion > option:selected").attr("value") == "Retirar") {
            $("#servicioPuertaRetirar").slideDown("slow");
            $("#servicioPuertaEntrega").slideUp("slow")
        }
        if ($("#Accion > option:selected").attr("value") == "EntregarRetirar") {
            $("#servicioPuertaRetirar").slideDown("slow");
            $("#servicioPuertaEntrega").slideDown("slow")
        }
    }
}

function resetValidation() {

        var $form = $('form');

        //reset jQuery Validate's internals
        $form.validate().resetForm();

        //reset unobtrusive validation summary, if it exists
        $form.find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        //reset unobtrusive field level, if it exists
        $form.find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        return $form;
}


function agregarResponsable() {
    var valid = true;

    if ($("#NuevoApellido").val() == "") {
        addError("NuevoApellido", "Hubo un error al guardar la información de Comisiones");
        valid = false;
    }
    if ($("#NuevoNombre").val() == "") {
        addError("NuevoApellido", "Hubo un error al guardar la información de Comisiones");
        valid = false;
    }

    if (!valid)
        return;

    $.ajax({
        async: true,
        url: '/Comision/ResponsableAdd',
        type: 'POST',
        data:
            {
                "apellido": $("#NuevoApellido").val(),
                "nombre": $("#NuevoNombre").val()
            },
        dataType: "html",
        success: function (data) {
            $('#dialog').html(data)
        },
        error: function (error) { alert(error); disableSpinner(); }
    });
}

function deleteResponsable(id)
{
    $.ajax({
        async: true,
        url: '/Comision/ResponsableDelete',
        type: 'POST',
        data:
            {
                "id": id,
            },
        dataType: "html",
        success: function (data) {
            EliminarComboResponsable(id);
            $('#dialog').html(data);
        },
        error: function (error) { alert(error); disableSpinner(); }
    });
}


function addError(displayError, mesage) {
    if ($("#ErrorResponsable" + displayError).length == 0)
        $("#ErrorResponsable").append("<span id='Error" + displayError + "' class='field-validation-error'><p>" + mesage + "</p></span>");
}

function CargarComboResponsable(id, apellido, nombre)
{
    $("#SelectResponsable").append('<option value=' + id + '>' + apellido + ', ' + nombre + '</option>');
}

function EliminarComboResponsable(id) {
    $("#SelectResponsable option[value='" + id + "']").remove();
}

function CerrarResponsable() {
    $('#SelectResponsable').find('option').remove();

    $("#SelectResponsable").append('<option value="">Elija una opción</option>');

    $(".Responsables").each(function () {
        var id = $(this).attr('data-responsable-id');
        var apellido = $(this).attr('data-responsable-apellido');
        var nombre = $(this).attr('data-responsable-nombre');

        $("#SelectResponsable").append('<option value=' + id + '>' + apellido + ', ' + nombre + '</option>');
    });

    $('#dialog').html("<div id='loadingPopUp'></div>");

    $('#dialog').dialog('close');
}

function SpinnerPopUp() {
    $("#loadingPopUp").fadeIn();
    var opts = {
        lines: 12, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        color: '#0387cd', // #rgb or #rrggbb
        speed: 1, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false // Whether to use hardware acceleration
    };
    var target = document.getElementById('loadingPopUp');
    spinner = new Spinner(opts).spin(target);
}