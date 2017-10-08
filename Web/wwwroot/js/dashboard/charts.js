
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
        var a = 2;
    },
    loadVestingChart: function () {
        //var openDate = new Date();
        //g = new Dygraph(
        //    document.getElementById('valuesChart'), dataArray,
        //    {
        //        labels: ["Year", "Value"],
        //        color: "#00bdff",
        //        width: 400,
        //        height: 200,
        //        axisLabelFontSize: 10,
        //        strokeWidth: 2,
        //        axes: {
        //            x: {
        //                drawGrid: false,
        //                axisLabelFormatter: function (x) {
        //                    //return x;
        //                    var date = new Date(openDate.getFullYear(), openDate.getMonth() + parseInt(x));
        //                    return date.getMonth() + 1 + "/" + date.getFullYear();
        //                },
        //                axisLineColor: 'white'
        //            },
        //            y: {
        //                axisLabelFormatter: function (y) {
        //                    return '$' + y.toFixed(2);
        //                },
        //                axisLineColor: 'white'
        //            },
        //        },
        //        gridLineColor: '#d9e0e6',
        //        legend: 'follow',
        //        digitsAfterDecimal: 2
        //    }
        //);
        c3.generate({
            bindto: '#valuesChart',
            padding: {
                top: 30,
                right: 10,
                bottom: 0,
                left: 50,
            },
            data: {
                x: 'x',
                columns: [
                    ['x', '2010-01-01', '2011-01-01', '2012-01-01', '2013-01-01', '2014-01-01', '2015-01-01'],
                    ['TOTAL VALUE', 0, 100, 200, 300, 400, 500],
                    ['TOTAL VESTED', 0, 60, 130, 155, 240, 260],
                    ['TOTAL INVESTED', 0, 50, 100, 150, 200, 250]
                ]
            },
            color: {
                pattern: ['#0be6d0', '#00bdff', '#0243b7']
            },
            axis: {
                y: {
                    tick: {
                        count: 5,
                        format: function (d) { return "$ " + d; }
                    }
                },
                x: {
                    type: 'timeseries',
                    tick: {
                        format: function (x) { return x.getFullYear(); }
                        //format: '%Y' // format string is also available for timeseries data
                    }
                }
            },
            grid: {
                y: {
                    show: true
                },
            }
        });
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
                height: 200,
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
                digitsAfterDecimal: 2
            }
        );

    }
};