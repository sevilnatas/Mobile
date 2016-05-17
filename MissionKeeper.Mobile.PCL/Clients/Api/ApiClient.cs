using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MissionKeeper;

namespace MissionKeeper.Mobile.Pickle {

    public abstract class ApiClientAbstract : HttpClient { }

    public class ApiClient : ApiClientAbstract {
        public async Task<IEnumerable<Mission>> GetMissionsAsync() {

            try {

                string missionsURL = $"http://missionkeeperapi.azurewebsites.net/api/missions";

                HttpResponseMessage responseMsg = await GetAsync(missionsURL);

                string resultStr = await responseMsg.Content.ReadAsStringAsync();
                IEnumerable<Mission> missions = JsonConvert.DeserializeObject<List<Mission>>(resultStr);

                return missions;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"GetMissions Failed : { ex.Message}");
                return new Mission[0];
            }
        }

        public async Task<Mission> GetMissionAsync(Guid missionID) {

            try {
                string missionsURL = string.Format("http://missionkeeperapi.azurewebsites.net/api/mission/{0}", missionID);
                HttpResponseMessage responseMsg = await GetAsync(missionsURL);

                string resultStr = await responseMsg.Content.ReadAsStringAsync();
                Mission mission = JsonConvert.DeserializeObject<Mission>(resultStr);

                return mission;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"GetMission Failed : { ex.Message}");
                throw;
            }

        }

        public async Task<MissionKeeper.Flightpath> GetTelemetryAsync(Guid missionId, Guid sessionID, TimeSpan elapsedTime) {

            try {
                string missionsURL = string.Format("http://missionkeeperapi.azurewebsites.net/api/mission/{0}/stream/{1}/flightpath/{2}/", missionId, sessionID, elapsedTime.TotalSeconds);
                HttpResponseMessage responseMsg = await GetAsync(missionsURL);
                Flightpath rtnPath = new Flightpath();                

                try {
                    // Create Json.Net formatter serializing DateTime using the ISO 8601 format
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                    serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());

                    string resultStr = await responseMsg.Content.ReadAsStringAsync();
                    var XXtelemetry = JsonConvert.DeserializeObject<List<object>>(resultStr);

                    foreach (dynamic tSpan in XXtelemetry) {                        

                        StreamTelemetry tTelem = new StreamTelemetry() {
                            MissionID = Guid.Parse(tSpan["MissionID"].Value),
                            StreamID = Guid.Parse( tSpan["StreamID"].Value),
                            lat = tSpan["lat"].Value,
                            lng = tSpan["lng"].Value,
                            LocalTime = tSpan["LocalTime"].Value,

                            ElapsedTime = ParseTimeSpan( tSpan["ElapsedTime"].Value ),
                            TimeSliceBegin = ParseTimeSpan(tSpan["TimeSliceBegin"].Value),
                            TimeSliceEnd = ParseTimeSpan(tSpan["TimeSliceEnd"].Value),

                        };

                        rtnPath.Add(tTelem);

                    }

                }
                catch (Exception ex) {
                    throw;
                }
                              
                return rtnPath;

            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"GetTelemetry Failed : { ex.Message}");
                throw;
            }


        }

        private TimeSpan ParseTimeSpan( string timeSpan ) {

            TimeSpan rtnSpan;

            string[] splits = new string[] { "PT", "H", "M", "S" };
            string[]  frags = timeSpan.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            if (frags.Length == 1) {
                rtnSpan = new TimeSpan(0, 0, int.Parse(frags[0]));
            }
            else if (frags.Length == 2) {
                rtnSpan = new TimeSpan(0, int.Parse(frags[0]), int.Parse(frags[1])  );
            }
            else if (frags.Length == 3) {
                rtnSpan = new TimeSpan(int.Parse(frags[0]), int.Parse(frags[1]), int.Parse(frags[2])  );
            }
            else {
                throw new Exception();
            }

            return rtnSpan;
        }

    }
}
