﻿$(document).ready(function () {
    $(".next-step").click(function (e) {
        if ($('.next-button').attr('disabled') == null) {
            var stepId = $(this).closest('.step').data('step-id');   
            if (stepId == 2) {
                $('.asset-input-group input').each(function (index, element) {
                    if (!$(element).val())
                        $(element).val('0');
                });
                deploy();
            }
            else
                nextTab(stepId);
        }
    });

   
    $('#fundFeeInput').blur(validateFormToEnableNextButton);
     
    $(".prev-step").click(function (e) {
        var stepId = $(this).closest('.step').data('step-id');
        prevTab(stepId);
    });
     
    $(".asset-input-group-left").click(function (e) {
        var sequential = $(this).data('carousel-sequential');
        openAssetInformationModal(sequential);
    });

    Wizard.Components.ContractDeploy.CodeMirror = CodeMirror.fromTextArea(Wizard.Components.ContractDeploy.Code[0], {
        lineNumbers: true,
        mode: "text/javascript"
    });

    loadAssetsGraphs();
    registerCompanyForm.init()
});

function validateFormToEnableNextButton() {
    if ($('.total-assets').hasClass('has-success') && $('#fundFeeInput').val() != "" &&  $('#registerFund').valid())
        $('.next-button').removeAttr('disabled');
    else
        $('.next-button').attr('disabled', 'disabled');
}

function nextAssetReference() {
    Wizard.Components.AssetsCarousel.carousel('next');
}

function previousAssetReference() {
    Wizard.Components.AssetsCarousel.carousel('prev');
}

function openAssetInformationModal(sequential) {
    Wizard.Components.AssetsCarousel.carousel(sequential);
    $('#assetInformationModal').modal('show');    
};

function loadAssetsGraphs() {
    $.ajax({
        url: urlGetGoldReference,
        method: "GET",
        success: loadGoldGraph,
        error: function (xhr, ajaxOptions, thrownError) {
            
        }
    });           
    $.ajax({
        url: urlGetSP500Reference,
        method: "GET",
        success: loadSPGraph,
        error: function (xhr, ajaxOptions, thrownError) {
            
        }
    });    
    $.ajax({
        url: urlGetVWEHXReference,
        method: "GET",
        success: loadVWEHXGraph,
        error: function (xhr, ajaxOptions, thrownError) {
            
        }
    }); 
    $.ajax({
        url: urlGetMSCIWorldReference,
        method: "GET",
        success: loadMsciGraph,
        error: function (xhr, ajaxOptions, thrownError) {
            
        }
    }); 
    $.ajax({
        url: urlGetBitcoinReference,
        method: "GET",
        success: loadBtcGraph,
        error: function (xhr, ajaxOptions, thrownError) {
            
        }
    });    
}

function loadGoldGraph(data) {
    createGraph("goldGraph", data.values);
}

function loadSPGraph(data) {
    createGraph("sandpGraph", data.values);
}

function loadVWEHXGraph(data) {
    createGraph("vwehxGraph", data.values);
}

function loadMsciGraph(data) {
    createGraph("msciGraph", data.values);
}

function loadBtcGraph(data) {
    createGraph("btcGraph", data.values);
}

var openDate = new Date();
function createGraph(elementId, data) {
    g = new Dygraph(
        document.getElementById(elementId), data,
        {
            labels: ["Year", "Value"],
            color: "#00bdff",
            width: 380,
            height: 200,
            axisLabelFontSize: 10,
            strokeWidth: 2,
            axes: {
                x: {
                    drawGrid: false,
                    axisLabelFormatter: function (x) {
                        var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(x));
                        return date.getMonth()+1 + "/" + date.getFullYear();
                    },
                    axisLineColor: 'white'
                },
                y: {
                    axisLabelFormatter: function (y) {
                        return '$' + y.toFixed(2);
                    },
                    axisLineColor: 'white'
                },
            },
            gridLineColor: '#d9e0e6',
            legend: 'follow',
            legendFormatter: legendFormatter,
            digitsAfterDecimal: 2 
        }
    );
}


function legendFormatter(data) {
    if (data.x == null) {
        // This happens when there's no selection and {legend: 'always'} is set.
        return '';
    }

    var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(data.xHTML));
    var html = "<span class='legend-x-value'>" + (parseInt(date.getMonth()) + 1) + "/" + date.getFullYear() +"</span>";
    data.series.forEach(function (series) {
        if (!series.isVisible) return;
        var labeledData = '$'+series.yHTML;
        if (series.isHighlighted) {
            labeledData = '<b>' + labeledData + '</b>';
        }
        html += "<span class='legend-y-value'>" + '<br>' + labeledData + "</span>";
    });
    return html;
}



