const labels = [
    'Celsius',
    'Fahrenheit'
];

const dataTemplate = {
    labels: labels,
    datasets: [
        {
            label: 'Celsius',
            data: [12, 11, 12, 13, 14, 14, 13],
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
            data: [66, 66, 67, 70, 69, 70, 70],
            backgroundColor: [
                'rgba(255, 159, 64, 1)'
            ],
            borderColor: [
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 1
        }
    ]
};

const config = {
    type: 'line',
    data: dataTemplate,
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Sensor Reading'
            }
        }
    },
};

class ChartViewModel {
    constructor(sensor) {
        this.sensorId = sensor;
        this.data = [];
    }

    addDataPoint = (c, f) => {
        if (this.data.length == 100) {
            this.data.pop();
        }
        this.data.push({
            Celsius: c,
            Fahrenheit: f
        });
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

            try {
                var myChart = new Chart(document.getElementById('chart-' + sensorId), config);
            } catch (ex) {
                // console.log(ex);
                // worry about this later
            }
            
        }
        viewModel.addReadingToChart(sensorId, c, f);
    } catch (e) {
        console.log(e);
    }
}