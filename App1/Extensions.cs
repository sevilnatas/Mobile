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

using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace MissionKeeper {

    public static class Extensions {

        public static IEnumerable<LatLng> Waypoints(this Flightpath flightPath, TimeSpan elapsedTime) {

            List<LatLng> rtnList = new List<LatLng>();
            foreach (var telemetry in flightPath.PreviousWaypoints(elapsedTime)) {
                LatLng tLatLng = telemetry.ToLatLng();
                rtnList.Add(tLatLng);
            }
            return rtnList;

        }

        public static LatLng ToLatLng( this Telemetry telemetry ) {
            LatLng rtnLatLng = new LatLng(telemetry.lat, telemetry.lng);                      
            return rtnLatLng;
        }

    }
}