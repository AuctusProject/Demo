$(document).ready(function () {

    $('[data-toggle="popover"]').popover();

    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest('.bmd-form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.bmd-form-group').removeClass('has-error');
        },
        errorElement: 'div',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            $(element).closest('.bmd-form-group').children('.help-block').remove();
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });

    $('form').validate();

    hljs.configure({ useBR: true });

    $.connection.hub.disconnected(function () {
        if ($.connection.hub.lastError)
        { alert("Disconnected. Reason: " + $.connection.hub.lastError.message); }
    });

    connection.start().done(function ()
    {
        if (signalrDone) {
            signalrDone();
        } else {
            signalrDone = function () { };
        }
    });
});

jQuery.fn.outerHTML = function () {
    return jQuery('<div />').append(this.eq(0).clone()).html();
};

var connection = $.hubConnection();
var hub = connection.createHubProxy("AuctusDemo");
var signalrDone = null;

$('.form-control').focus(function () { $(this).parent().addClass('is-focused'); });
$('.form-control').blur(function () { $(this).parent().removeClass('is-focused'); });


var Parameter = {
    BlockExplorerUrl: "https://rinkeby.etherscan.io"
};