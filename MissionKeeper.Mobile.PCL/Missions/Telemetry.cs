using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionKeeper
{
    public class Telemetry
    {

        public Guid MissionID { get; set; }

        public Guid StreamID { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public double lat { get; set; }

        public double lng { get; set; }

        public DateTime LocalTime { get; set; }

        public TimeSpan TimeSliceBegin { get; set; }

        public TimeSpan TimeSliceEnd { get; set; }

        public override string ToString() {
            return string.Format("LAT {0}: LNG {1}", lat, lng);
        }

    }

    public class StreamTelemetry : Telemetry { }

}
