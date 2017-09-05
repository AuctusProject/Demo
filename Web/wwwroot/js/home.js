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
        if ($(this).closest('form').valid()) {
            var $active = $('.wizard .nav-tabs li.active');
            $active.next().removeClass('disabled');
            nextTab($active);
        }
    });

    $(".prev-step").click(function (e) {
        var $active = $('.wizard .nav-tabs li.active');
        prevTab($active);

    });

    Wizard.Components.Contract.Initialize();
    Wizard.Components.Buttons.Save.click(Wizard.Operations.Save);

    $('form').validate();

    Wizard.Components.ContractDeploy.CodeMirror = CodeMirror.fromTextArea(Wizard.Components.ContractDeploy.Code[0], {
        lineNumbers: true,
        mode: "text/javascript"
    });
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
        },
        Contract: {
            VestingRules: {}
        }
    },
    Contract: {
        RowTemplateHtml: $('#vesting-row-template').prop('outerHTML'),
        Content: $('#vesting-content'),
        Initialize: function () {
            $('#vesting-row-template').remove();
            createSlider(1, 0);
        }
    },
    Buttons: {
        Save: $('#wizard-save-button')
    },
    ContractDeploy: {
        ContractDeployRow: $('#contract-deploy-row'),
        NextButton: $('#btn-contract-deploy-next'),        
        Title: $('#contract-deploy-title'),
        Icon: $('#contract-deploy-icon'),
        ContractCodeWrapper: $('#contract-deploy-code-wrapper'),
        Code: $('#contract-deploy-code'),
        TransactionIdLink: $('#contract-deploy-tx-id-link'),
        TransactionIdTitle: $('#contract-deploy-tx-id-title'),
    },
    WizardRow: $('#wizard-row')
};

