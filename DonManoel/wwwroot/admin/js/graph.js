
$(function () {
    'use strict';

    if (!isNullOrEmpty(GRAFICOPEDIDOSSEMANAL)) {
        let arrayData = [];
        let arrayLabel = [];
        $.each(GRAFICOPEDIDOSSEMANAL, function (idx, item) {
            arrayData.push(item.Quantidade);
            arrayLabel.push(item.Descricao);
        });

        var optionsSparkPedidos = {
            series: [{
                name: "Pedidos",
                data: arrayData,
            }],
            chart: {
                type: 'area',
                height: 60,
                width: 100,
                sparkline: {
                    enabled: true
                },
            },
            colors: ["#01b075"],
            stroke: {
                curve: 'smooth'
            },
            fill: {
                opacity: 0.3
            },
            xaxis: {
                categories: arrayLabel,
                labels: {
                    show: false,
                },
            },
            //yaxis: {
            //    min: 0
            //},
        };

        var chartPedidos = new ApexCharts(document.querySelector("#chart-pedidos"), optionsSparkPedidos);
        chartPedidos.render();

    }

    if (!isNullOrEmpty(GRAFICOPEDIDOSSEMANALGARCOM)) {
        let arrayData = [];
        let arrayLabel = [];
        $.each(GRAFICOPEDIDOSSEMANALGARCOM, function (idx, item) {
            arrayData.push(item.Quantidade);
            arrayLabel.push(item.Descricao);
        });

        var optionsSparkPedidos = {
            series: [{
                name: "Pedidos",
                data: arrayData,
            }],
            chart: {
                type: 'area',
                height: 60,
                width: 100,
                sparkline: {
                    enabled: true
                },
            },
            colors: ["#01b075"],
            stroke: {
                curve: 'smooth'
            },
            fill: {
                opacity: 0.3
            },
            xaxis: {
                categories: arrayLabel,
                labels: {
                    show: false,
                },
            },
            //yaxis: {
            //    min: 0
            //},
        };

        var chartPedidos = new ApexCharts(document.querySelector("#chart-pedidos-garcon"), optionsSparkPedidos);
        chartPedidos.render();

    }


    /*

    var optionsSpark2 = {
        series: [{
            name: "Sessions",
            data: [12, 14, 9, 27, 32, 15]
        }],
        chart: {
            type: 'area',
            height: 60,
            width: 100,
            sparkline: {
                enabled: true
            },
        },
        colors: ["#4c95dd"],
        stroke: {
            curve: 'smooth'
        },
        fill: {
            opacity: 0.3
        },
        yaxis: {
            min: 0
        },
    };

    var chartSpark2 = new ApexCharts(document.querySelector("#chart-spark2"), optionsSpark2);
    chartSpark2.render();
    */

    /*
    var options = {
        series: [{
            name: 'Elogio',
            data: [31, 40, 28, 51, 42, 109, 100]
        }, {
            name: 'Reclamacao',
            data: [11, 32, 45, 32, 34, 52, 41]
        }],
        chart: {
            width: 180,
            height: 120,
            type: 'area',
            zoom: {
                enabled: false
            },
        },
        dataLabels: {
            enabled: false
        },
        colors: ["#e66430", "#4c95dd"],
        stroke: {
            curve: 'smooth',
            width: 0.5,
        },
        grid: {
            show: false,
        },
        xaxis: {
            categories: ["Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sabado", "Domingo"],
            labels: {
                show: false,
            },
        },
        yaxis: {
            labels: {
                show: false,
            },
        },
        legend: {
            show: false,
        },
    };

    var chart = new ApexCharts(document.querySelector("#chart3"), options);
    chart.render();
    */
});