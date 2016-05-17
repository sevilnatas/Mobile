using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionKeeper
{
    public class Flightpath : List<Telemetry> {
        public Flightpath() : base() { }

        public Flightpath(IEnumerable<Telemetry> telemetry) : base(telemetry) { }

        public bool HasTelemetry {
            get {
                return this.Count > 0;
            }
        }

        public Telemetry PositionAt(TimeSpan elapsedTime) {

            if (this.Count == 0  ) {
                throw new NoTelemetryException();
            }
            else if (elapsedTime < this[0].ElapsedTime ) {
                return this[0];
            }
            else if (elapsedTime > this[this.Count-1].ElapsedTime) {
                return this[this.Count-1];
            }

            List<Telemetry> rtnTelem = new List<Telemetry>();
            for (int i = 0; i < this.Count - 1; i++) {
                if (this[i].ElapsedTime <= elapsedTime && elapsedTime < this[i + 1].ElapsedTime) {
                    rtnTelem.Add(this[i]);
                }
            }

            try {
                return rtnTelem.Last();
            }
            catch {
                throw new NoTelemetryException();
            }
        }

        public IEnumerable<Telemetry> PreviousWaypoints(TimeSpan elapsedTime) {           
            try {
                return this.Where(t => t.ElapsedTime <= elapsedTime);
            }
            catch {
                throw new NoTelemetryException();
            }

        }

        public Telemetry PositionStart() {
            try {
                return this.First();
            }
            catch {
                throw new NoTelemetryException();
            }
        }

        public Telemetry PositionEnd() {
            try {
                return this.Last();
            }
            catch {
                throw new NoTelemetryException();
            }
        }

    }
}
