using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Configuration;

namespace jenkinsWatcher
{
    public class jenkinsWatcher
    {

        //HttpWebRequest httpWebRequest;
        RestClient client;
        string url;
        string apiToken;
        string username;

        public jenkinsWatcher(string url, string apiToken, string username)
        {
            this.url = url;
            this.apiToken = apiToken;
            this.username = username;
            SetupClient();
        }

        private void SetupClient()
        {
            client = new RestClient(url);
            byte[] credentialBuffer = new UTF8Encoding().GetBytes(username + ":" + apiToken);
            client.AddDefaultHeader("authorization", "Basic " + Convert.ToBase64String(credentialBuffer));

        }

        public List<Job> GetAllStatuses()
        {
            var request = new RestRequest();
            request.RootElement = "jobs";
            IRestResponse response = client.Execute(request);
            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

            JenkinsObject jenkinsInfo = deserial.Deserialize<JenkinsObject>(response);
            List<Job> jobs = jenkinsInfo.jobs;

            return jobs;

        }

        public Job GetStatus(string JobName)
        {
            var request = new RestRequest();
            request.RootElement = "jobs";
            IRestResponse response = client.Execute(request);
            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

            JenkinsObject jenkinsInfo = deserial.Deserialize<JenkinsObject>(response);
            List<Job> jobs = jenkinsInfo.jobs;


            List<Job> filteredJob =
                    System.Linq.Enumerable.Where(jobs, j => j.name == JobName).ToList() ;

            if (filteredJob.Count() == 0)
            {
                filteredJob.Add(new Job { name = "not found", color = "not found", url = "not found" });
            }

            return filteredJob.First();

        }

        public List<Job> GetStatus(List<string> JobNames)
        {
            var request = new RestRequest();
            request.RootElement = "jobs";
            IRestResponse response = client.Execute(request);
            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

            JenkinsObject jenkinsInfo = deserial.Deserialize<JenkinsObject>(response);
            List<Job> jobs = jenkinsInfo.jobs;

            List<Job> results = new List<Job> { };
            foreach (var j in JobNames)
            {
                results.Add(jobs.Find(jo => jo.name == j));

            }

            if (results.Count() == 0)
            {
                results.Add(new Job { name = "not found", color = "not found", url = "not found" });
            }

            return results;

        }
    }


}

