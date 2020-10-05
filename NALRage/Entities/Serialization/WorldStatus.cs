using Rage;
using System;

namespace NALRage.Entities.Serialization
{
    [Serializable]
    public struct WorldStatus
    {
        public WorldStatus(WeatherType weather, int hour, int minute)
        {
            CurrentWeather = weather;
            Hour = hour;
            Minute = minute;
        }

        public WeatherType CurrentWeather { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
