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
using MissionKeeper;

namespace MissionKeeper.Mobile.Droid
{
   

    public class TelemetryIntent : MissionIntent
    {
        public TelemetryIntent(Context packageContext, Guid missionID, Guid streamID, TimeSpan elapsedTime) : base(packageContext, missionID)
        {
            this.SetAction("MissionKeeper.MissionActivity");
            this.PutExtra("streamID", streamID.ToString());
            this.PutExtra("elapsedTime", elapsedTime.TotalSeconds);
        }

    }

}