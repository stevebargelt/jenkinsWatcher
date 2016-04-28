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
        private const int LIGHT_PIN = 4; // TODO: Put in config file
        private const double SERVICE_CHECK_INTERVAL = 60000; // TODO: Put this in config file
        private GpioPin pin;
        private GpioPinValue currentValue = GpioPinValue.Low;
        private DispatcherTimer timer;
        private CancellationTokenSource cts;
        private ApplicationData applicationData;
        private StorageFolder localFolder;
        private string url;
        private string apiToken;
        private string username;
        private JenkinsWatcherUni.jenkinsWatcher jW;

        public MainPage()
        {
            this.InitializeComponent();

            cts = new CancellationTokenSource();
            applicationData = Windows.Storage.ApplicationData.Current;
            localFolder = applicationData.LocalFolder;
            GetConfigValues();
            jW = new JenkinsWatcherUni.jenkinsWatcher(url, apiToken, username);
            InitGPIO();
            // Start checking Jenkins every minute.
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(SERVICE_CHECK_INTERVAL);
            timer.Tick += Timer_Tick;
        }
    
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            timer.Start();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            timer.Stop();
            base.OnNavigatingFrom(e);
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            pin = gpio.OpenPin(LIGHT_PIN);
            pin.Write(currentValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

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

        private async void GetConfigValues()
        {
            var configFile = await readConfigFile();
            secretConfig  secretConfigInfo = new secretConfig(configFile);
            Uri resourceAddress = new Uri(secretConfigInfo.jenkinsUrl);
            url = secretConfigInfo.jenkinsUrl;
            apiToken = secretConfigInfo.jenkinsApiKey;
            username = secretConfigInfo.jenkinsUsername;
        }

        private async Task<List<JenkinsJob>> GetAllStatuses()
        {
            return await jW.GetAllStatuses();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            GetConfigValues();
            var jobs = await GetAllStatuses();
            textBlock.Text = "";
            foreach (var j in jobs)
            {
                textBlock.Text += j.Color + " " + j.Name + "/n /n"; 
            }

        }
        private async void Timer_Tick(object sender, object e)
        {
            var jobs = await GetAllStatuses();
            bool anyFailing = false; 
            foreach (var job in jobs)
            {
                if (job.Color.ToLower() == "red")
                {
                    anyFailing = true;
                }
            }
            if (anyFailing)
            {
                currentValue = GpioPinValue.High;
            }
            else
            {
                currentValue = GpioPinValue.Low;
            }
            pin.Write(currentValue);
        }
    }
}