function nextTab(currentStepId) {
    var $active = $('#step' + currentStepId);
    var $nextTab = $('#step' + (currentStepId + 1));
    $active.hide();
    $nextTab.show();
    if (currentStepId == 1) {
        registerEmployeeForm.init();
    } else if (currentStepId == 2){
        
    }
}

function prevTab(currentStepId) {
    var $active = $('#step' + currentStepId);
    var $prevTab = $('#step' + (currentStepId - 1));
    $active.hide();
    $prevTab.show();
    $('.next-button').removeAttr('disabled');
}

function goToTab(currentStepId, stepId) {
    var $active = $('#step' + currentStepId);
    var $prevTab = $('#step' + (stepId));
    $active.hide();
    $prevTab.show();
    $('.next-button').removeAttr('disabled');
}

function deploy() {
    Wizard.Operations.Save();
}

var Wizard = {};

Wizard.Components = {
    Model: {
        Fund: {
            Name: $('#fundNameInput'),
            Fee: $('#fundFeeInput'),
            GoldPercentage: $('#goldAllocationInput'),
            SPPercentage: $('#spAllocationInput'),
            VWEHXPercentage: $('#vwehxAllocationInput'),
            MSCIPercentage: $('#msciAllocationInput'),
            BitcoinPercentage: $('#bitcoinAllocationInput'),
        },
        Company: {
            Name: $('#companyNameInput'),
            BonusFee: $('#employerMatchingInput'),
            MaxBonusFee: $('#maxCounterpartInput'),
            VestingRules: {
            }
        },
        Employee: {
            Name: $('#employeeNameInput'),
            Salary: $('#salaryInput'),
            ContributionPercentage: $('#contributionInput')
        }
    },
    VestingFields: {
        VestingPeriod: $('.vesting-period'),
        LessOneYear: $('#lessOneYear'),
        AfterOneYear: $('#afterOneYear'),
        AfterTwoYears: $('#afterTwoYears'),
        AfterThreeYears: $('#afterThreeYears'),
        AfterFourYears: $('#afterFourYears'),
        AfterFiveYears: $('#afterFiveYears')
    },
    AssetsCarousel: $("#carouselExampleIndicators"),
    ContractDeploy: {
        PensionFundId: null,
        Transaction: null,
        NextButton: $('.next-button'),
        ButtonsControl: $('.buttons-control-div'),
        GeneratingContract: $('.generating-contract'),
        ContractBeingDeployedDiv: $('#contract-being-deployed'),
        ContractDeployedDiv: $('#contract-deployed'),
        ContractCodeWrapper: $('.contract-deploy-code-wrapper'),
        Code: $('#contract-deploy-code'),
        TransactionIdLink: $('.contract-deploy-tx-id-link'),
        ContractAddressLink: $('.contract-deploy-contract-address-link'),
    },
    WizardRow: $('#wizard-row'),
    ErrorMessage: $('.error-message')
};

