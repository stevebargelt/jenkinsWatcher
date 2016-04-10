using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace JenkinsWatcherUni
{
    public class jenkinsWatcher
    {

        string url;
        string apiToken;
        string username;
        private HttpClient httpClient;
        private Uri resourceAddress;
        private CancellationTokenSource cts;

        public jenkinsWatcher(string url, string apiToken, string username)
        {
            this.url = url;
            this.apiToken = apiToken;
            this.username = username;
            cts = new CancellationTokenSource();
            SetupClient();
        }

        private void SetupClient()
        {
            resourceAddress = new Uri(url);
            var myFilter = new HttpBaseProtocolFilter();
            myFilter.AllowUI = false;
            myFilter.ServerCredential = new PasswordCredential(url, username, apiToken);
            httpClient = new HttpClient(myFilter);
        }

        public bool TryGetUri(string uriString, out Uri uri)
        {
            // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
            // since the user may provide URIs for servers located on the internet or intranet. If apps only
            // communicate with servers on the internet, only the "Internet (Client)" capability should be set.
            // Similarly if an app is only intended to communicate on the intranet, only the "Home and Work
            // Networking" capability should be set.
            if (!Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out uri))
            {
                return false;
            }

            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                return false;
            }

            return true;
        }

        private async Task<List<JenkinsJob>> GetJobs()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(resourceAddress).AsTask(cts.Token);
                JenkinsObject jenkinsInfo = new JenkinsObject(await response.Content.ReadAsStringAsync().AsTask(cts.Token));
                response.EnsureSuccessStatusCode();
                return jenkinsInfo.jobs; 
            }
            catch (TaskCanceledException)
            {
                //textBlock.Text = "Request Canceled";
                //TODO: Figure out error reporting
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException;
                //textBlock.Text = "Error: " + ex.Message;
                //TODO: Figure out error reporting
            }

            return null;
        }

        public async Task<List<JenkinsJob>> GetAllStatuses()
        {
            List<JenkinsJob> jobs = await  GetJobs();
            return jobs;
        }

        //public Job GetStatus(string JobName)
        //{
        //    var request = new RestRequest();
        //    request.RootElement = "jobs";
        //    IRestResponse response = client.Execute(request);
        //    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

        //    JenkinsObject jenkinsInfo = deserial.Deserialize<JenkinsObject>(response);
        //    List<Job> jobs = jenkinsInfo.jobs;


        //    List<Job> filteredJob =
        //            System.Linq.Enumerable.Where(jobs, j => j.name == JobName).ToList();

        //    if (filteredJob.Count() == 0)
        //    {
        //        filteredJob.Add(new Job { name = "not found", color = "not found", url = "not found" });
        //    }

        //    return filteredJob.First();

        //}

        //public List<Job> GetStatus(List<string> JobNames)
        //{
        //    var request = new RestRequest();
        //    request.RootElement = "jobs";
        //    IRestResponse response = client.Execute(request);
        //    RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();

        //    JenkinsObject jenkinsInfo = deserial.Deserialize<JenkinsObject>(response);
        //    List<Job> jobs = jenkinsInfo.jobs;

        //    List<Job> results = new List<Job> { };
        //    foreach (var j in JobNames)
        //    {
        //        results.Add(jobs.Find(jo => jo.name == j));

        //    }

        //    if (results.Count() == 0)
        //    {
        //        results.Add(new Job { name = "not found", color = "not found", url = "not found" });
        //    }

        //    return results;

        //}
    }

}
