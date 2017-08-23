$(document).ready(function () {
    //Initialize tooltips
    $('.nav-tabs > li a[title]').tooltip();

    //Wizard
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
        var $target = $(e.target);

        if ($target.parent().hasClass('disabled')) {
            return false;
        }
    });

    $(".next-step").click(function (e) {
        var $active = $('.wizard .nav-tabs li.active');
        $active.next().removeClass('disabled');
        nextTab($active);
    });

    $(".prev-step").click(function (e) {
        var $active = $('.wizard .nav-tabs li.active');
        prevTab($active);

    });
   
    Wizard.Components.Contract.Initialize();
    Wizard.Components.Buttons.Save.click(Wizard.Operations.Save);
});

function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}

function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}

Wizard = {};

Wizard.Components = {
    Model: {
        Fund: {
            Name: $('#fundName'),
            Fee: $('#fee'),
            LatePaymentFee: $('#latePaymentFee')
        },
        Company: {
            Name: $('#companyName'),
            BonusFee: $('#bonusFee'),
            MaxBonusFee: $('#maxBonusFee')
        },
        Employee: {
            Name: $('#employeeName'),
            Salary: $('#salary'),
            ContributionPercentage: $('#contributionPercentage')
        }
    },
    Contract: {
        RowTemplateHtml: $('#vesting-row-template').prop('outerHTML'),
        Content: $('#vesting-content'),
        MaxVestingIndex: 0,
        Initialize: function () {
            $('#vesting-row-template').remove();
            //createSlider(++Wizard.Components.Contract.MaxVestingIndex);
        }
    },
    Buttons: {
        Save: $('#wizard-save-button')
    }
};

Wizard.Operations = {
    Save: function () {
        var model = convertModel(Wizard.Components.Model);
        $.post("Home/Save", model);
    }
};

function convertModel(obj) {
    var model = {};
    for (prop in obj) {
        if (obj[prop] instanceof jQuery) {
            model[prop] = obj[prop].val();
        }
        else {
            model[prop] = convertModel(obj[prop]);
        }
    }
    return model;
}

function createSlider(index) {
    Wizard.Components.Contract.Content.append(Wizard.Components.Contract.RowTemplateHtml.replaceAll('template', index));
    $('#vesting-row-' + index).show();

    var minValue = getPreviousSliderValue(index) + 1;
    
    $('#slider-range-'+index).slider({
        range: index === 1 ? "min" : true,
        value: index === 1 ? 100 : null,
        values: index === 1 ? null : [minValue, 100],
        min: 0,
        max: 100,
        stop: function (event, ui) {
            onStopSlider($(this).data('index'), ui.value);
        },
        create: function () {
            if (index === 1) {
                $(this).children('#slider-value-' + index).text($(this).slider("value"));
            }
        },
        slide: function (event, ui) {
            if (index === 1) {
                $(this).children('#slider-value-' + index).text(ui.value);
            }
            else {
                if (ui.value < minValue)
                    return false;
                $(this).children('#slider-value-' + index).text(ui.values[1]);
            }
        }
    });
}

function getPreviousSliderValue(index) {
    if (index <= 1)
        return 0;
    var previousSliderValue = $('#slider-range-' + (index - 1)).slider("value");
    return previousSliderValue; 
}

function onStopSlider(sliderIndex, value) {
    if (sliderIndex === Wizard.Components.Contract.MaxVestingIndex && value < 100)
        createSlider(++Wizard.Components.Contract.MaxVestingIndex);
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};