Wizard.Operations = {
    Save: function () {
        var model = convertModel(Wizard.Components.Model);
        model.Company.VestingRules = GetVestingRules();
        model.Captcha = $("#g-recaptcha-response").val();

        if (Wizard.Operations.Validate(model)) {
            Wizard.Operations.ShowGeneratingContract(model);

            $.ajax({
                url: urlSaveFund,
                method: "POST",
                data: model,
                success: Wizard.Operations.OnSave,
                error: function (xhr, ajaxOptions, thrownError) {
                    Wizard.Operations.OnSaveError(xhr.responseText);
                }
            });
        }
    },
    ShowGeneratingContract: function (model) {
        Wizard.Components.ContractDeploy.ButtonsControl.hide();
        Wizard.Components.ContractDeploy.GeneratingContract.show();
        Wizard.Components.ErrorMessage.hide();
    },
    OnSave: function (data) {
        Wizard.Components.ContractDeploy.PensionFundId = data.pensionFundId;
        Wizard.Operations.OnCreationUncompleted();
        Wizard.Components.ErrorMessage.hide();
    },
    OnCreationCompleted: function (data) {
        Wizard.Components.ContractDeploy.Transaction = data.transactionHash;
        Wizard.Operations.OnDeployUncompleted();
        Wizard.Components.ErrorMessage.hide();
        Wizard.Components.ContractDeploy.ContractDeployedDiv.hide();
        Wizard.Components.ContractDeploy.TransactionIdLink.attr("href", Parameter.BlockExplorerUrl + "/tx/" + Wizard.Components.ContractDeploy.Transaction);
        Wizard.Components.ContractDeploy.CodeMirror.setValue(js_beautify(data.smartContractCode, { indent_size: 4 })); 
        setTimeout(function () { Wizard.Components.ContractDeploy.CodeMirror.refresh(); }, 1);
        Wizard.Components.ContractDeploy.ContractCodeWrapper.show();
        Wizard.Components.ContractDeploy.ContractBeingDeployedDiv.show();
        nextTab(2);
    },
    OnCreationUncompleted: function (pensionFundId) {
        var id = Wizard.Components.ContractDeploy.PensionFundId ? Wizard.Components.ContractDeploy.PensionFundId : pensionFundId;
        $.ajax({
            url: urlCheckCreation,
            method: "POST",
            data: { pensionFundId: id },
            success: function (response) {
                if (!response || !response.success) {
                    setTimeout(function () { Wizard.Operations.OnCreationUncompleted(id); }, 5000);
                } else {
                    Wizard.Operations.OnCreationCompleted(response);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                Wizard.Operations.OnCreationUncompleted();
            }
        });
    },
    OnDeployCompleted: function (data) {
        Wizard.Components.ErrorMessage.hide();
        Wizard.Components.ContractDeploy.ContractBeingDeployedDiv.hide();
        Wizard.Components.ContractDeploy.ContractDeployedDiv.show();
        Wizard.Components.ContractDeploy.ContractAddressLink.attr("href", Parameter.BlockExplorerUrl + "/address/" + data.address);
        Wizard.Components.ContractDeploy.GeneratingContract.hide();
        Wizard.Components.ContractDeploy.ButtonsControl.show()
        Wizard.Components.ContractDeploy.NextButton.removeAttr('disabled');
        Wizard.Components.ContractDeploy.NextButton.unbind('click').click(function () { Wizard.Operations.GoToDashBoard(data.address); });
        Wizard.Components.ContractDeploy.NextButton.show();
    },
    OnDeployUncompleted: function (deployTransaction) {
        var hash = Wizard.Components.ContractDeploy.Transaction ? Wizard.Components.ContractDeploy.Transaction : deployTransaction;
        $.ajax({
            url: urlCheckDeploy,
            method: "POST",
            data: { transactionHash: hash },
            success: function (response) {
                if (!response || !response.success) {
                    setTimeout(function () { Wizard.Operations.OnDeployUncompleted(hash); }, 5000);
                } else {
                    Wizard.Operations.OnDeployCompleted(response);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                Wizard.Operations.OnDeployUncompleted();
            }
        });
    },
    OnSaveError: function (errorDescription) {
        Wizard.Components.ContractDeploy.ButtonsControl.show();
        Wizard.Components.ContractDeploy.GeneratingContract.hide();
        if (errorDescription) {
            Wizard.Components.ErrorMessage.html('<span>' + errorDescription + '</span>');
        }
        Wizard.Components.ErrorMessage.show();
    },
    OnDeployError: function () {
        grecaptcha.reset();
        goToTab(4, 3);
        Wizard.Components.ContractDeploy.ButtonsControl.show();
        Wizard.Components.ContractDeploy.GeneratingContract.hide();
        Wizard.Components.ErrorMessage.html('<span> We’re sorry, we had an unexpected error! Please try again in a minute.</span>');
        Wizard.Components.ErrorMessage.show();
    },
    GoToDashBoard: function (ContractAddress) {
        window.location = "/pensionFund/" + ContractAddress;
    },
    Validate: function (model) {
        return true;
    }
};

function convertModel(obj) {
    var model = {};
    for (prop in obj) {
        if (obj[prop] instanceof jQuery) {
            if (!isNaN(obj[prop].val())) {
                if (obj[prop].val().indexOf('.') != -1) {
                    model[prop] = JSON.stringify(parseFloat(obj[prop].val()));
                }
                else {
                    model[prop] = parseInt(obj[prop].val());
                }
            }
            else {
                model[prop] = obj[prop].val();
            }
        }
        else {
            model[prop] = convertModel(obj[prop]);
        }
    }
    return model;
}

function GetVestingRules() {
    var periods = $('.vesting-period');
    var rules = [];
    periods.each(function (index, elem) {
        var vestingPeriod = $(elem).html();
        var vestingPercentage = $(elem).siblings('.input-percentage').find('.vesting-percentage').val();
        rules.push({ Percentage: parseInt(vestingPercentage), Period: parseInt(vestingPeriod) });
    });
    
    return rules;
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};