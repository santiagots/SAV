$(document).ready(function () {

    $("#ViajesEnElMismoDia").dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        height: 'auto',
        width: '1000px',
        title: "Viajes Cliente"
    }).dialog('open');

    $("#SelectProvinciaPrincipal").change(function () {
        if ($(this).val() == "") {
            $("#SelectLocalidadPrincipal").html("<option value=''>Elija una opción</option>");
            return;
        }
        $.ajax({
            url: '/Cliente/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvinciaPrincipal > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidadPrincipal")); },
            error: function (error) { addLocalidadesError(error);}
        });
    });

    $("#SelectProvinciaSecundaria").change(function () {
        if ($(this).val() == "") {
            $("#SelectLocalidadSecundaria").html("<option value=''>Elija una opción</option>");
            return;
        }
        $.ajax({
            url: '/Cliente/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvinciaSecundaria > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidadSecundaria")); },
            error: function (error) { addLocalidadesError(error); }
        });
    });

    $("#SelectProvinciaOtros").change(function () {
        if ($(this).val() == "") {
            $("#SelectLocalidadOtros").html("<option value=''>Elija una opción</option>");
            return;
        }
        $.ajax({
            url: '/Cliente/GetLocalidades',
            type: 'POST',
            data: { "IdProvincia": $("#SelectProvinciaOtros > option:selected").attr("value") },
            dataType: "json",
            success: function (data) { addLocalidades(data, $("#SelectLocalidadOtros")); },
            error: function (error) { addLocalidadesError(error); }
        });
    });

    $("#Guardar").click(function () {
        debugger;
        validarViajesDia()
        if ($("#Pago").attr('checked') && $("form:first").valid())
            imprimirBoleto();
    });
    
    $("#AscensoDomicilioPrincipal").click(function () {
        viajeDirectoAscensoDescensoDomicilio();
    });

    $("#DescensoDomicilioPrincipal").click(function () {
        viajeDirectoAscensoDescensoDomicilio();
    });

    $("#DescensoDomicilioOtros").click(function () {
        viajeDirectoAscensoDescensoDomicilio();
        showHideDescensoDomicilioOtros();
    });

    showHideDescensoDomicilioOtros();
    viajeDirectoAscensoDescensoDomicilio();
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

function imprimirBoleto()
{
        var apellido = $("#Apellido").val();
        var nombre = $("#Nombre").val();
        var dni = $("#DNI").val();

        var origen = $("#Origen").val();

        if ($("#SelectServicioPuertaAscenso").length > 0)
            if ($("#SelectServicioPuertaAscenso option:selected").val() == 1)
                origen += " - " + $("#CallePrincipal").val() + " " + $("#CalleNumeroPrincipal").val() + " " + $("#CallePisoPrincipal").val()
            else
                origen += " - " + $("#CalleSecundaria").val() + " " + $("#CalleNumeroSecundaria").val() + " " + $("#CallePisoSecundaria").val()

        if ($("#SelectServicioDirectoAscenso").length > 0)
            origen += " - " + $("#SelectServicioDirectoAscenso option:selected").text();

        var destino = $("#Destino").val();

        if ($("#SelectServicioPuertaDescenso").length > 0)
            if ($("#SelectServicioPuertaDescenso option:selected").val() == 1)
                destino += " - " + $("#CallePrincipal").val() + " " + $("#CalleNumeroPrincipal").val() + " " + $("#CallePisoPrincipal").val()
            else
                destino += " - " + $("#CalleSecundaria").val() + " " + $("#CalleNumeroSecundaria").val() + " " + $("#CallePisoSecundaria").val()

        if ($("#SelectServicioDirectoDescenso").length > 0)
            destino += " - " + $("#SelectServicioDirectoDescenso option:selected").text();

        var fecha = $("#FechaSalida").val();
        var idViaje = $("#IdViaje").val();
        var servicio = $("#Servicio").val();

        CenterWindow(screen.availWidth * 0.9, screen.availHeight * 0.6, 0, '/Cliente/ImprimirBoleto?Apellido=' + apellido + '&Nombre=' + nombre + '&DNI=' + dni + '&Origen=' + origen + '&Destino=' + destino + '&fecha=' + fecha + '&idViaje=' + idViaje + '&servicio=' + servicio, 'Boleto');
}

