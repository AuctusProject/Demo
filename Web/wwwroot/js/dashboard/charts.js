
$(document).ready(function () {
    dashboardCharts.init();
});

var dashboardCharts = {
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
            }
        });
    },
    loadAssetsVariationChart: function () {
        var assetsReferenceValue = pensionFundData.assetsReferenceValue;
        var dataArray = [];
        dataArray.push('Portifolio Variation');
        for (var i = 0; i < assetsReferenceValue.length; ++i) {
            dataArray.push(assetsReferenceValue[i].value);
        }
        var chart = c3.generate({
            bindto: '#variation-chart',
            data: {
                columns: [dataArray]
            },
            color: {
                pattern: ['#0243b7']
            }
        });
    }
};