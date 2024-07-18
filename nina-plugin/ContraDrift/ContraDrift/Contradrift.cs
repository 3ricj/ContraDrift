using Accord.Imaging.Filters;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using NINA.Astrometry;
using NINA.Contradrift.Properties;
using NINA.Core.Locale;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Model;
using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using NINA.PlateSolving;
using NINA.PlateSolving.Interfaces;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using NINA.WPF.Base.Interfaces.ViewModel;
using NINA.WPF.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Settings = NINA.Contradrift.Properties.Settings;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;
using Accord.DataSets;
using System.Linq.Expressions;
using Accord.IO;
using NINA.Image.FileFormat.FITS;
using NINA.Image.FileFormat;
using System.IO;


namespace NINA.Contradrift {
    /// <summary>
    /// This class exports the IPluginManifest interface and will be used for the general plugin information and options
    /// The base class "PluginBase" will populate all the necessary Manifest Meta Data out of the AssemblyInfo attributes. Please fill these accoringly
    /// 
    /// An instance of this class will be created and set as datacontext on the plugin options tab in N.I.N.A. to be able to configure global plugin settings
    /// The user interface for the settings will be defined by a DataTemplate with the key having the naming convention "Contradrift_Options" where Contradrift corresponds to the AssemblyTitle - In this template example it is found in the Options.xaml
    /// </summary>
    [Export(typeof(IPluginManifest))]
    public class Contradrift : PluginBase, INotifyPropertyChanged {
        private readonly IPluginOptionsAccessor pluginSettings;
        private readonly IProfileService profileService;
        private readonly IImageSaveMediator imageSaveMediator;
        private readonly ITelescopeMediator telescopeMediator;
        private readonly IPlateSolverFactory plateSolverFactory;
        private readonly IPlateSolver plateSolver;
        private readonly IImageDataFactory ImageDataFactory;

        public ICommand ResetContraDriftDefaultsCommand { get; private set; }
        public ICommand BenchMarkCommand { get; private set; }

        private readonly IImageDataFactory imageDataFactory;
        protected IApplicationStatusMediator applicationStatusMediator;
        //public PlateSolvingStatusVM PlateSolveStatusVM { get; } = new PlateSolvingStatusVM();
        private CancellationTokenSource dummyCancellationSource = new CancellationTokenSource();
        private CancellationToken dummyCancellation = new CancellationToken();

        private Progress<ApplicationStatus> progress;

        public IImageData CroppedImageData;




        // Implementing a file pattern
        private readonly ImagePattern exampleImagePattern = new ImagePattern("$$EXAMPLEPATTERN$$", "An example of an image pattern implementation", "ContraDrift");

        [ImportingConstructor]
        [Obsolete]
        public Contradrift(IProfileService profileService, IOptionsVM options, IImageSaveMediator imageSaveMediator, ITelescopeMediator telescopeMediator, IPlateSolverFactory plateSolverFactory, IApplicationStatusMediator applicationStatusMediator, IImageDataFactory imageDataFactory) {

            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

            // This helper class can be used to store plugin settings that are dependent on the current profile
            this.pluginSettings = new PluginOptionsAccessor(profileService, Guid.Parse(this.Identifier));
            this.profileService = profileService;
            // React on a changed profile
            profileService.ProfileChanged += ProfileService_ProfileChanged;
            InitializeOptions();

            // Hook into image saving for adding FITS keywords or image file patterns
            this.imageSaveMediator = imageSaveMediator;

            // Run these handlers when an image is being saved
            this.imageSaveMediator.BeforeImageSaved += ImageSaveMediator_BeforeImageSaved;
            this.imageSaveMediator.BeforeFinalizeImageSaved += ImageSaveMediator_BeforeFinalizeImageSaved;
            this.imageSaveMediator.ImageSaved += ImageSaveMediator_ImageSaved;
                

            this.telescopeMediator = telescopeMediator;
            this.plateSolverFactory = plateSolverFactory;
            this.applicationStatusMediator = applicationStatusMediator;
            this.progress = new Progress<ApplicationStatus>(ProgressStatusUpdate);
            this.imageDataFactory = imageDataFactory;

            this.plateSolver = plateSolverFactory.GetPlateSolver(profileService.ActiveProfile.PlateSolveSettings);

            this.ResetContraDriftDefaultsCommand = new RelayCommand(ResetDefaults);
            this.BenchMarkCommand = new RelayCommand(StartBenchMark);

  

            // Register a new image file pattern for the Options > Imaging > File Patterns area
            //options.AddImagePattern(exampleImagePattern);
        }

