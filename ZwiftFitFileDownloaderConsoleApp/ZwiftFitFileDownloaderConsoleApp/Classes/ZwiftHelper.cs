using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZwiftFitFileDownloaderConsoleApp.Classes
{
    internal class ZwiftHelper
    {
        internal List<Activity> GetActivitiesForUser(string accessToken, string userId, DateTime firstActivityDate)
        {
            List<Activity> activities = new List<Activity>();

            foreach (var timestamp in GetUnixTimestampsSinceFirst(firstActivityDate))
            {
                var response = GetActivitiesFromApi(accessToken, userId, timestamp);
                var responseArray = ConvertResponseToJsonArray(response.ToString());
                foreach (var activity in responseArray)
                {
                    Activity activityToAdd = JsonConvert.DeserializeObject<Activity>(activity.ToString());
                    if (!activities.Where(a => a.ActivityId.Equals(activityToAdd.ActivityId)).Any())
                    {
                        activities.Add(activityToAdd); 
                    }
                }
            }

            return activities;
        }

        internal string GetActivitiesFromApi(string accessToken, string userId, string timestamp)
        {
            HttpClient client = new HttpClient();
            var url = new Uri($"https://us-or-rly101.zwift.com/api/profiles/{userId}/activities?before={timestamp}");
            client.BaseAddress = url;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client.GetStringAsync(url).Result;
        }

        internal JArray ConvertResponseToJsonArray(string response)
        {
            return JArray.Parse(response);
        }

        internal List<string> GetUnixTimestampsSinceFirst(DateTime firstActivityDate)
        {
            var timestamps = new List<string>();
            var startingDate = firstActivityDate.AddMonths(1);
            while(startingDate <= DateTime.Now)
            {
                timestamps.Add(((DateTimeOffset)startingDate).ToUnixTimeMilliseconds().ToString());
                startingDate = startingDate.AddMonths(1);
            }
            timestamps.Add(((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString());
            return timestamps;
        }

        internal void DownloadActivityFitFile(string filePath, Activity activity)
        {
            HttpClient client = new HttpClient();
            var url = new Uri($"https://{activity.ActivityFitFileBucket}.s3.amazonaws.com/{activity.ActivityFitFileKey}");
            client.BaseAddress = url;
            client.DefaultRequestHeaders.Add("Accept", "application/octet-streams");
            var response = client.GetAsync(url).Result;
            using var memoryStream = response.Content.ReadAsStreamAsync().Result;
            using var fileStream = new FileStream(Path.Combine(filePath, activity.ActivityName+".fit"), FileMode.Create);
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(fileStream);
        }
    }
}
