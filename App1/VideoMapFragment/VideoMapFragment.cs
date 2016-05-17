using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using MissionKeeper;

/*   
 *  https://developer.xamarin.com/guides/android/platform_features/maps_and_location/maps/part_1_-_maps_application/
 */

namespace MissionKeeper {

    public class VideoMapFragment : Fragment, IOnMapReadyCallback {


        private Timer moMissionTimer = new Timer(6000);
        private Mission moActiveMission;
        private MissionStream moMissionStream;
        private Flightpath moFlightPath;
        private GoogleMap moMap;
        

        public event Action<IEnumerable<LatLng>> FlightpathChanged;

        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            _SetupTimer();

           View rtnView =  inflater.Inflate(Resource.Layout.MapVideoFragmentLayout, container, false);

            //MapFragment mapFrag = MapFragment.NewInstance();
            //FragmentTransaction tx = FragmentManager.BeginTransaction();
            //tx.Add(Resource.Id.mapFragContainer, mapFrag);
            //tx.Commit();

            MapFragment mapFrag = (MapFragment)  FragmentManager.FindFragmentById(Resource.Id.mapFragment);

            int status = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(this.Activity) ;
            if (status == ConnectionResult.Success) {
                mapFrag.GetMapAsync(this);
            }
            else {
               // Dialog d = GooglePlayServicesUtil.GetErrorDialog(status, this.Activity, 1234);
               // d.IsCancelable = false;
               // d.Show();

            }

            //if (null != mapFrag.Map) {
            //    moMap = mapFrag.Map;
            //}
            //else {
            //    mapFrag.GetMapAsync(this);
            //}

            
            return rtnView;

        }


        public Mission Mission {
            get {
                return moActiveMission;
            }

            set {
                moActiveMission = value;
            }
        }
       
        public MissionStream MissionStream {
            get {
                return moMissionStream;
            }
            set {
                moMissionStream = value;
                OnMissionStreamChanged(value);
            }
        }

        public Flightpath FlightPath {
            get {
                return moFlightPath;
            }

            set {
                moFlightPath = value;
                SetCameraBounds();
            }
        }


        public void StartMission() {
            var videoView = this.View.FindViewById<VideoView>(Resource.Id.SampleVideoView);
            videoView.Start();
            moMissionTimer.Start();

        }

        private void OnMissionStreamChanged(MissionStream missionStream) {

            this.Activity.RunOnUiThread(() => {
                var videoView = this.View.FindViewById<VideoView>(Resource.Id.SampleVideoView);

                var uri = Android.Net.Uri.Parse(missionStream.VideoURI);
                videoView.SetVideoURI(uri);
            });

        }

        public TimeSpan ElapsedTime {
            get {
                var videoView = this.View.FindViewById<VideoView>(Resource.Id.SampleVideoView);
                TimeSpan elapsedTime = TimeSpan.FromMilliseconds(videoView.CurrentPosition);
                return elapsedTime;
            }
            set {
                var videoView = this.View.FindViewById<VideoView>(Resource.Id.SampleVideoView);
                videoView.SeekTo(Convert.ToInt32(value.TotalMilliseconds));

                this.Activity.RunOnUiThread(() => {
                    OnMissionElapsedTimeChanged(value);
                });
                
            }
        }


        private void OnMissionElapsedTimeChanged(TimeSpan elapsedTime) {

            string tPoist = "N/A";
            Telemetry tTelem;

            if (moFlightPath != null && moFlightPath.HasTelemetry) {
                tTelem = moFlightPath.PositionAt(elapsedTime);
                tPoist = tTelem.ToString();

                TextView textView = this.View.FindViewById<TextView>(Resource.Id.VideoTextOverlay);
                textView.Text = string.Format("{0} {1} ", tPoist, tTelem.LocalTime.ToShortTimeString());


                if (null != FlightpathChanged) {
                    FlightpathChanged(moFlightPath.Waypoints(elapsedTime));
                }

                UpdateMap(elapsedTime);
            }

            

        }

        private void UpdateMap(TimeSpan elapsedTime) {
            if (moMap != null) {

                moMap.Clear();

                /* Set the icons associated with the flighpath  */
                SetStartIcon();
                SetMostRecentIcon(elapsedTime);

                DrawFlightPath(elapsedTime);

            }
        }

        private void DrawFlightPath(TimeSpan elapsedTime) {

            PolylineOptions polyLineOpts = new PolylineOptions();
            polyLineOpts.InvokeColor( moMissionStream.Color  );

            foreach (var wayPoint in moFlightPath.PreviousWaypoints(elapsedTime) ) {
                polyLineOpts.Add( wayPoint.ToLatLng() );
            }

            moMap.AddPolyline(polyLineOpts);
        }

        private void SetCameraBounds() {
            /* https://developers.google.com/maps/documentation/android-api/views#setting_boundaries */
            const int kCameraPaddingPixels = 10;

            if (moFlightPath != null && moFlightPath.HasTelemetry) {
                /*  There should be a more clever way to accomplish this withot having to look through the collection 4 times */

                double minLat = moFlightPath.Min(t => t.lat);
                double maxLat = moFlightPath.Max(t => t.lat);

                double minLng = moFlightPath.Min(t => t.lng);
                double maxLng = moFlightPath.Max(t => t.lng);

                LatLng southWest = new LatLng(minLat, minLng);
                LatLng northEast = new LatLng(maxLat, maxLng);

                LatLngBounds tBounds = new LatLngBounds(southWest, northEast);
                moMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(tBounds, kCameraPaddingPixels));
            }

        }

        private void SetStartIcon() {
            Telemetry tTelem = this.moFlightPath.PositionStart();
            SetIcon(tTelem, Resource.Drawable.FlagGreenIcon, "Start");
        }

        private void SetMostRecentIcon(TimeSpan elapsedTime) {
            Telemetry tTelem = this.moFlightPath.PositionAt(elapsedTime);
            SetIcon(tTelem, Resource.Drawable.DroneIcon , "Current");
        }


        private void SetIcon(Telemetry telemetry,  int iconID, string title ) {

            
            LatLng tLatLng = telemetry.ToLatLng();

            var bitMap = BitmapDescriptorFactory.FromResource(iconID);

            MarkerOptions markerStart = new MarkerOptions();
            markerStart.SetIcon(bitMap);
            markerStart.SetPosition(tLatLng);
            markerStart.SetTitle(title);
            moMap.AddMarker(markerStart);


        }


        private void _SetupTimer() {

            moMissionTimer.Elapsed += OnMissionTimerElapsed;

        }

        private void OnMissionTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) {

            this.Activity.RunOnUiThread(() => {
                OnMissionElapsedTimeChanged(ElapsedTime);
            });
        }

        public void OnMapReady(GoogleMap googleMap) {
            moMap = googleMap;
            moMap.MapType = GoogleMap.MapTypeHybrid;
        }
    }

}