Wizard.Operations = {
    Save: function () {
        var model = convertModel(Wizard.Components.Model);
        model.Contract.VestingRules = GetVestingRules();

        if (Wizard.Operations.Validate(model)) {

            Wizard.Operations.ShowGeneratingContract(model);

            $.ajax({
                url: "Home/Save",
                beforeSend: function (request) {
                    request.setRequestHeader("HubConnectionId", hub.connection.id);
                },
                data: model,
                success: Wizard.Operations.OnSave,
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }
    },
    ShowGeneratingContract: function (model) {
        Wizard.Components.ContractDeploy.ContractCodeWrapper.hide();
        Wizard.Components.ContractDeploy.Title.html("Generating Smart Contract...");
        Wizard.Components.WizardRow.hide();
        Wizard.Components.ContractDeploy.NextButton.hide();
        Wizard.Components.ContractDeploy.ContractDeployRow.show();
    },
    OnSave: function (data) {
        Wizard.Components.ContractDeploy.Title.html("Deploying Smart Contract...");
        Wizard.Components.ContractDeploy.TransactionIdLink.attr("href", "https://ropsten.etherscan.io/tx/" + data.transactionHash);
        Wizard.Components.ContractDeploy.TransactionIdLink.html(data.transactionHash);
        Wizard.Components.ContractDeploy.TransactionIdTitle.show();
        Wizard.Components.ContractDeploy.CodeMirror.setValue(js_beautify(data.smartContractCode, { indent_size: 4 }));
        setTimeout(function () { Wizard.Components.ContractDeploy.CodeMirror.refresh(); },1);
        Wizard.Components.ContractDeploy.ContractCodeWrapper.show();
    },
    OnDeployCompleted: function (data) {
        Wizard.Components.ContractDeploy.Title.html("<i class='fa fa-check-circle-o'></i> Deploy Completed!");
        Wizard.Components.ContractDeploy.Title.addClass("text-success");
        Wizard.Components.ContractDeploy.Icon.html("Address: <a target='_blank' href='https://ropsten.etherscan.io/address/" + data.Address + "'>" + data.Address + "</a>");
        Wizard.Components.ContractDeploy.Icon.removeClass();
        Wizard.Components.ContractDeploy.NextButton.removeAttr('disabled');
        Wizard.Components.ContractDeploy.NextButton.click(function () { Wizard.Operations.GoToDashBoard(data.TransactionHash); });
        Wizard.Components.ContractDeploy.NextButton.show();
    },
    GoToDashBoard: function (PensionFundOptionAddress) {
        alert('Go to dashboard: ' + PensionFundOptionAddress);
    },
    Validate: function (model) {
        var previousVestingRule = null;
        for (var i = 0; i < model.Contract.VestingRules.length; i++) {
            if (previousVestingRule != null) {
                if (model.Contract.VestingRules[i].Period <= previousVestingRule.Period) {
                    $('#vesting-error').text("Period must be crescent");
                    $('#vesting-content').addClass('has-error');
                    $('#vesting-error').show();
                    return false;
                }
            }
            previousVestingRule = model.Contract.VestingRules[i];
        }
        $('#vesting-content').removeClass('has-error');
        $('#vesting-error').hide();
        return true;
    }
};

function GetVestingRules() {
    var rules = [];
    $('.vesting-row').each(function (i, element) {
        var period = $('.vesting-period', element).val();
        var percentage = $('.vesting-percentage', element).text();
        rules.push({ Percentage: parseInt(percentage), Period: parseInt(period) });
    });
    return rules;
}


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

function disableIfIsOnlyOneButtonAndEnableIfIsMore() {
    var buttons = $('.btn-remove-slider');
    if (buttons.length == 1)
        buttons.prop('disabled', true);
    else
        buttons.prop('disabled', false);
}

function createSlider(index, currentIndex) {
    Wizard.Components.Contract.Content.append(Wizard.Components.Contract.RowTemplateHtml.replaceAll('template', index));
    disableIfIsOnlyOneButtonAndEnableIfIsMore();
    $("#btn-remove-" + index).click(
        function () {
            var currentSlider = $('#slider-range-' + index);

            var previousIndex = currentSlider.slider('option', 'previousIndex');
            var previousSlider = $('#slider-range-' + previousIndex);

            var nextIndex = currentSlider.slider('option', 'nextIndex');
            var nextSlider = $('#slider-range-' + nextIndex);

            previousSlider.slider('option', 'nextIndex', nextIndex);
            nextSlider.slider('option', 'previousIndex', previousIndex);

            if (nextSlider.length)
                setSliderMinValue(nextSlider.slider('option', 'myIndex'), previousSlider.length ? previousSlider.slider('values')[1] : 0);

            if (previousSlider.length)
                setSliderMaxValue(previousSlider.slider('option', 'myIndex'), nextSlider.length ? nextSlider.slider('values')[0] : 100);

            $('#vesting-row-' + index).remove();
            disableIfIsOnlyOneButtonAndEnableIfIsMore();
        }
    );


    $('#vesting-row-' + index).show();

    var minValue = getPreviousSliderValue(currentIndex);

    $('#slider-range-' + index).slider({
        range: true,
        value: $(this).data('index') === 1 ? 100 : minValue,
        values: [minValue, 100],
        min: 0,
        max: 100,
        stop: function (event, ui) {
            if (ui.value == ui.values[0])
                return false;

            onStopSlider($(this).slider('option', 'myIndex'), ui.value);
        },
        create: function () {
            setSliderValuesText(this, $(this).slider('option', 'myIndex'), [minValue, 100]);
        },
        slide: function (event, ui) {
            if (ui.value == ui.values[0])
                return false;

            if (ui.value >= getSliderMaxValue($(this).slider('option', 'nextIndex'))) {
                setSliderMaxValue($(this).slider('option', 'myIndex'), getSliderMaxValue($(this).slider('option', 'nextIndex')) - 1);
                return false;
            }

            setSliderMinValue($(this).slider('option', 'nextIndex'), ui.value);
            setSliderValuesText(this, $(this).slider('option', 'myIndex'), ui.values);
        },
        myIndex: index,
        nextIndex: index + 1,
        previousIndex: currentIndex
    });
}

function setSliderMaxValue(index, value) {
    var element = $('#slider-range-' + (index));
    element.slider('values', [getSliderMinValue(index), value]).change();
    setSliderMinValue(element.slider('option', 'nextIndex'), value);
    setSliderValuesText(element, index, element.slider("values"));
}

function setSliderValuesText(element, index, values) {
    $(element).children('#slider-value-1-' + index).text(values[0]);
    $(element).children('#slider-value-2-' + index).text(values[1]);
}

function setSliderMinValue(index, value) {
    var element = $('#slider-range-' + (index));
    element.slider('values', [value, getSliderMaxValue(index)]).change();
    setSliderValuesText(element, index, element.slider("values"));
}

function getPreviousSliderValue(index) {
    if (index < 1)
        return 0;

    return getSliderMaxValue(index);
}

function getSliderMinValue(index) {
    return previousSliderValue = $('#slider-range-' + (index)).slider("values")[0];
}

function getSliderMaxValue(index) {
    return previousSliderValue = $('#slider-range-' + (index)).slider("values")[1];
}

function onStopSlider(index, value) {
    var currentSlider = $('#slider-range-' + index);
    var nextIndex = currentSlider.slider('option', 'nextIndex');
    var nextSlider = $('#slider-range-' + nextIndex);

    if (!nextSlider.length && value < 100)
        createSlider(nextIndex, index);
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};