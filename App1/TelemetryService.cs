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

namespace MissionKeeper.Mobile.Droid
{
    class TelemetryService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {

            Guid missionID = Guid.Parse(intent.GetStringExtra("missionID"));
            Guid streamID = Guid.Parse(intent.GetStringExtra("streamID"));
            TimeSpan elapsedTime = TimeSpan.FromSeconds( intent.GetDoubleExtra("elapsedTime", 0.0) );

            Pickle.ApiClient tClient = new Pickle.ApiClient();
            tClient.GetTelemetryAsync(missionID, streamID, elapsedTime).ContinueWith(getting => {

                System.IO.MemoryStream tStream = new System.IO.MemoryStream();
                var ser = new DataContractSerializer(  typeof( IEnumerable<Telemetry> ) );

                ser.WriteObject(tStream, getting.Result);

                var tIntent = new Intent("WhatTheFuck");
                tIntent.PutExtra("flightPath", tStream.ToArray());

                SendBroadcast(tIntent);

            });

        }
    }
}