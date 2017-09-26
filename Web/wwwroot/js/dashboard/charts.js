
$(document).ready(function () {
    var a = 2;
    dashboardCharts.loadAssetsAllocationChart();
});

var dashboardCharts = {

    loadAssetsAllocationChart: function(){
        var chart = c3.generate({
            bindto: '#assets-chart',
            data: {
                columns: [
                    ['data1', 30],
                    ['data2', 120],
                ],
                type : 'donut',
                onclick: function (d, i) { console.log("onclick", d, i); },
                onmouseover: function (d, i) { console.log("onmouseover", d, i); },
                onmouseout: function (d, i) { console.log("onmouseout", d, i); }
            },
            donut: {
                title: "Iris Petal Width"
            }
        });
    }
};