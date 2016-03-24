//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Devices.Enumeration;
//using Windows.Graphics.Imaging;
//using Windows.Media.Capture;
//using Windows.Media.Devices;
//using Windows.Media.MediaProperties;
//using Windows.Storage;
//using Windows.Storage.Streams;
//using Windows.UI.Popups;
//using Windows.UI.ViewManagement;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Media.Imaging;
//using ZXing;


//namespace Bt.RiverLevelConfig
//{
//    public class QRHelper
//    {
//        private MediaCapture _mediaCapture;
//        private byte[] imageBuffer;
//        public int exit = 0;
//        private MessageDialog dialog;
//        private ApplicationView currentView = ApplicationView.GetForCurrentView();
//        private CaptureElement captureElement;

//        public QRHelper(CaptureElement captureElement)
//        {
//            this.captureElement = captureElement;
//        }

//        private async void ScanQrCode()
//        {
//            try
//            {
//                await InitializeQrCode();


//                var imgProp = new ImageEncodingProperties
//                {
//                    Subtype = "BMP",
//                    Width = 380,
//                    Height = 380
//                };
//                var bcReader = new BarcodeReader();


//                while (exit == 0)
//                {
//                    var stream = new InMemoryRandomAccessStream();
//                    await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);


//                    stream.Seek(0);
//                    var wbm = new WriteableBitmap(380, 380);
//                    await wbm.SetSourceAsync(stream);
//                    var result = bcReader.Decode(wbm);


//                    if (result != null)
//                    {
//                        var torch = _mediaCapture.VideoDeviceController.TorchControl;
//                        if (torch.Supported) torch.Enabled = false;
//                        await _mediaCapture.StopPreviewAsync();
//                        var msgbox = new MessageDialog(result.Text);
//                        await msgbox.ShowAsync();


//                        try
//                        {
//                            StorageFolder folder = ApplicationData.Current.LocalFolder;
//                            if (folder != null)
//                            {
//                                //Saving Scan Text  
//                                StorageFile sampleFile = await folder.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
//                                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, "Swift as a shadow");

//                                //Saving Scan Image  

//                                StorageFile file = await folder.CreateFileAsync("imagefile" + ".jpg", CreationCollisionOption.ReplaceExisting);
//                                using (var storageStream = await file.OpenAsync(FileAccessMode.ReadWrite))
//                                {
//                                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, storageStream);
//                                    var pixelStream = wbm.PixelBuffer.AsStream();
//                                    var pixels = new byte[pixelStream.Length];
//                                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);


//                                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wbm.PixelWidth, (uint)wbm.PixelHeight, 48, 48, pixels);
//                                    await encoder.FlushAsync();
//                                }

//                            }
//                            MyImage.Source = wbm;

//                            exit = 1;
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageDialog dialog = new MessageDialog("Error while initializing media capture device: " + ex.Message);
//                            dialog.ShowAsync();
//                            GC.Collect();
//                        }


//                        //  
//                    }
//                }
//            }
//            catch { }
//        }

//        private async Task InitializeQrCode()
//        {
//            string error = null;
//            try
//            {
//                //if (_mediaCapture == null)  
//                //{  
//                // Find all available webcams  
//                DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);


//                // Get the proper webcam (default one)  
//                DeviceInformation backWebcam = (from webcam in webcamList where webcam.IsEnabled select webcam).FirstOrDefault();


//                // Initializing MediaCapture  


//                _mediaCapture = new MediaCapture();
//                await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
//                {
//                    VideoDeviceId = backWebcam.Id,
//                    AudioDeviceId = "",
//                    StreamingCaptureMode = StreamingCaptureMode.Video,
//                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview
//                });


//                // Adjust camera rotation for Phone  
//                _mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
//                _mediaCapture.SetRecordRotation(VideoRotation.Clockwise90Degrees);


//                // Set the source of CaptureElement to MediaCapture  
//                captureElement.Source = _mediaCapture;
//                await _mediaCapture.StartPreviewAsync();


//                _mediaCapture.FocusChanged += _mediaCapture_FocusChanged;


//                // Seetting Focus & Flash(if Needed)  


//                var torch = _mediaCapture.VideoDeviceController.TorchControl;
//                if (torch.Supported) torch.Enabled = true;


//                await _mediaCapture.VideoDeviceController.FocusControl.UnlockAsync();
//                var focusSettings = new FocusSettings();
//                focusSettings.AutoFocusRange = AutoFocusRange.FullRange;
//                focusSettings.Mode = FocusMode.Continuous;
//                focusSettings.WaitForFocus = true;
//                focusSettings.DisableDriverFallback = false;
//                _mediaCapture.VideoDeviceController.FocusControl.Configure(focusSettings);
//                await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();


//                //}  
//            }
//            catch (Exception ex)
//            {
//                dialog = new MessageDialog("Error: " + ex.Message);
//                dialog.ShowAsync();
//            }
//        }

//    }
//}
