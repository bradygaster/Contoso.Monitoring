const labels = [
    'Celsius',
    'Fahrenheit'
];

class ChartViewModel {
    constructor(sensor) {
        this.sensorId = sensor;
        this.sampleSize = 10;
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
                        borderWidth: 1,
                        lineTension: 0.25,
                        pointRadius: 0
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
                        borderWidth: 1,
                        lineTension: 0.25,
                        pointRadius: 0
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
                animation: {
                    duration: 1,
                    easing: 'linear'
                },
                scales: {
                    y: {
                        min: 0,
                        max: 100,
                    }
                }
            },
        };

        this.config.data.datasets[0].data.length = this.sampleSize;
        this.config.data.datasets[1].data.length = this.sampleSize;
        this.config.data.datasets[0].data.fill(50);
        this.config.data.datasets[1].data.fill(50);

        window.setTimeout(() => {
            this.chart = new Chart(document.getElementById('canvas-' + this.sensorId).getContext('2d'), this.config);
        }, 300);
    }

    addDataPoint = (c, f) => {
        try {
            this.config.data.datasets[0].data.push(c);
            this.config.data.datasets[0].data.shift();
            this.config.data.datasets[1].data.push(f);
            this.config.data.datasets[1].data.shift();

            if(this.chart) {
                this.chart.update();
            }

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

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/monitor')
    .build();

window.onload = () => {
    connection.on('OnTemperatureReadingReceived', (reading) => {
        window.addChartValue(reading.sensorName, reading.celsius, reading.fahrenheit);
    });

    start();
}

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});