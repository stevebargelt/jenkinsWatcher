using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using JenkinsWatcherUni;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Threading;
using Windows.Storage;
using Windows.Devices.Gpio;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace jenkinsWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // Use GPIO pin 5 to set values
        private const int SET_PIN = 7;
        private GpioPin setPin;
        private GpioPinValue currentValue = GpioPinValue.High;
        private DispatcherTimer timer;
        private CancellationTokenSource cts;
        private ApplicationData applicationData;
        private StorageFolder localFolder;
        private string url;
        private string apiToken;
        private string username;

        public MainPage()
        {
            this.InitializeComponent();
        }
    
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            cts = new CancellationTokenSource();
            applicationData = Windows.Storage.ApplicationData.Current;
            localFolder = applicationData.LocalFolder;
            ReadConfig();
            var jobs = GetAllStatuses();
            // Start toggling the pin value every 500ms.
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        async Task<string> readConfigFile()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile configFile;
            try
            {
                configFile = await packageFolder.GetFileAsync("secretConfig.json");
                return await Windows.Storage.FileIO.ReadTextAsync(configFile);
            }
            catch (Exception ex)
            {
                textBlock.Text = "Error: " + ex.Message;
            }
            return "There was an error";
        }

        private async void ReadConfig()
        {
            var configFile = await readConfigFile();

            secretConfig  secretConfigInfo = new secretConfig(configFile);

            Uri resourceAddress = new Uri(secretConfigInfo.jenkinsUrl);
            url = secretConfigInfo.jenkinsUrl;
            apiToken = secretConfigInfo.jenkinsApiKey;
            username = secretConfigInfo.jenkinsUsername;

            var jW = new JenkinsWatcherUni.jenkinsWatcher(url, apiToken, username);

            var JenkinsJobs = await jW.GetAllStatuses();
        }

        private async Task<List<JenkinsJob>> GetAllStatuses()
        {
            var jW = new JenkinsWatcherUni.jenkinsWatcher(url, apiToken, username);
            return await jW.GetAllStatuses();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ReadConfig();
            var jobs = GetAllStatuses();
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
        private void Timer_Tick(object sender, object e)
        {
            // Toggle the existing pin value

        }
    }
}
