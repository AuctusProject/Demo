
$(document).ready(function () {
    dashboardCharts.loadAssetsAllocationChart();
});

var dashboardCharts = {

    loadAssetsAllocationChart: function (data) {
        var assets = pensionFundData.assets;
        var dataArray = [];
        for (var i = 0; i < assets.length; ++i) {
            dataArray.push([assets[i].name, assets[i].percentage]);
        }
        var chart = c3.generate({
            bindto: '#assets-chart',
            data: {
                columns: dataArray,
                type: 'donut'
            }
        });
    }
};