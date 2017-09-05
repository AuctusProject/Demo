// Write your Javascript code.
$(document).ready(function () {
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });

    hljs.configure({ useBR: true });

    var deploy = function (msg) {
        $('.deploy-message').text(msg);
    };

    hub.on('deploy', deploy);

    hub.on('deployCompleted', Wizard.Operations.OnDeployCompleted);
    hub.on('deployUncompleted', Wizard.Operations.OnDeployUncompleted);
    hub.on('deployError', Wizard.Operations.OnDeployError);

    $.connection.hub.disconnected(function () {
        if ($.connection.hub.lastError)
        { alert("Disconnected. Reason: " + $.connection.hub.lastError.message); }
    });

    connection.start();
});

var connection = $.hubConnection();
var hub = connection.createHubProxy("AuctusDemo");

