using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MissionKeeper;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace MissionKeeper.Mobile.Pickle
{

    public abstract class ApiClientAbstract : HttpClient { }

    public class ApiClient: ApiClientAbstract
    {
        public async Task<IEnumerable<Mission>> GetMissionsAsync() {

            try
            {
                
                string missionsURL = $"http://missionkeeperapi.azurewebsites.net/api/missions";

                HttpResponseMessage responseMsg =   await GetAsync(missionsURL);
                var x = GetAsync(missionsURL);


                string resultStr = await responseMsg.Content.ReadAsStringAsync();
                IEnumerable<Mission> missions = JsonConvert.DeserializeObject<List<Mission>>(resultStr );

                return await x.Result;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"GetMissions Failed : { ex.Message}");                
                return new Mission[0] ;
            }
        }

    }
}
