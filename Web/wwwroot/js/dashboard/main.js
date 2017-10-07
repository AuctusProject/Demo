﻿$(document).ready(function () {
    Dashboard.init();
    hub.on('paymentsCompleted', Dashboard.paymentsCompleted);
    hub.on('paymentsUncompleted', Dashboard.paymentsUncompleted);
    hub.on('readPaymentsError', Dashboard.readPaymentsError);
    hub.on('withdrawalCompleted', Dashboard.withdrawalCompleted);
    hub.on('withdrawalUncompleted', Dashboard.withdrawalUncompleted);
    hub.on('readWithdrawalError', Dashboard.readWithdrawalError);
});

var Dashboard = {
    init: function () {
        signalrDone = Dashboard.readTransactions;
        Dashboard.configTimeline();
        Dashboard.configPaymentWindow();
    },
    readTransactions: function () {
        Dashboard.readPayments();
        Dashboard.readWithdraw();
    },
    configTimeline: function () {
        $("#paymentBtn").on('click', function () {
            document.getElementById('slider').noUiSlider.set([1, 4]);
            $('#month').val(4);
            $('#paymentModal').modal('toggle');
        });
        $("#withdrawBtn").on('click', function () {
            $.ajax({
                url: urlGetPaymentInfo, data: Dashboard.getBaseData(), method: "GET",
                success: function (response) {
                    $('#employeeReceivable').text(Dashboard.getFormattedNumber(response.employeeSzaboCashback));
                    $('#companyReceivable').text(Dashboard.getFormattedNumber(response.employerSzaboCashback));
                    $('#withdrawModal').modal('toggle');
                }
            });
        });
    },
    configPaymentWindow: function () {
        this.paymentSlider = document.getElementById('slider');
        noUiSlider.create(this.paymentSlider, {
            start: [1, 60],
            step: 1,
            connect: true,
            tooltips: false,
            range: { 'min': 1, 'max': 60 }
        });
        $($('.noUi-connect')[0]).css('background-color', '#07dce0');
        $($('.noUi-origin')[0]).attr('disabled', 'disabled');
        $($('div.noUi-base div.noUi-origin div.noUi-handle')[0]).removeClass('noUi-handle');

        this.paymentSlider.noUiSlider.on('slide', function (values, handle) {
            $('#month').val(parseInt(values[1]));
        });
        $('#month').on('change', function () {
            document.getElementById('slider').noUiSlider.set([1, $(this).val()]);
        });
    },
    withdraw: function () {
        $('#withdrawModal').modal('toggle');
        Dashboard.setActionButtons(true);
        Dashboard.ajaxHubCall(urlGenerateWithdraw, Dashboard.getBaseData(), Dashboard.withdrawalUncompleted);
    },
    payment: function () {
        $('#paymentModal').modal('toggle');
        Dashboard.setActionButtons(true);
        var data = Dashboard.getBaseData();
        data["monthsAmount"] = $('#month').val();
        Dashboard.ajaxHubCall(urlGeneratePayment, data, Dashboard.paymentsUncompleted);
    },
    paymentsCompleted: function (response) {
        Dashboard.setTimeline(response);
        Dashboard.setSummary(response);  
        
        Dashboard.setActionButtons(false);
    },
    paymentsUncompleted: function (response) {
        Dashboard.setTimeline(response);
        Dashboard.setSummary(response);

        Dashboard.setActionButtons(true);
        Dashboard.readPayments();
    },
    readPaymentsError: function (response) {
        Dashboard.readPayments();
    },
    withdrawalCompleted: function (response) {



        Dashboard.setActionButtons(response);
    },
    withdrawalUncompleted: function (response) {



        Dashboard.setActionButtons(true);
        Dashboard.readWithdraw();
    },
    readWithdrawalError: function (response) {
        Dashboard.readWithdraw();
    },
    readPayments: function () {
        Dashboard.ajaxHubCall(urlReadPayment, Dashboard.getBaseData());
    },
    readWithdraw: function () {
        Dashboard.ajaxHubCall(urlReadWithdraw, Dashboard.getBaseData());
    },
    setSummary: function (progress) {
        if (progress) {
            $('#totalInvested').text(Dashboard.getFormattedNumber(progress.TotalInvested));
            $('#totalVested').text(Dashboard.getFormattedNumber(progress.TotalVested));
            $('#totalToken').text(Dashboard.getFormattedNumber(progress.TotalToken));
            $('#feePaid').text(Dashboard.getFormattedNumber(progress.TotalPensinonFundFee));
            $('#auctusFee').text(Dashboard.getFormattedNumber(progress.TotalAuctusFee));
            if (progress.NextVestingDate) {
                var partialDates = progress.NextVestingDate.split(' ');
                $('#nextVestingDate').html('<span>' + partialDates[0] + '</span><span>' + partialDates[1] + '</span><span>' + partialDates[2] + '</span>');
            }
        }
    },
    setTimeline: function (progress) {
        if (progress && progress.LastPeriod && progress.LastPeriod > 0) {
            $('div.timeline-grid-horizontal-line').each(function (i) {
                var current = $(this);
                if (progress.LastPeriod >= parseInt(current.data('line'))) {
                    current.removeClass('timeline-grid-horizontal-line-pending');
                    current.addClass('timeline-grid-horizontal-line-complete');
                } else {
                    current.removeClass('timeline-grid-horizontal-line-complete');
                    current.addClass('timeline-grid-horizontal-line-pending');
                }
            });  
        }
    },
    setActionButtons: function (disabled) {
        $("#comfirmWithdraw").prop("disabled", disabled);
        $("#comfirmPayment").prop("disabled", disabled);
    },
    getFormattedNumber: function (number) {
        return (Math.round(number * 100) / 100).toLocaleString({ minimumFractionDigits: 2 });
    },
    getBaseData: function () {
        return {
            contractAddress: pensionFundData.contractAddress
        };
    },
    ajaxHubCall: function (url, data, successFunction, errorFunction) {
        $.ajax({
            url: url,
            data: data,
            method: "POST",
            beforeSend: function (request) {
                request.setRequestHeader("HubConnectionId", hub.connection.id);
            },
            success: function (response) {
                if (successFunction) {
                    successFunction(response);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (errorFunction) {
                    errorFunction();
                }
            }
        });
    }
 };