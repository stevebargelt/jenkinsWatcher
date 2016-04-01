﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using JenkinsWatcherUni;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Web.Http;
using System.Threading;
using System.Xml.Linq;
using Windows.Data.Json;
using Windows.Web.Http.Filters;
using Windows.Security.Credentials;
using Windows.Web.Http.Headers;
//using Microsoft.Azure.Management.KeyVault;
//using Microsoft.Azure.KeyVault.WebKey;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace jenkinsWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private HttpClient httpClient;
        private CancellationTokenSource cts;

        public MainPage()
        {
            this.InitializeComponent();
            //DoTheWork();
            //cts = new CancellationTokenSource();
        }
    
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //Helpers.CreateHttpClient(ref httpClient);
            cts = new CancellationTokenSource();
            DoTheWork();
        }

        private async void DoTheWork()
        {
            var uri = new System.Uri("ms-appx:///secretConfig.config");
            var sampleFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            var contents = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);

            Uri resourceAddress = new Uri("http://");
            var url = "http://jenkins/api/json";
            var apiToken = "";
            var username = "";

            var myFilter = new HttpBaseProtocolFilter();
            myFilter.AllowUI = false;
            myFilter.ServerCredential = new PasswordCredential(url, username, apiToken);
            httpClient = new HttpClient(myFilter);

            //var vaultcli = new Microsoft.Azure.Management.KeyVault.
            //keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken), GetHttpClient());

            if (!TryGetUri(url, out resourceAddress))
            {
                textBlock.Text = "Invalid URI.";
                return;
            }

            try
            {
                textBlock.Text = resourceAddress.ToString();
                HttpResponseMessage response = await httpClient.GetAsync(resourceAddress).AsTask(cts.Token);
 
                //await DisplayTextResultAsync(response, textBlock, cts.Token);

                JenkinsObject jenkinsInfo = new JenkinsObject(await response.Content.ReadAsStringAsync().AsTask(cts.Token));

                response.EnsureSuccessStatusCode();

            List<JenkinsJob> jobs = jenkinsInfo.jobs;
            foreach (JenkinsJob jo in jobs)
            {
                textBlock.Text += (jo.Name + " " + jo.Color);
            }
            }
            catch (TaskCanceledException)
            {
                textBlock.Text = "Request Canceled";
            }
            catch (Exception ex)
            {
                textBlock.Text = "Error: " + ex.Message;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DoTheWork();
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

        public async Task DisplayTextResultAsync(
            HttpResponseMessage response,
            TextBlock output,
            CancellationToken token)
        {
            string responseBodyAsText;
            output.Text += SerializeHeaders(response);
            responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(token);

            token.ThrowIfCancellationRequested();

            // Insert new lines.
            responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine);

            output.Text += responseBodyAsText;
        }

        public string SerializeHeaders(HttpResponseMessage response)
        {
            StringBuilder output = new StringBuilder();

            // We cast the StatusCode to an int so we display the numeric value (e.g., "200") rather than the
            // name of the enum (e.g., "OK") which would often be redundant with the ReasonPhrase.
            output.Append(((int)response.StatusCode) + " " + response.ReasonPhrase + "\r\n");

            SerializeHeaderCollection(response.Headers, output);
            SerializeHeaderCollection(response.Content.Headers, output);
            output.Append("\r\n");
            return output.ToString();
        }

        public void SerializeHeaderCollection(IEnumerable<KeyValuePair<string, string>> headers,
                                                                            StringBuilder output)
        {
            foreach (var header in headers)
            {
                output.Append(header.Key + ": " + header.Value + "\r\n");
            }
        }
    }
}