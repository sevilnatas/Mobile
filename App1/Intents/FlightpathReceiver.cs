using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Runtime.Serialization;

namespace MissionKeeper.Mobile.Droid {


    [BroadcastReceiver]
    [IntentFilter( new string[] { "flightPath" }, Priority = (int)  IntentFilterPriority.LowPriority )]
    class FlightpathReceiver : BroadcastReceiver {

        public Guid MissionID { get; set; }
        public Guid StreamID { get; set; }
        public Flightpath Flight { get; set; }

        public override void OnReceive(Context context, Intent intent) {

            try {

                MissionID = Guid.Parse( intent.GetStringExtra("missionID") );
                StreamID = Guid.Parse( intent.GetStringExtra("streamID") );

                System.IO.MemoryStream tStream = new System.IO.MemoryStream( intent.GetByteArrayExtra("flightPath") );
                var ser = new DataContractSerializer(typeof(IEnumerable<Telemetry>));
                var x = (IEnumerable<Telemetry>)ser.ReadObject(tStream);
               
            }
            catch {
                throw; 

            }


        }


    }

}