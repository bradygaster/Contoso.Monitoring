using System;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;

namespace Contoso.Monitoring.Sensors.Temperature.Services
{
    public class FakeTemperatureSensorClient : ITemperatureSensorClient
    {
        private string _randomSensorName;
        private Random _rnd;

        public FakeTemperatureSensorClient()
        {
            _rnd = new Random();
            _randomSensorName = string.Concat(
                _rnd.Next(1, 4).ToString(), 
                _rnd.Next(1, 11).ToString().PadLeft(2, '0')
            );
        }

        public Task<TemperatureReading> GetTemperatureReading()
        {
            var fakeTempInF = _rnd.Next(60, 80); 

            return Task.FromResult(new TemperatureReading
            {
                SensorName = _randomSensorName,
                Fahrenheit = fakeTempInF,
                Celsius = TemperatureReadingConverter.ToCelsius(fakeTempInF)
            });
        }
    }
}