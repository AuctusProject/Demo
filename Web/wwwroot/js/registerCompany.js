
$(document).ready(function () {
    var slider = document.getElementById('slider');

    noUiSlider.create(slider, {
	    start: [20, 40, 60, 80],
        step: 1,
        tooltips: true,
	    connect: true,
	    range: {
	        'min': 0,
	        'max': 100
	    }
	});

    $('.noUi-handle:nth-child(1)').attr('background', '#0243b7');
});