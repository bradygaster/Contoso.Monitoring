using System;
using System.Diagnostics;
using System.Globalization;
using Contoso.Monitoring.Grains;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class FakeTemperatureSensorClient : ITemperatureSensorReceiveRequestObserver
    {
        // Min temperature, max temperature, min duration of a temperature swing in seconds, and max of that.
        private const double MinVal = 50;
        private const double MaxVal = 85;
        private const int MinPhaseDuration = 5;
        private const int MaxPhaseDuration = 30;

        private string _randomSensorName;
        private Random _rnd = new Random();
        private Stopwatch _totalTime = Stopwatch.StartNew();
        private double _currentValue;

        private double _phaseStartTemp;
        private double _phaseEndTemp;
        private double _phaseDurationSeconds;
        private readonly ILogger<FakeTemperatureSensorClient> _logger;

        public FakeTemperatureSensorClient(ILogger<FakeTemperatureSensorClient> logger)
        {
            _randomSensorName = string.Concat(
                _rnd.Next(1, 9).ToString(CultureInfo.CurrentCulture),
                "-",
                _rnd.Next(1, 100).ToString(CultureInfo.CurrentCulture).PadLeft(2, '0')
            );

            StartNewPhase();
            _logger = logger;
        }

        public Task<TemperatureSensor> GetTemperatureReading()
        {
            _logger.LogInformation($"Sensor {_randomSensorName} getting temperature.");
            UpdateTemperatureReading();
            _logger.LogInformation($"Sensor {_randomSensorName} returning temperature.");

            return Task.FromResult(new TemperatureSensor
            {
                SensorName = _randomSensorName,
                Fahrenheit = _currentValue,
                Celsius = TemperatureReadingConverter.ToCelsius(_currentValue)
            });
        }

        public void UpdateTemperatureReading()
        {
            double startRads;
            double endRads;
            double offset;
            if (_phaseStartTemp >= _phaseEndTemp)
            {
                // Cooling phase, that means moving from PI/2 to 3*PI/2
                startRads = Math.PI / 2;
                endRads = 3 * Math.PI / 2;
                offset = -1;
            }
            else
            {
                // Heading phase, that means moving from 3*PI/2 to 5*PI/2
                startRads = 3 * Math.PI / 2;
                endRads = 5 * Math.PI / 2;
                offset = 1;
            }
            
            var currentSeconds = _totalTime.Elapsed.TotalSeconds;
            var currentRads = startRads + (currentSeconds * Math.PI / _phaseDurationSeconds);

            _currentValue = _phaseStartTemp + ((offset + Math.Sin(currentRads)) / 2) * Math.Abs(_phaseStartTemp - _phaseEndTemp);

            if (currentRads >= endRads)
            {
                StartNewPhase();
            }
        }

        private void StartNewPhase()
        {
            _phaseStartTemp = _currentValue;
            _phaseEndTemp = MinVal + _rnd.NextDouble() * (MaxVal - MinVal);
            _phaseDurationSeconds = _rnd.Next(MinPhaseDuration, MaxPhaseDuration);
            _totalTime.Restart();
        }
    }
}