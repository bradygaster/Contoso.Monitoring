using System;
using System.Globalization;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;

namespace Contoso.Monitoring.Sensors.Temperature
{
    public class FakeTemperatureSensorClient : ITemperatureSensorClient
    {
        private string _randomSensorName;
        private Random _rnd = new Random();
        private int _currentValue = 70;

        public FakeTemperatureSensorClient()
        {
            _randomSensorName = string.Concat(
                _rnd.Next(1, 4).ToString(CultureInfo.CurrentCulture),
                _rnd.Next(1, 11).ToString(CultureInfo.CurrentCulture).PadLeft(2, '0')
            );

            _currentValue = _rnd.Next(60, 80);
        }

        public Task<TemperatureReading> GetTemperatureReading()
        {
            _currentValue = _rnd.Next(60, 80);

            return Task.FromResult(new TemperatureReading
            {
                SensorName = _randomSensorName,
                Fahrenheit = _currentValue,
                Celsius = TemperatureReadingConverter.ToCelsius(_currentValue)
            });
        }
    }
}