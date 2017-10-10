
$(document).ready(function () {
    $('#registerEmployee').validate();
    registerEmployeeForm.configInputs();
});

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
        if ($('#employeeNameInput').val() != "" && $('#salaryInput').val() != "" && $('#contributionInput').val() != "" && $('#registerEmployee').valid())
	        $('.next-button').removeAttr('disabled');
	    else
	        $('.next-button').attr('disabled', 'disabled');
	}  
};