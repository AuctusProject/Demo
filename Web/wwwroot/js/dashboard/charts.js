
$(document).ready(function () {
    var a = 2;
    dashboardCharts.loadAssetsAllocationChart();
});

var dashboardCharts = {

    loadAssetsAllocationChart: function (data) {
        var a = assetsReferenceValue;
        var chart = c3.generate({
            bindto: '#assets-chart',
            data: {
                columns: [
                    ['data1', 30],
                    ['data2', 120],
                ],
                type: 'donut'
            }
        });
    }
};