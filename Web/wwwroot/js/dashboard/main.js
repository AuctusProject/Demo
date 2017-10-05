﻿﻿$(document).ready(function () {
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
        Dashboard.configTimeline();
        Dashboard.configPaymentWindow();
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
            $('#withdrawModal').modal('toggle');
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
        Dashboard.ajaxHubCall(urlGenerateWithdraw, Dashboard.getBaseData(), Dashboard.withdrawalUncompleted);
    },
    payment: function () {
        $('#paymentModal').modal('toggle');
        var data = Dashboard.getBaseData();
        data["monthsAmount"] = $('#month').val();
        Dashboard.ajaxHubCall(urlGeneratePayment, data, Dashboard.paymentsUncompleted);
    },
    paymentsCompleted: function (response) {
         
    },
    paymentsUncompleted: function (response) {

        Dashboard.readPayments();
    },
    readPaymentsError: function (response) {

    },
    withdrawalCompleted: function (response) {

    },
    withdrawalUncompleted: function (response) {

        Dashboard.readWithdraw();
    },
    readWithdrawalError: function (response) {

    },
    readPayments: function () {
        Dashboard.ajaxHubCall(urlReadPayment, Dashboard.getBaseData());
    },
    readWithdraw: function () {
        Dashboard.ajaxHubCall(urlReadWithdraw, Dashboard.getBaseData());
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