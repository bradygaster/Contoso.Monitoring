using System;
using System.Threading.Tasks;
using Contoso.Monitoring.Grains.Interfaces;

namespace Contoso.Monitoring.Sensors.Temperature.Services
{
    public class FakeTemperatureSensorClient : ITemperatureSensorClient
    {
        private string _randomSensorName;

        public FakeTemperatureSensorClient()
        {
            _randomSensorName = string.Concat(new Random().Next(1,10), new Random().Next(1,50));
        }

        public Task<TemperatureReading> GetTemperatureReading()
        {
            var fakeTempInF = new Random().Next(60, 80); 

            return Task.FromResult(new TemperatureReading
            {
                SensorName = _randomSensorName,
                Fahrenheit = fakeTempInF,
                Celsius = TemperatureReadingConverter.ToCelsius(fakeTempInF)
            });
        }
    }
}