
$(document).ready(function () {
    $('#registerCompany').validate();
    registerCompanyForm.configVestingSlider();
    registerCompanyForm.configModal();
    registerCompanyForm.configInputs();
});

var registerCompanyForm = {
    init: function(){
        this.enableNextButtonIfApplicable();
        $('#companyNameInput').focus();
    },
    configInputs: function(){
	    $('#companyNameInput').blur(registerCompanyForm.enableNextButtonIfApplicable);
	    $('#employerMatchingInput').blur(registerCompanyForm.enableNextButtonIfApplicable);
	    $('#maxCounterpartInput').blur(registerCompanyForm.enableNextButtonIfApplicable);
	},
    enableNextButtonIfApplicable: function() {
        if ($('#companyNameInput').val() != "" && $('#employerMatchingInput').val() != "" && $('#maxCounterpartInput').val() != "" && $('#registerCompany').valid())
	        $('.next-button').removeAttr('disabled');
	    else
	        $('.next-button').attr('disabled', 'disabled');
	},
    configModal: function(){
	    $("#advancedOptions").on('shown.bs.modal', function(){
	        registerCompanyForm.vestingSlider.tmpValues = registerCompanyForm.vestingSlider.noUiSlider.get();
	    });
	},
    configVestingSlider: function(){
	    this.vestingSlider = document.getElementById('slider');

	    var startValues = [0, 20, 40, 60, 80, 100]
	    noUiSlider.create(this.vestingSlider, {
	        start: startValues,
	        step: 1,
	        tooltips: true,
	        connect: true,
	        range: {
	            'min': 0,
	            'max': 100
	        }
	    });
	    
	    $($('.noUi-handle')[0]).css('background-color', '#002d78');
	    $($('.noUi-connect')[0]).css('background-color', '#002d78');
	    $($('.noUi-handle')[1]).css('background-color', '#0243b7');
	    $($('.noUi-connect')[1]).css('background-color', '#0243b7');
	    $($('.noUi-handle')[2]).css('background-color', '#00bdff');
	    $($('.noUi-connect')[2]).css('background-color', '#00bdff');
	    $($('.noUi-handle')[3]).css('background-color', '#07dce0');
	    $($('.noUi-connect')[3]).css('background-color', '#07dce0');
	    $($('.noUi-handle')[4]).css('background-color', '#0be6d0');
	    $($('.noUi-connect')[4]).css('background-color', '#0be6d0');

	    $($('.noUi-origin')[0]).attr('disabled', 'disabled');
	    $($('.noUi-origin')[5]).attr('disabled', 'disabled');

	    $('.noUi-tooltip').addClass('unselected');
	    registerCompanyForm.setTooltipValues(startValues);

	    this.vestingSlider.noUiSlider.on('slide', function( values, handle ) {
	        $($('.noUi-tooltip')[handle]).removeClass('unselected');
	        registerCompanyForm.setTooltipValues(values);
	    });

	    this.vestingSlider.noUiSlider.on('start', function( values, handle ) {
	        $($('.noUi-tooltip')[handle]).removeClass('unselected');
	        registerCompanyForm.hideTooltipsExceptSelected(handle);
	        registerCompanyForm.setTooltipValues(values);
	    });

	    this.vestingSlider.noUiSlider.on('change', function( values, handle ) {
	        registerCompanyForm.setTooltipValues(values);
	    });

	    this.vestingSlider.noUiSlider.on('set', function( values, handle ) {
	        registerCompanyForm.setTooltipValues(values);
	    });

	    this.vestingSlider.noUiSlider.on('end', function( values, handle ) {
	        $($('.noUi-tooltip')[handle]).addClass('unselected');
	        registerCompanyForm.showAllTooltips();
	    });
	},
    showAllTooltips: function(){
	    for (var i = 0; i < 6; ++i){
	        $($('.noUi-tooltip')[i]).css('display', 'block');
	    }
	},
    hideTooltipsExceptSelected: function(indexSelected){
	    for (var i = 0; i < 6; ++i){
	        if (i != indexSelected){
	            $($('.noUi-tooltip')[i]).css('display', 'none');
	        }
	    }
	},
    setTooltipValues: function(values){
	    $($('.noUi-tooltip')[0]).html('<span class="year">&lt; 1 year<br></span><span class="percentage">'+parseInt(values[0])+'%</span>');
	    $($('.noUi-tooltip')[1]).html('<span class="year">1 year<br></span><span class="percentage">'+parseInt(values[1])+'%</span>');
	    $($('.noUi-tooltip')[2]).html('<span class="year">2 years<br></span><span class="percentage">'+parseInt(values[2])+'%</span>');
	    $($('.noUi-tooltip')[3]).html('<span class="year">3 years<br></span><span class="percentage">'+parseInt(values[3])+'%</span>');
	    $($('.noUi-tooltip')[4]).html('<span class="year">4 years<br></span><span class="percentage">'+parseInt(values[4])+'%</span>');
	    $($('.noUi-tooltip')[5]).html('<span class="year">&gt; 5 years<br></span><span class="percentage">'+parseInt(values[5])+'%</span>');    
	},
    cancel: function(){
	    this.vestingSlider.noUiSlider.set(this.vestingSlider.tmpValues);
	    $('#advancedOptions').modal('toggle');
	},
    confirm: function (){
	    var values = this.vestingSlider.noUiSlider.get();
	    $('#lessOneYear').val(parseInt(values[0]));
	    $('#afterOneYear').val(parseInt(values[1]));
	    $('#afterTwoYears').val(parseInt(values[2]));
	    $('#afterThreeYears').val(parseInt(values[3]));
	    $('#afterFourYears').val(parseInt(values[4]));
	    $('#afterFiveYears').val(parseInt(values[5]));

	    $('#advancedOptions').modal('toggle');
	}
};