        private async void ImageSaveMediator_ImageSaved(object sender, ImageSavedEventArgs e) {
            Logger.Debug("Filename:" + e.PathToImage.LocalPath);
            

            string newfilename = Path.Combine(
                Path.GetDirectoryName(e.PathToImage.LocalPath),
                "center", 
                Path.GetFileNameWithoutExtension(e.PathToImage.ToString()) + "_center" 
                );

            Logger.Debug("New Filename:" + newfilename);


            var FileSaveInfo = new FileSaveInfo {
                FilePath = newfilename,
                FileType = Core.Enum.FileTypeEnum.FITS,
                FilePattern = ""
            };

            if (!Directory.Exists(Path.GetDirectoryName(newfilename))) { Directory.CreateDirectory(Path.GetDirectoryName(newfilename)); }


            //IImageData rawData;
            //rawData = CroppedImageData;

            //CroppedImageData.MetaData = e.MetaData;
            //e.MetaData.GenericHeaders.ForEach
            foreach (var header in e.MetaData.GenericHeaders.ToList()) { 
                Logger.Debug(header.ToString());
                CroppedImageData.MetaData.GenericHeaders.Add(header);
            }

            //CroppedImageData.MetaData.GenericHeaders.Add(e.MetaData);

            //CroppedImageData.MetaData.GenericHeaders.Add(new DoubleMetaDataHeader(ContraDriftSolveDEC, ContraDriftSolveDECValue, string.Format("Contradrift platesolve DEC value in unknown units.")));



            Logger.Debug("Image loaded: " + CroppedImageData.Properties.Width + " x " + CroppedImageData.Properties.Height);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeOutSeconds));
            await CroppedImageData.SaveToDisk(FileSaveInfo, cts.Token);



