const labels = [
    'Celsius',
    'Fahrenheit'
];

class ChartViewModel {
    constructor(sensor) {
        this.sensorId = sensor;
        this.config = {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Celsius',
                        data: [],
                        backgroundColor: [
                            'rgba(255, 99, 132, 1)'
                        ],
                        borderColor: [
                            'rgba(255, 99, 132, 1)'
                        ],
                        borderWidth: 1
                    },
                    {
                        label: 'Fahrenheit',
                        data: [],
                        backgroundColor: [
                            'rgba(255, 159, 64, 1)'
                        ],
                        borderColor: [
                            'rgba(255, 159, 64, 1)'
                        ],
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Sensor ' + sensor + ' Reading'
                    }
                },
                scales: {
                    y: {
                        min: 0,
                        max: 100,
                    }
                }
            },
        };

        document.getElementById('chart-' + this.sensorId).innerHTML = '<canvas id="canvas-' + this.sensorId + '"></canvas>';
        this.chart = new Chart(document.getElementById('canvas-' + this.sensorId), this.config);
    }

    addDataPoint = (c, f) => {
        try {

            if (this.config.data.datasets[0].data.length == 100) {
                this.config.data.datasets[0].data.pop();
            }
            this.config.data.datasets[0].data.push(c);

            if (this.config.data.datasets[1].data.length == 100) {
                this.config.data.datasets[1].data.pop();
            }
            this.config.data.datasets[1].data.push(f);

            this.chart.update();

            //console.log('Sensor ' + this.sensorId + ' has ' + this.config.data.datasets[0].data.length + ' c values and ' + this.config.data.datasets[1].data.length + ' f values.');
        } catch (e) {
            console.log(e);
        }
    }
}

class ViewModel {
    constructor() {
        this.Sensors = new Object();
    }

    chartExists = (s) => {
        try {
            if (s in this.Sensors) {
                return true;
            }
            return false;
        } catch (e) {
            console.log(e);
        }
    }

    addChart = (s) => {
        try {
            var newSensor = new ChartViewModel(s);
            this.Sensors[s] = newSensor;
        } catch (e) {
            console.log(e);
        }
    }

    addReadingToChart = (s, c, f) => {
        this.Sensors[s].addDataPoint(c, f);
    }
}

var viewModel = new ViewModel();

window.addChartValue = (sensorId, c, f) => {
    try {
        if (viewModel.chartExists(sensorId) == false) {
            viewModel.addChart(sensorId);
        }
        viewModel.addReadingToChart(sensorId, c, f);
    } catch (e) {
        console.log(e);
    }
}