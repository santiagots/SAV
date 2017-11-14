var click = 0;
var epaValues = [];
var individuoValues = [];
var porcValues = []
var cont=0;
var contI=0;
var apps = ['Session','Sell Station','FIPI','Outlook ','Banca Comercial'];
$(document).ready(function() {	
	jQuery.extend(jQuery.validator.messages, {
	  required: "Este campo es obligatorio.",
	  remote: "Por favor, rellena este campo.",
	  email: "Por favor, escribe una direcci�n de correo v�lida",
	  url: "Por favor, escribe una URL v�lida.",
	  date: "Por favor, escribe una fecha v�lida.",
	  dateISO: "Por favor, escribe una fecha (ISO) v�lida.",
	  number: "Por favor, escribe un n�mero entero v�lido.",
	  digits: "Por favor, escribe s�lo d�gitos.",
	  creditcard: "Por favor, escribe un n�mero de tarjeta v�lido.",
	  equalTo: "Por favor, escribe el mismo valor de nuevo.",
	  accept: "Por favor, escribe un valor con una extensi�n aceptada.",
	  maxlength: jQuery.validator.format("Por favor, no escribas m�s de {0} caracteres."),
	  minlength: jQuery.validator.format("Por favor, no escribas menos de {0} caracteres."),
	  rangelength: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1} caracteres."),
	  range: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1}."),
	  max: jQuery.validator.format("Por favor, escribe un valor menor o igual a {0}."),
	  min: jQuery.validator.format("Por favor, escribe un valor mayor o igual a {0}.")
	});
	jQuery.validator.setDefaults({
		errorPlacement: function(error, element) {	
			$('#errorPlace').append('<p class=\'error\'><span>&nbsp;</span>'+error[0].innerHTML+'</p>');			
		},
		onfocusout: false,
		onkeyup: false,
	});  
	$('.headAlertTables').click(function (e) {
		e.preventDefault();
		if ($(this).find('a').hasClass('desplegado')){
			$(this).find('a').removeClass('desplegado');
			$(this).find('a').addClass('contraido');
			$(this).removeClass('selected');
			$(this).parent().parent().find('#contraido'+$(this).find('a').attr('id')).slideDown('slow');
			$(this).parent().find('#tabla'+$(this).attr('id')).slideDown('slow');
			$(this).parent().parent().find('#contraido'+$(this).find('a').attr('id')).removeClass('hidden').delay(1000);
			$(this).parent().parent().find('#graph'+$(this).find('a').attr('id')).slideUp('slow');						
			
		}else if ($(this).find('a').hasClass('contraido')){
			$(this).find('a').addClass('desplegado');
			$(this).find('a').removeClass('contraido');
			$(this).addClass('selected');
			$(this).parent().parent().find('#contraido'+$(this).find('a').attr('id')).toggle("slide");
			$(this).parent().parent().find('#graph'+$(this).find('a').attr('id')).removeClass('hidden');
			$(this).parent().parent().find('#graph'+$(this).find('a').attr('id')).slideDown('slow');	
		}
		$(this).parent().find('#tabla'+$(this).find('a').attr('id')).toggle("slide");
		
		reduceRowText();
	});	
	
	$('.headAlertTablesExtended').click(function (e) {
		e.preventDefault();
		if ($(this).find("a").hasClass('desplegado')){
			$(this).find("a").removeClass('desplegado');
			$(this).find("a").addClass('contraido');
			$(this).removeClass('selected');
			//$(this).parent().parent().find('#contraido'+$(this).find("a").attr('id')).slideDown('slow');
			$(this).parent().find('#tabla'+$(this).find("a").attr('id')).slideDown('slow');
			
			$(this).parent().parent().find('#cantidad'+$(this).find("a").attr('name')).addClass('hidden');
		}else if ($(this).find("a").hasClass('contraido')){
			$(this).find("a").addClass('desplegado');
			$(this).find("a").removeClass('contraido');
			$(this).addClass('selected');
			$(this).parent().parent().find('#contraido'+$(this).find("a").attr('id')).slideUp('fast');
			$(this).parent().parent().find('#cantidad'+$(this).find("a").attr('name')).removeClass('hidden');
		}
		$(this).parent().find('#tabla'+$(this).find("a").attr('id')).slideToggle('slow');
		if($(this).parent().parent().parent().find('.headAlertTablesExtended.selected').length > 0){
			$(this).parent().parent().parent().find('.descripcionTabla').removeClass('hidden');
		}
		else{
			$(this).parent().parent().parent().find('.descripcionTabla').addClass('hidden');
		}
	});
	
	
	
	$('.botonVerMasTabla1').click(function(e){		
		if(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text()) < parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadTotal').text())){
			var cont = 0;
			var trs = $(this).parent().find("tbody tr.clonable");
			for(var a=0; trs.eq(a).length > 0 && cont < 4; a++){
				if(trs.eq(a).hasClass("clonable")){
					trs.eq(a).removeClass("hidden");
					trs.eq(a).removeClass("clonable");
					cont++;
				}			
			}
			$(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text())+cont);
			$(this).parent().find('table').trigger("applyWidgets");
			
		}
		if(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text()) == parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadTotal').text())){
				$(this).addClass('hidden');
		}
	});
	
	$('.botonVerMas').click(function(e){	
		var id = $(this).attr('id');

		var tabla = $("#"+id).parent().children('table').attr('id');
		$("#"+tabla).tablesorterPager({container: $("#pager"+id), size: parseInt($("#pager"+id+" .pagesize").html())+4});
			
		if(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text()) < parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadTotal').text())){
			var cont = 0;
			var trs = $(this).parent().find("tbody tr");
			for(var a=parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text()); trs.eq(a).length > 0 && cont < 4; a++){
				cont++;
			}
			$("#pager"+id).children('.pagesize').html(parseInt($("#pager"+id).children('.pagesize').html())+cont);				
			$("#"+tabla).tablesorterPager({container: $("#pager"+id), size: $("#pager"+id+" .pagesize").html()});	
			$(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text())+cont);
			$(this).parent().find('table').trigger("applyWidgets");
			
			if(parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadActual').text()) == parseInt($(this).parent().parent().find('#cantidad'+$(this).attr('id')+' .cantidadTotal').text())){
				$(this).addClass('hidden');
			}
		}	
	});
	
	$('.alertTablesExtended').each(function(e){
		if(parseInt($(this).find('.cantidad label.cantidadActual').text()) == parseInt($(this).find('.cantidad label.cantidadTotal').text())){
				$(this).find('.botonVerMasTabla1').addClass('hidden');
				$(this).find('.botonVerMas').addClass('hidden');
			}
	});

	/*if ($('#apps')){
		$('#apps a').each(function(i){
			    $(this).simpletip({content: $(this).attr('name')});
		});
	}*/
	if ($('.divGraph')){
		$('.divGraph div').each(function(i){
			$(this).simpletip({content: "<div class=\"tooltip-skin tooltip-izq\"></div><div class=\"tooltip-skin tooltip-skin-x tooltip-x\">"
			+$(this).attr('name') 
			+ "</div><div class=\"tooltip-skin tooltip-der\"></div><div class=\"tooltip-skin tooltip-skin-cen tooltip-cen\"></div>", 
			fixed: true, 
			position:'bottom', 
			offset:[0,-45]});	
		});
	}
	/*$('#individuos)*/
	if ($('#epa')){
		$('#epa a').each(function(i){
			simpleToolTip($(this).attr('name'), $(this));
		});
	}
	if ($('#individuos')){
		$('#individuos a').each(function(i){
			simpleToolTip($(this).attr('name'), $(this));
		});
	}

	$('#content').click(function(){
		if (click > 0) {
			hideListing();
		}
		click = 0;		
	});
	if ($('#helpClick')){
		$('#helpClick').click(function(e){
			hideListing();	
			e.preventDefault();
			e.stopPropagation();		
			$('#helpBox').removeClass('hidden');
			click = 1;
		});
	}
	if ($('#calendarClick')){
		$('#calendarClick').click(function(e){
			e.preventDefault();
			e.stopPropagation();
			hideListing();
			$('#calendarBox').removeClass('hidden');
			click = 1;
		});
	}	
	$('#footer').css('top',function(){		
		return $(window).height() - 30;		
	});

	/*$('.tooltip').css('top',function(){		
		return $(window).height() - 40;		
	});*/
    if ($('#interaccion')){
		$('#interaccion').click(function(e){	
            if($('#subInteraccion').hasClass('hidden'))
			    $('#subInteraccion').removeClass('hidden');
            else 
                $('#subInteraccion').addClass('hidden');
		});
	}
});
function hideListing(){
	$('#helpBox').addClass('hidden');
	$('#calendarBox').addClass('hidden');
}

function simpleToolTip(data, elem) {
    var self = this;
    elem = jQuery(elem);

    var tooltip = jQuery(document.createElement('div'))
                     .addClass('tooltip')
                     .html(data)
                     .appendTo(elem);
	//TODO en produccion data debe ser data.content
    tooltip.hide();

    var elemPos = elem.offset();

    posX = elemPos.left - jQuery("#footer").offset().left;
    posY = -28;	

    tooltip.css({ left: posX - 25, top: posY });
 }

function showTooltip(id) {
    $("#" + id).children().css("display", "block");
    $("#" + id).children().fadeIn(350);
}

function hideTooltip(id) {
    $("#" + id).children().css("display", "none");
    $("#" + id).children().fadeOut(350);
}