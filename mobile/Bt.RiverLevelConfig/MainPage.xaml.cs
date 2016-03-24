using Bt.RiverLevelConfig.Modul;
using Bt.RiverLevelConfig.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bt.RiverLevelConfig
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.txtId.Text = GetValue("DeviceId");

            this.txtGpsLat.Text = GetValue("txtGpsLat");
            this.txtGpsLong.Text = GetValue("txtGpsLong");

            this.txtMin.Text = GetValue("txtMin");
            this.txtMax.Text = GetValue("txtMax");
            this.txtAct.Text = GetValue("txtAct");

            base.OnNavigatedTo(e);
        }

        private string GetValue(string name)
        {
            if (SettingsContains(name))
            {
                object data;
                ApplicationData.Current.LocalSettings.Values.TryGetValue(name, out data);
                return data as string;
            }
            return string.Empty;
        }

        private void SetValue(string name, string value)
        {
            if (!SettingsContains(name))
            {
                ApplicationData.Current.LocalSettings.Values.Add(name, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[name] = value;
            }
        }

        private static bool SettingsContains(string name)
        {
            return ApplicationData.Current.LocalSettings.Values.Keys.Contains(name);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SetValue("DeviceId", this.txtId.Text);

            SetValue("txtGpsLat",this.txtGpsLat.Text);
            SetValue("txtGpsLong", this.txtGpsLong.Text);

            SetValue("txtMin", this.txtMin.Text);
            SetValue("txtMax", this.txtMax.Text);
            SetValue("txtAct", this.txtAct.Text);
            base.OnNavigatingFrom(e);
        }

        private void ScanQRCode_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ScanQr)); //, itemId);
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var service = new IoTHubService();

            double level = 2; // m/s
            Random rand = new Random();
           
            double levelRandom = level + rand.NextDouble() * 4 - 2;

            LevelMessage telemetryDataPoint = new LevelMessage
            {
                DeviceId = this.txtId.Text, // "MOBIL",

                GPSLat = this.txtGpsLat.Text,
                GPSLong = this.txtGpsLong.Text,
                ActualLevel = Convert.ToDouble(this.txtAct.Text),
                MinLevel = Convert.ToDouble(this.txtMin.Text),
                MaxLevel = Convert.ToDouble(this.txtMax.Text),
                Message = "Zbynek"
            };

            try
            {
                service.SendMessage(telemetryDataPoint);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            var dialog2 = new MessageDialog("Data odeslána :)");
            await dialog2.ShowAsync();

        }

      

        private async void GetGps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
            var position = await locator.GetGeopositionAsync();
            Geocoordinate Coordinate = position.Coordinate;

            this.txtGpsLong.Text = Coordinate.Longitude.ToString();
            this.txtGpsLat.Text = Coordinate.Latitude.ToString();
        }
    }
}
