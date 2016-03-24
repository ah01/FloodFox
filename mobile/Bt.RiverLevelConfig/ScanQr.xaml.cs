using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bt.RiverLevelConfig
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScanQr : Page
    {
        private MediaCapture _mediaCapture;
        private byte[] imageBuffer;
        public int exit = 0;
        private MessageDialog dialog;
        private ApplicationView currentView = ApplicationView.GetForCurrentView();


        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            exit = 1;
           // await _mediaCapture.StopPreviewAsync();
            _mediaCapture.Dispose();
            _mediaCapture = null;
            base.OnNavigatingFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
           
            // this.Frame.Navigate(typeof(MainPage), result);
        }

        //private void OnBackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    exit = 1;
        //    _mediaCapture.Dispose();
        //    if (this.Frame.CanGoBack)
        //    {
        //        e.Handled = true;
        //        this.Frame.GoBack();
        //    }
        //}

        // CTOR
        public ScanQr()
        {
            this.InitializeComponent();
        }

        private async Task<string> ScanQrCode()
        {
            try
            {
                await InitializeQrCode();


                var imgProp = new ImageEncodingProperties
                {
                    Subtype = "BMP",
                    Width = 380,
                    Height = 380
                };
                var bcReader = new BarcodeReader();


                while (exit == 0)
                {
                    var stream = new InMemoryRandomAccessStream();
                    await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);


                    stream.Seek(0);
                    var wbm = new WriteableBitmap(380, 380);
                    await wbm.SetSourceAsync(stream);
                    var result = bcReader.Decode(wbm);


                    if (result != null)
                    {
                        var torch = _mediaCapture.VideoDeviceController.TorchControl;
                        if (torch.Supported) torch.Enabled = false;
                        await _mediaCapture.StopPreviewAsync();
                        exit = 1;
                        return result.Text;

                      

                    }
                }
            }
            catch { }
            return null;
        }

        private async Task InitializeQrCode()
        {
            string error = null;
            try
            {
                //if (_mediaCapture == null)  
                //{  
                // Find all available webcams  
                DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


                // Get the proper webcam (default one)  
                DeviceInformation backWebcam = (from webcam in webcamList where webcam.IsEnabled select webcam).FirstOrDefault();


                // Initializing MediaCapture  


                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = backWebcam.Id,
                    AudioDeviceId = "",
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview
                });


                // Adjust camera rotation for Phone  
                _mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                _mediaCapture.SetRecordRotation(VideoRotation.Clockwise90Degrees);


                // Set the source of CaptureElement to MediaCapture  
                captureElement.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();


                // _mediaCapture.FocusChanged += _mediaCapture_FocusChanged; // ToDo:


                // Seetting Focus & Flash(if Needed)  


                var torch = _mediaCapture.VideoDeviceController.TorchControl;
                if (torch.Supported) torch.Enabled = true;


                await _mediaCapture.VideoDeviceController.FocusControl.UnlockAsync();
                var focusSettings = new FocusSettings();
                focusSettings.AutoFocusRange = AutoFocusRange.FullRange;
                focusSettings.Mode = FocusMode.Continuous;
                focusSettings.WaitForFocus = true;
                focusSettings.DisableDriverFallback = false;
                _mediaCapture.VideoDeviceController.FocusControl.Configure(focusSettings);
                await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();


                //}  
            }
            catch (Exception ex)
            {
                dialog = new MessageDialog("Error: " + ex.Message);
                dialog.ShowAsync();
            }
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var result = await ScanQrCode();
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("DeviceId"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("DeviceId", result);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["DeviceId"] = result;
            }
            
            this.Frame.GoBack();
        }
    }
}
