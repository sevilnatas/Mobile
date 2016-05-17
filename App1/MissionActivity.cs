using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using Android.Gms.Maps;
using Android.Gms.Maps.Model;


namespace MissionKeeper.Mobile.Droid
{
    [Activity(Label = "MissionActivity")]
    [IntentFilter( new string[] { "MissionKeeper.Mobile.Android.MissionActivity" }, Priority = (int) IntentFilterPriority.HighPriority )]
    public class MissionActivity : Activity
    {

        Mission moActiveMission;

        VideoMapFragment moVideoMapFragment;

        protected override void OnCreate(Bundle savedInstanceState) {

            base.OnCreate(savedInstanceState);

            _RegisterReciever();

            Guid missionID = Guid.Parse(Intent.GetStringExtra("missionID"));
            
            SetContentView(Resource.Layout.Mission);

            Button button = FindViewById<Button>(Resource.Id.StartVideo);
            moVideoMapFragment = this.FragmentManager.FindFragmentById<VideoMapFragment>(Resource.Id.aaaVideoMapFragment);
            moVideoMapFragment.FlightpathChanged += OnFlightpathChanged;

            GetMissionAndStreamAsync( missionID ).ContinueWith( getting => {

                this.RunOnUiThread(() => {
                    try {
                        this.Title = string.Format("Mission: {0} ({1}) ", moActiveMission.Name, moActiveMission.Location);                        
                    }
                    catch (Exception ex) {
                        throw;
                    }

                });


            });

            button.Click += delegate {
                moVideoMapFragment.StartMission();
            };

        }

        private void OnFlightpathChanged(System.Collections.Generic.IEnumerable<Android.Gms.Maps.Model.LatLng> path) {

            //PolylineOptions polyLineOpts = new PolylineOptions();

            //foreach (var wayPoint in path ) {
            //    polyLineOpts.Add(wayPoint);
            //}


    //        MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFragment);

            // var mapFrag = this.FragmentManager.FindFragmentById<fragment>( Resource.Id.mapFragment)

     //       mapFrag.Map.Clear();
     //       mapFrag.Map.AddPolyline(polyLineOpts);


            
        }

        private async Task GetMissionAndStreamAsync(Guid missionID) {

            Mobile.Pickle.ApiClient tClient = new Mobile.Pickle.ApiClient();

            moActiveMission = await tClient.GetMissionAsync(missionID);
            moVideoMapFragment.Mission = moActiveMission;
            moVideoMapFragment.MissionStream = moActiveMission.VideoStreams.First();


            moVideoMapFragment.FlightPath = await tClient.GetTelemetryAsync(moVideoMapFragment.Mission.ID, moVideoMapFragment.MissionStream.StreamID, TimeSpan.FromHours(24.0));

        }

        private void _RegisterReciever() {

            IntentFilter tFilter = new IntentFilter();
            tFilter.AddAction("");

        }
    }
}