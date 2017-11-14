var spinner;
$(document).ready(function () {

    $(".waitingMenuBar").click(function () {
        enableSpinner();
    });

    $(".waiting").click(function () {
        //if ($("form").length == 0 || $("form").valid()) {
        //enableSpinner();

        var error = false
        $("form").each(function (i) {
            if (!$(this).valid())
                error = true
        });
        if (!error)
            enableSpinner();
    });
});

function enableSpinner() {
    $("#loading").fadeIn();
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
    var target = document.getElementById('loading');
    spinner = new Spinner(opts).spin(target);
}

function disableSpinner() {
    $("#loading").fadeOut();
    if (spinner != undefined)
        spinner.stop();
    $(".waiting").each(function () {
        if (!hasEvent(this, "click"))
            $(this).click(function () { enableSpinner(); });
    });
}

function confirmSpinner(message) {
    if (confirm(message)) {
        enableSpinner();
        return true;
    }
    else {
        disableSpinner();
        return false;
    }
}


var hasEvent = function (el, eventName) {
    if (!$(el).length || !$(el).data('events')) {
        return false;
    }
    return !!$(el).data('events')[eventName];
};
