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
    
    //startSignalRConnection();

    //connection.disconnected(function () {
    //    if (signalrDisconnected) {
    //        signalrDisconnected();
    //    }
    //    setTimeout(function () {
    //        startSignalRConnection();
    //    }, 5000);
    //});

    $('.onlyInteger').keypress(isNumber);
});

//function startSignalRConnection() {
//    connection.start()
//        .done(function () {
//            if (signalrDone) {
//                signalrDone();
//            } else {
//                signalrDone = function () { };
//            }
//        })
//        .fail(function () { alert('SignalR could not be connected.'); });
//}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

jQuery.fn.outerHTML = function () {
    return jQuery('<div />').append(this.eq(0).clone()).html();
};

//var connection = $.hubConnection();
//var hub = connection.createHubProxy("AuctusDemo");
//var signalrDone = null;
//var signalrDisconnected = null;

$('.form-control').focus(function () { $(this).parent().addClass('is-focused'); });
$('.form-control').blur(function () { $(this).parent().removeClass('is-focused'); });


var Parameter = {
    BlockExplorerUrl: "https://rinkeby.etherscan.io"
};