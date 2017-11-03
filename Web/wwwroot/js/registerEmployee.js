
$(document).ready(function () {
    $('#registerEmployee').validate();
    registerEmployeeForm.configInputs();
    $('.asset-input-group input').blur(updateTotalAssets);
});

function updateTotalAssets() {
    var total = 0;
    $('.asset-input-group input').each(function (i, element) {
        if (element.value < 0)
            element.value = 0;
        if (!isNaN(parseInt(element.value))) {
            total += parseInt(element.value);
        }

    });
    if (total != 100) {
        $('.total-assets').addClass('has-error');
        $('.total-assets').removeClass('has-success');
    }
    else {
        $('.total-assets').removeClass('has-error');
        $('.total-assets').addClass('has-success');
    }

    $('.total-assets-value').html(total);
    registerEmployeeForm.enableNextButtonIfApplicable();
}

var registerEmployeeForm = {
    init: function(){
        this.enableNextButtonIfApplicable();
        $('#employeeNameInput').focus();
    },
    configInputs: function(){
        $('#employeeNameInput').blur(registerEmployeeForm.enableNextButtonIfApplicable);
        $('#salaryInput').blur(registerEmployeeForm.enableNextButtonIfApplicable);
        $('#contributionInput').blur(registerEmployeeForm.enableNextButtonIfApplicable);
	},
    enableNextButtonIfApplicable: function() {
        if ($('.total-assets').hasClass('has-success') && $('#employeeNameInput').val() != "" && $('#salaryInput').val() != "" && $('#contributionInput').val() != "" && $('#registerEmployee').valid())
	        $('.next-button').removeAttr('disabled');
	    else
	        $('.next-button').attr('disabled', 'disabled');
	}  
};