            return;
            //            throw new NotImplementedException();
        }

        public override Task Teardown() {
            // Make sure to unregister an event when the object is no longer in use. Otherwise garbage collection will be prevented.
            profileService.ProfileChanged -= ProfileService_ProfileChanged;
            imageSaveMediator.BeforeImageSaved -= ImageSaveMediator_BeforeImageSaved;
            imageSaveMediator.BeforeFinalizeImageSaved -= ImageSaveMediator_BeforeFinalizeImageSaved;

            return base.Teardown();
        }
        private void ProgressStatusUpdate(ApplicationStatus status) {
            if (string.IsNullOrWhiteSpace(status.Source)) {
                status.Source = "ContraDrift";
            }
            applicationStatusMediator.StatusUpdate(status);
        }
        private void ProfileService_ProfileChanged(object sender, EventArgs e) {
            // Rase the event that this profile specific value has been changed due to the profile switch
            RaisePropertyChanged(nameof(ProfileSpecificNotificationMessage));
        }
        private async void StartBenchMark() {
            // Load reference fit into memory

            string path = "c:/temp/reference_image.fits";
            // 9576 x 6388

            IImageData rawData;

            try {
                

                //TestImage = (IImageData)ImageDataFactory.CreateFromFile("c:\\temp\\reference-image.fits", 16, false, 0, dummyCancellation);
                rawData = await imageDataFactory.CreateFromFile(path, 16, false, 0, dummyCancellation);
                Logger.Debug("Image loaded: " + rawData.Properties.Width + " x " + rawData.Properties.Height);
            } catch (Exception ex) { Logger.Error(ex);  return; }

            

            Logger.Debug("Loaded fits file");

            // 9576 x 6388

            PlateSolveParameter param = new PlateSolveParameter() {
                Binning = 1,
                Coordinates = telescopeMediator.GetCurrentPosition(),
                DownSampleFactor = 1,
                FocalLength = profileService.ActiveProfile.TelescopeSettings.FocalLength,
                MaxObjects = profileService.ActiveProfile.PlateSolveSettings.MaxObjects,
                PixelSize = profileService.ActiveProfile.CameraSettings.PixelSize,
                Regions = profileService.ActiveProfile.PlateSolveSettings.Regions,
                SearchRadius = profileService.ActiveProfile.PlateSolveSettings.SearchRadius,
                BlindFailoverEnabled = false
            };

            PlateSolveResult plateSolveResult;

            foreach (var i in Enumerable.Range(1, 10)) {
                int target_width = (int) rawData.Properties.Width / i;
                int target_height = (int) rawData.Properties.Height / i;

                Logger.Debug("Resizing to: " + target_width + " x " + target_height);
                var croppedImage = Crop(rawData, (rawData.Properties.Width - target_width) / 2, (rawData.Properties.Height - target_height) / 2, target_width, target_height);

                
                  using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeOutSeconds))) {
                    try {
                        plateSolveResult = await this.plateSolver.SolveAsync(croppedImage, param, progress, cts.Token);
                        if (plateSolveResult.Success) {

                            Logger.Error("PlateSolve Complete.  RA: " + plateSolveResult.Coordinates.RAString + " DEC: " + plateSolveResult.Coordinates.DecString);


                        } else {
                            Logger.Error("Platesolve Fail." + plateSolveResult);
                        }
                    } catch (OperationCanceledException) {
                        // Handle the timeout exception
                        Logger.Error("Platesolve did not return within" + TimeOutSeconds + " Seconds.");
                    }

                }
            }



        }


        private async Task ImageSaveMediator_BeforeImageSaved(object sender, BeforeImageSavedEventArgs e) {

            // contradrift

            PlateSolveParameter param = new PlateSolveParameter() {
                Binning = 1,
                Coordinates = telescopeMediator.GetCurrentPosition(),
                DownSampleFactor = 1,
                FocalLength = profileService.ActiveProfile.TelescopeSettings.FocalLength,
                MaxObjects = profileService.ActiveProfile.PlateSolveSettings.MaxObjects,
                PixelSize = profileService.ActiveProfile.CameraSettings.PixelSize,
                Regions = profileService.ActiveProfile.PlateSolveSettings.Regions,
                SearchRadius = profileService.ActiveProfile.PlateSolveSettings.SearchRadius,
                BlindFailoverEnabled = false
            };

            PlateSolveResult plateSolveResult;

            // 9576  x 6388
            //var TimeOutSeconds = 1;
            Logger.Debug("Start Crop down to " + CropSize + " square.");

            var croppedImage = Crop(e.Image, (e.Image.Properties.Width - CropSize) / 2, (e.Image.Properties.Height - CropSize) / 2, CropSize, CropSize);
            

            Logger.Debug("Start Solve with a timeout of " + TimeSpan.FromSeconds(TimeOutSeconds));

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeOutSeconds))) {
                try {
                    plateSolveResult = await this.plateSolver.SolveAsync(croppedImage, param, progress, cts.Token);
                    if (plateSolveResult.Success) {
                        string ContraDriftSolveRa = "SOLVERA";
                        double ContraDriftSolveRaValue = plateSolveResult.Coordinates.RA;

                        e.Image.MetaData.GenericHeaders.Add(new DoubleMetaDataHeader(ContraDriftSolveRa, ContraDriftSolveRaValue, string.Format("Contradrift platesolve RA HourAngle")));

                        string ContraDriftSolveDEC = "SOLVEDEC";
                        double ContraDriftSolveDECValue = plateSolveResult.Coordinates.Dec;


                        e.Image.MetaData.GenericHeaders.Add(new DoubleMetaDataHeader(ContraDriftSolveDEC, ContraDriftSolveDECValue, string.Format("Contradrift platesolve DEC value in unknown units.")));
                        //croppedImage.MetaData.GenericHeaders.Add(new DoubleMetaDataHeader(ContraDriftSolveDEC, ContraDriftSolveDECValue, string.Format("Contradrift platesolve DEC value in unknown units.")));

                        //this.telescopeMediator.SetCustomTrackingRate(SiderealShiftTrackingRate.Create(0.0, 0.0));
                    } else {
                        Logger.Error("Plate Solve Failed!!");
                    }
                } catch (OperationCanceledException) {
                    // Handle the timeout exception
                    Logger.Error("Platesolve did not return within" + TimeOutSeconds + " Seconds.");
                }
            }
            
            plateSolveResult = await this.plateSolver.SolveAsync(croppedImage, param, progress, dummyCancellationSource.Token);
            Logger.Debug("Complete headers and return");

            CroppedImageData = croppedImage;

            return;
        }

        public IImageData Crop(IImageData sourceImage, int x, int y, int width, int height) {
            // Get the image properties
            var properties = sourceImage.Properties;
            int sourceWidth = properties.Width;
            int sourceHeight = properties.Height;

            // Validate the crop dimensions
            if (x < 0 || y < 0 || width <= 0 || height <= 0 ||
                x + width > sourceWidth || y + height > sourceHeight) {
                throw new ArgumentException("Invalid crop dimensions.");
            }

            // Create a new array to store the cropped pixel data
            ushort[] croppedData = new ushort[width * height];

            // Copy the pixel data from the source image to the cropped array
            for (int i = 0; i < height; i++) {
                int sourceOffset = (y + i) * sourceWidth + x;
                int destinationOffset = i * width;
                Array.Copy(sourceImage.Data.FlatArray, sourceOffset, croppedData, destinationOffset, width);
            }

            // Create a new IImageData object with the cropped pixel data
            IImageData croppedImage = imageDataFactory.CreateBaseImageData(
                croppedData, width, height, properties.BitDepth, properties.IsBayered, sourceImage.MetaData);

            return croppedImage;
        }

        private Task ImageSaveMediator_BeforeFinalizeImageSaved(object sender, BeforeFinalizeImageSavedEventArgs e) {
            // Populate the example image pattern with data. This can provide data that may not be immediately available
            e.AddImagePattern(new ImagePattern(exampleImagePattern.Key, exampleImagePattern.Description, exampleImagePattern.Category) {
                Value = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss.ffffffK}"
            });

            return Task.CompletedTask;
        }

        public string DefaultNotificationMessage {
            get {
                return Settings.Default.DefaultNotificationMessage;
            }
            set {
                Settings.Default.DefaultNotificationMessage = value;
                CoreUtil.SaveSettings(Settings.Default);
                RaisePropertyChanged();
            }
        }

        public string ProfileSpecificNotificationMessage {
            get {
                return pluginSettings.GetValueString(nameof(ProfileSpecificNotificationMessage), string.Empty);
            }
            set {
                pluginSettings.SetValueString(nameof(ProfileSpecificNotificationMessage), value);
                RaisePropertyChanged();
            }
        }
        private void InitializeOptions() {
//            CropSize = pluginSettings.GetValueInt32("CropSize", 1024);
//            TimeOutSeconds = pluginSettings.GetValueSingle("TimeOutSeconds", (Single)1.0);

        }

        public void ResetDefaults() {
            CropSize = 1024;
            TimeOutSeconds = (Single)1.0;
            RaisePropertyChanged();

        }

        public int CropSize {
            get {
                return pluginSettings.GetValueInt32(nameof(CropSize), 1024);
            }
            set {
                pluginSettings.SetValueInt32(nameof(CropSize), value);
                RaisePropertyChanged();
            }
        }
        public Single TimeOutSeconds {
            get {
                return pluginSettings.GetValueSingle(nameof(TimeOutSeconds), (Single)1.0);
            }
            set {
                pluginSettings.SetValueSingle(nameof(TimeOutSeconds), value);
                RaisePropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