function CenterWindow(windowWidth, windowHeight, windowOuterHeight, url, wname, features) {
    var centerLeft = parseInt((window.screen.availWidth - windowWidth) / 2);
    var centerTop = parseInt(((window.screen.availHeight - windowHeight) / 2) - windowOuterHeight);

    var misc_features;
    if (features) {
        misc_features = ', ' + features;
    }
    else {
        misc_features = ', status=no, location=no, scrollbars=yes, resizable=yes';
    }
    //var windowFeatures = 'width=' + windowWidth + ',height=' + windowHeight + ',left=' + centerLeft + ',top=' + centerTop + misc_features;
    var windowFeatures = 'width=' + windowWidth + ',height=' + windowHeight + misc_features;
    var win = window.open(url, wname, windowFeatures);
    win.moveTo(centerLeft, centerTop);
    win.focus();
    return win;
}

function viajeDirectoAscensoDescensoDomicilio()
{
    
    //$("#DNI").attr("data-val", false);
    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");


    var ascensoDomicilioPrincipal = $("#AscensoDomicilioPrincipal");
    var descensoDomicilioPrincipal = $("#DescensoDomicilioPrincipal");
    var descensoDomicilioOtros = $("#DescensoDomicilioOtros");
    var servicioDirectoAscenso = $("#SelectServicioDirectoAscenso");
    var servicioDirectoDescenso = $("#SelectServicioDirectoDescenso");
    

    if ($(ascensoDomicilioPrincipal).is(':checked')) {
        $(servicioDirectoAscenso).prop('disabled', true);
        $(descensoDomicilioPrincipal).prop('disabled', true);
        $(descensoDomicilioOtros).prop('disabled', false);

        $("#SelectProvinciaPrincipal").attr("data-val-required", "Debe seleccionar una Provincia al Domicilio Principal");
        $("#SelectLocalidadPrincipal").attr("data-val-required", "Debe seleccionar una Localidad al Domicilio Principal");
        $("#DomicilioPrincipal #CallePrincipal").attr("data-val-required", "Debe ingresar una Calle al Domicilio Principal");
        $("#DomicilioPrincipal #CalleNumeroPrincipal").attr("data-val-required", "Debe ingresar un Numero al Domicilio Principal");
         
        $("#DomicilioPrincipal b").show();
    }
    else {
        $(servicioDirectoAscenso).prop('disabled', false);

        $("#DomicilioPrincipal :input").removeAttr("data-val-required");
        $("#DomicilioPrincipal b").hide();
    }
    if ($(descensoDomicilioPrincipal).is(':checked')){
        $(ascensoDomicilioPrincipal).prop('disabled', true);
        $(servicioDirectoDescenso).prop('disabled', true);
        $(descensoDomicilioOtros).prop('disabled', true);
    }
    else if (descensoDomicilioOtros.is(':checked')) {
        $(descensoDomicilioPrincipal).prop('disabled', true);
        $(servicioDirectoDescenso).prop('disabled', true);
    }
    else {
        $(servicioDirectoDescenso).prop('disabled', false);
    }
    if (!$(ascensoDomicilioPrincipal).is(':checked') && !$(descensoDomicilioPrincipal).is(':checked') && !descensoDomicilioOtros.is(':checked')) {
        $(ascensoDomicilioPrincipal).prop('disabled', false);
        $(descensoDomicilioPrincipal).prop('disabled', false);
        $(descensoDomicilioOtros).prop('disabled', false);
        $(servicioDirectoAscenso).prop('disabled', false);
        $(servicioDirectoDescenso).prop('disabled', false);
    }

    $("form").removeData("validator");
    $("form").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse("form");
    $('form').resetValidation();
}

function showHideDescensoDomicilioOtros()
{
    if($("#DescensoDomicilioOtros").is(':checked'))
        $("#domicilioOtros").slideDown("slow");
    else
        $("#domicilioOtros").slideUp("slow");
}

function validarViajesDia()
{

}