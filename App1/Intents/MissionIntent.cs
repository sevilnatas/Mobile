using System;
using Android.Content;

namespace MissionKeeper.Mobile.Droid
{
    public class MissionIntent : Intent
    {

        public MissionIntent(Context packageContext, Guid  missionID ) : base(packageContext, typeof(MissionActivity) ) {
            this.SetAction("MissionKeeper.Mobile.Android.MissionActivity");
            this.PutExtra("missionID", missionID.ToString());

        }

    }

}