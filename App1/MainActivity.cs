using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Linq;
using System.Collections.Generic;

namespace MissionKeeper.Mobile.Droid
{


    [Activity(Label = "MissionKeeper", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        IEnumerable<Mission> moMissions;
        

    //    base.OnCreate(bundle);
    //        SetContentView(Resource.Layout.Main_list_view);
    //    // get list of books from xml file in your resource
    //    string[] books = Resources.GetStringArray(Resource.Array.books_array);
    //    ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item,  books);

    //       ListView.TextFilterEnabled = true;
    //       // add a listener to Item click
    //       ListView.ItemClick += delegate(object sender, ItemEventArgs args)
    //       {
    //           // When clicked, show a toast with the TextView text
    //            Toast.MakeText(Application," Selected Item: "+ ((TextView)   args.View).Text,ToastLength.Short).Show();
    //};



    protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Pickle.ApiClient tClient = new Pickle.ApiClient();
            var task = tClient.GetMissionsAsync().ContinueWith( getting => {

                moMissions = getting.Result;
                var MissionNames = moMissions.Select(m => m.Name);

                ListView listView = FindViewById<ListView>(Resource.Id.missionName);                
               // listView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.ActivityListItem, MissionNames.ToArray());

                foreach (var mission in moMissions)
                {
                    System.Diagnostics.Debug.WriteLine(mission.Name);
                }

            });


            Button buttonWaterTower = FindViewById<Button>(Resource.Id.WaterTower);
            buttonWaterTower.Click += delegate {
                if (moMissions != null) {
                    try {
                        MissionIntent missionIntent = new MissionIntent(this, moMissions.First().ID);
                        StartActivity(missionIntent);
                    }
                    catch { }
                }
            };

            Button buttonSwamp = FindViewById<Button>(Resource.Id.Swamp);
            buttonSwamp.Click += delegate {
                if (moMissions != null) {
                    try {
                        MissionIntent missionIntent = new MissionIntent(this, moMissions.ToArray()[1].ID);
                        StartActivity(missionIntent);
                    }
                    catch { }
                }
            };



        }




    }
}

