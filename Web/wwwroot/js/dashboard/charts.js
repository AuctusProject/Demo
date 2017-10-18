
$(document).ready(function () {
    Dashboard.charts.init();
});

Dashboard.charts = {
    init: function () {
        $('input[type=radio][name=chart-options]').change(function () {
            if (this.id == 'allocation') {
                $('#allocation-chart').css('display', 'block');
                $('#variation-chart').css('display', 'none');
            }
            else if (this.id == 'variation') {
                $('#allocation-chart').css('display', 'none');
                $('#variation-chart').css('display', 'block');
            }
        });
        this.loadAssetsAllocationChart();
        this.loadAssetsVariationChart();
        this.loadVestingChart();
    },
    update: function (response) {
        this.loadVestingChart(response.Values);
    },
    loadVestingChart: function (values) {
        if (!values || (Array.isArray(values) && values.length === 0)) {
            $('#noDataValuesChart').css('display', 'block');
            return;
        }

        $('#noDataValuesChart').css('display', 'none');
        var dataArray = [];
        for (var i = 0; i < values.length; ++i) {
            dataArray.push([values[i].Period, values[i].Total, values[i].Invested, values[i].Vested]);
        }

        var openDate = new Date();
        g = new Dygraph(
            document.getElementById('valuesChart'), dataArray,
            {
                labels: ["Date", "Total Value", "Total Invested", "Total Vested"],
                colors: ['#0243b7', '#25C9A0', '#5ABCBB', '#00bdff', '#0be6d0'],
                //width: 400,
                height: 300,
                axisLabelFontSize: 10,
                strokeWidth: 2,
                axes: {
                    x: {
                        drawGrid: false,
                        axisLabelFormatter: function (x) {
                            //return x;
                            var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(x));
                            return date.getMonth() + 1 + "/" + date.getFullYear();
                        },
                        axisLineColor: 'white'
                    },
                    y: {
                        axisLabelFormatter: function (y) {
                            return '$' + Dashboard.getFormattedNumber(y);
                        },
                        axisLineColor: 'white'
                    },
                },
                gridLineColor: '#d9e0e6',
                legend: 'follow',
                legendFormatter: valuesChartLegendFormatter,
                digitsAfterDecimal: 2
            }
        );
    },
    loadAssetsAllocationChart: function () {
        var assets = pensionFundData.assets;
        var dataArray = [];
        for (var i = 0; i < assets.length; ++i) {
            dataArray.push([assets[i].name, assets[i].percentage]);
        }
        var chart = c3.generate({
            bindto: '#allocation-chart',
            data: {
                columns: dataArray,
                type: 'donut'
            },
            color: {
                pattern: ['#00bdff', '#25C9A0', '#5ABCBB', '#0243b7', '#0be6d0']
            }
        });
    },
    loadAssetsVariationChart: function () {
        var assetsReferenceValue = pensionFundData.assetsReferenceValue;
        var dataArray = [];
        for (var i = 0; i < assetsReferenceValue.length; ++i) {
            dataArray.push([assetsReferenceValue[i].period, assetsReferenceValue[i].value]);
        }
        var openDate = new Date();
        g = new Dygraph(
            document.getElementById('variation-chart'), dataArray,
            {
                labels: ["Year", "Value"],
                color: "#00bdff",
                width: 400,
                height: 300,
                axisLabelFontSize: 10,
                strokeWidth: 2,
                axes: {
                    x: {
                        drawGrid: false,
                        axisLabelFormatter: function (x) {
                            return x;
                            //var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(x));
                            //return date.getMonth() + 1 + "/" + date.getFullYear();
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
                legendFormatter: assetsVariationLegendFormatter,
                digitsAfterDecimal: 2
            }
        );

    }
};

function valuesChartLegendFormatter(data) {
    if (data.x == null) {
        // This happens when there's no selection and {legend: 'always'} is set.
        return '';
    }

    var openDate = new Date();
    var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(data.xHTML));
    var html = "<span class='legend-x-value'>" + (parseInt(date.getMonth()) + 1) + "/" + date.getFullYear() + "</span>";
    data.series.forEach(function (series) {
        if (!series.isVisible) return;
        var labeledData = series.labelHTML +': $' + series.yHTML;
        html += "<span class='legend-y-value'>" + '<br>' + labeledData + "</span>";
    });
    return html;
}

function assetsVariationLegendFormatter(data) {
    if (data.x == null) {
        // This happens when there's no selection and {legend: 'always'} is set.
        return '';
    }

    var openDate = new Date();
    var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(data.xHTML));
    var html = "<span class='legend-x-value'>" + (parseInt(date.getMonth()) + 1) + "/" + date.getFullYear() + "</span>";
    data.series.forEach(function (series) {
        if (!series.isVisible) return;
        var labeledData = '$' + series.yHTML;
        if (series.isHighlighted) {
            labeledData = '<b>' + labeledData + '</b>';
        }
        html += "<span class='legend-y-value'>" + '<br>' + labeledData + "</span>";
    });
    return html;
}