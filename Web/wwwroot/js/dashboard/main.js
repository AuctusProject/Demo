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
    remainingPayments: 60,
    finished: false,
    init: function () {
        if (!signalrDone) {
            signalrDone = Dashboard.readTransactions;
        } else {
            Dashboard.readTransactions();
        }
        Dashboard.configTimeline();
        Dashboard.configPaymentWindow();
        $('div.timeline-grid').click();
    },
    readTransactions: function () {
        Dashboard.readPayments();
        Dashboard.readWithdraw();
    },
    configTimeline: function () {
        Dashboard.disableActionButtons();
        $('div.timeline-grid').on('click', function () {
            $('div.popup-timeline-footer').on('click', function () {
                $('div.timeline-grid').click();
            });
        });
    },
    configPaymentWindow: function () {
        $('#month').on('change', function () {
            document.getElementById('slider').noUiSlider.set([1, $(this).val()]);
        });
    },
    withdraw: function () {
        $('#withdrawModal').modal('toggle');
        Dashboard.disableActionButtons();
        Dashboard.ajaxHubCall(urlGenerateWithdraw, Dashboard.getBaseData(), Dashboard.withdrawalUncompleted);
    },
    payment: function () {
        $('#paymentModal').modal('toggle');
        Dashboard.disableActionButtons();
        var data = Dashboard.getBaseData();
        data["monthsAmount"] = $('#month').val();
        Dashboard.ajaxHubCall(urlGeneratePayment, data, Dashboard.paymentsUncompleted);
    },
    paymentsCompleted: function (response) {
        Dashboard.setPayment(response);
        if (!Dashboard.finished) {
            Dashboard.enableActionButtons();
        }
    },
    paymentsUncompleted: function (response) {
        Dashboard.setPayment(response);
        Dashboard.disableActionButtons();
        Dashboard.readPayments();
    },
    readPaymentsError: function (response) {
        Dashboard.readPayments();
    },
    withdrawalCompleted: function (response) {
        if (response) {
            Dashboard.finished = true;
            Dashboard.disableActionButtons();
            $('.employee-receivable').text(Dashboard.getFormattedNumber(response.EmployeeSzaboCashback));
            $('.company-receivable').text(Dashboard.getFormattedNumber(response.EmployerSzaboCashback));
            $('#employeeWalletLink').attr("href", Parameter.BlockExplorerUrl + "/address/" + pensionFundData.employeeAddress);
            $('#employerWalletLink').attr("href", Parameter.BlockExplorerUrl + "/address/" + pensionFundData.companyAddress);
            $('#withdrawTransactionLink').attr("href", Parameter.BlockExplorerUrl + "/tx/" + response.TransactionHash);
            $('.share-symbol').click();
            $('#withdrawCompletedModal').modal('toggle');
        } 
    },
    withdrawalUncompleted: function (response) {
        Dashboard.disableActionButtons();
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
    setPayment: function (response) {
        Dashboard.setTimeline(response);
        Dashboard.setSummary(response);
        Dashboard.setTransactionHistory(response);
        Dashboard.charts && Dashboard.charts.update(response);
        if (response && response.TransactionHistory) {
            Dashboard.remainingPayments = 60 - response.TransactionHistory.length;
        }
    },
    setPaymentSlider: function () {
        this.paymentSlider = document.getElementById('slider');
        if (this.paymentSlider.noUiSlider) {
            this.paymentSlider.noUiSlider.destroy();
        }
        noUiSlider.create(this.paymentSlider, {
            start: [1, Dashboard.remainingPayments],
            step: 1,
            connect: true,
            tooltips: false,
            range: { 'min': 1, 'max': Dashboard.remainingPayments }
        });
        $($('.noUi-connect')[0]).css('background-color', '#07dce0');
        $($('.noUi-origin')[0]).attr('disabled', 'disabled');
        $($('div.noUi-base div.noUi-origin div.noUi-handle')[0]).removeClass('noUi-handle');
        this.paymentSlider.noUiSlider.on('slide', function (values, handle) {
            $('#month').val(parseInt(values[1]));
        });
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
    disableActionButtons: function () {
        $("#paymentBtn").off("click");
        $("#paymentBtn").removeClass("timeline-btn");
        $("#withdrawBtn").off("click");
        $("#withdrawBtn").removeClass("timeline-btn");
    },
    enableActionButtons: function () {
        $("#paymentBtn").on('click', function () {
            Dashboard.setPaymentSlider();
            var startValue = (Dashboard.remainingPayments > 4 ? 4 : Dashboard.remainingPayments);
            document.getElementById('slider').noUiSlider.set([1, startValue]);
            $('#month').val(startValue);
            $('#paymentModal').modal('toggle');
        });
        $("#withdrawBtn").on('click', function () {
            $.ajax({
                url: urlGetPaymentInfo, data: Dashboard.getBaseData(), method: "GET",
                success: function (response) {
                    $('.employee-receivable').text(Dashboard.getFormattedNumber(response.employeeSzaboCashback));
                    $('.company-receivable').text(Dashboard.getFormattedNumber(response.employerSzaboCashback));
                    $('#withdrawModal').modal('toggle');
                }
            });
        });
        $("#paymentBtn").addClass("timeline-btn");
        $("#withdrawBtn").addClass("timeline-btn");
    },
    getFormattedNumber: function (number) {
        return numeral((Math.round(number * 100) / 100)).format('0.000 a');
    },
    TransactionHistoryRowTemplate: $('.row-template').outerHTML(),
    setTransactionHistory: function (response) {

        if (response.TransactionHistory) {
            $('.table-content').html('');
            $('.table-content').show();
            for (var i = 0; i < response.TransactionHistory.length; i++) {
                var transaction = response.TransactionHistory[i];
                var row = Dashboard.TransactionHistoryRowTemplate;
                row = row.replace('{TRANSACTION_DATE}', transaction.PaymentDate == undefined ? " - " : transaction.PaymentDate);
                row = row.replace('{TRANSACTION_STATUS}', transaction.Status);
                row = row.replace('{TRANSACTION_STATUS_CLASS}', transaction.Status.toLowerCase());
                row = row.replace('{EMPLOYEE_TOKEN}', (transaction.EmployeeToken == undefined || transaction.EmployeeToken == null)? " - " : transaction.EmployeeToken.toFixed(2));
                row = row.replace('{EMPLOYER_TOKEN}', (transaction.CompanyToken == undefined || transaction.CompanyToken == null) ? " - " : transaction.CompanyToken.toFixed(2));
                if (transaction.EmployeeTransactionHash != null && transaction.EmployeeTransactionHash != '') {
                    row = row.replace('{EMPLOYEE_TRANSACTION_LINK}', Parameter.BlockExplorerUrl + "/tx/" + transaction.EmployeeTransactionHash);
                    row = row.replace('{EMPLOYEE_TRANSACTION_LINK_CLASS}', 'visible');
                }
                else {
                    row = row.replace('{EMPLOYEE_TRANSACTION_LINK_CLASS}', 'hidden');
                }
                if (transaction.CompanyTransactionHash != null && transaction.CompanyTransactionHash != '') {
                    row = row.replace('{EMPLOYER_TRANSACTION_LINK}', Parameter.BlockExplorerUrl + "/tx/" + transaction.CompanyTransactionHash);
                    row = row.replace('{EMPLOYER_TRANSACTION_LINK_CLASS}', 'visible');
                }
                else {
                    row = row.replace('{EMPLOYER_TRANSACTION_LINK_CLASS}', 'hidden');
                }
                $('.row-template').show();

                $('.table-content').append(row);
            }
        }        
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