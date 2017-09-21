
$(document).ready(function () {
    configVestingSlider();
});

function configVestingSlider(){
    var slider = document.getElementById('slider');

    var startValues = [0, 20, 40, 60, 80, 100]
    noUiSlider.create(slider, {
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
    setTooltipValues(startValues);

    slider.noUiSlider.on('slide', function( values, handle ) {
        $($('.noUi-tooltip')[handle]).removeClass('unselected');
        setTooltipValues(values);
    });

    slider.noUiSlider.on('start', function( values, handle ) {
        $($('.noUi-tooltip')[handle]).removeClass('unselected');
        hideTooltipsExceptSelected(handle);
        setTooltipValues(values);
    });

    slider.noUiSlider.on('change', function( values, handle ) {
        setTooltipValues(values);
    });

    slider.noUiSlider.on('set', function( values, handle ) {
        setTooltipValues(values);
    });

    slider.noUiSlider.on('end', function( values, handle ) {
        $($('.noUi-tooltip')[handle]).addClass('unselected');
        showAllTooltips();
    });
}

function showAllTooltips(){
    for (var i = 0; i < 6; ++i){
        $($('.noUi-tooltip')[i]).css('display', 'block');
    }
}

function hideTooltipsExceptSelected(indexSelected){
    for (var i = 0; i < 6; ++i){
        if (i != indexSelected){
            $($('.noUi-tooltip')[i]).css('display', 'none');
        }
    }
}

function setTooltipValues(values){
    $($('.noUi-tooltip')[0]).html('<span class="year">&lt; 1 year<br></span><span class="percentage">'+parseInt(values[0])+'%</span>');
    $($('.noUi-tooltip')[1]).html('<span class="year">1 year<br></span><span class="percentage">'+parseInt(values[1])+'%</span>');
    $($('.noUi-tooltip')[2]).html('<span class="year">2 years<br></span><span class="percentage">'+parseInt(values[2])+'%</span>');
    $($('.noUi-tooltip')[3]).html('<span class="year">3 years<br></span><span class="percentage">'+parseInt(values[3])+'%</span>');
    $($('.noUi-tooltip')[4]).html('<span class="year">4 years<br></span><span class="percentage">'+parseInt(values[4])+'%</span>');
    $($('.noUi-tooltip')[5]).html('<span class="year">&gt; 5 years<br></span><span class="percentage">'+parseInt(values[5])+'%</span>');    
}