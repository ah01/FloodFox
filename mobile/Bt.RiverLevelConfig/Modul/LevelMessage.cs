using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Bt.RiverLevelConfig.Modul
{
    public class LevelMessage
    {
        public string DeviceId { get; set; }

        public string Message { get; set; }

        public double ActualLevel { get; set; }

        public string GPS { get; set; }

        public double MaxLevel { get; set; }

        public double MinLevel { get; set; }

        public string GPSLat { get; internal set; }

        public string GPSLong { get; internal set; }
    }
}
