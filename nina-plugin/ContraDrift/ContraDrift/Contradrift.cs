using NINA.Contradrift.Properties;
using NINA.Core.Enum;
using NINA.Core.Utility;
using NINA.Image.FileFormat;
using NINA.Image.ImageData;
using NINA.Image.Interfaces;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.Interfaces.Mediator;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Settings = NINA.Contradrift.Properties.Settings;

namespace NINA.Contradrift {
    [Export(typeof(IPluginManifest))]
    public class Contradrift : PluginBase, INotifyPropertyChanged {
        private readonly IPluginOptionsAccessor pluginSettings;
        private readonly IProfileService profileService;
        private readonly IImageSaveMediator imageSaveMediator;
        private readonly IImageDataFactory imageDataFactory;

        public ICommand ResetContraDriftDefaultsCommand { get; private set; }
        public ICommand BrowseCropPathCommand { get; private set; }

        [ImportingConstructor]
        public Contradrift(
            IProfileService profileService,
            IImageSaveMediator imageSaveMediator,
            IImageDataFactory imageDataFactory) {

            if (Settings.Default.UpdateSettings) {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

            this.pluginSettings = new PluginOptionsAccessor(profileService, Guid.Parse(this.Identifier));
            this.profileService = profileService;
            this.imageSaveMediator = imageSaveMediator;
            this.imageDataFactory = imageDataFactory;

            profileService.ProfileChanged += ProfileService_ProfileChanged;
            imageSaveMediator.BeforeImageSaved += ImageSaveMediator_BeforeImageSaved;

            ResetContraDriftDefaultsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ResetDefaults);
            BrowseCropPathCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(BrowseCropPath);

            InitializeOptions();
        }

        public override Task Teardown() {
            profileService.ProfileChanged -= ProfileService_ProfileChanged;
            imageSaveMediator.BeforeImageSaved -= ImageSaveMediator_BeforeImageSaved;
            return base.Teardown();
        }

        private void ProfileService_ProfileChanged(object sender, EventArgs e) {
            RaisePropertyChanged(nameof(CropPath));
            RaisePropertyChanged(nameof(CropSize));
            RaisePropertyChanged(nameof(SaveTimeoutSeconds));
        }

        private async Task ImageSaveMediator_BeforeImageSaved(object sender, BeforeImageSavedEventArgs e) {
            try {
                var cropSize = CropSize;
                Logger.Info($"ContraDrift cropping to {cropSize}x{cropSize} square.");

                var patternTemplate = profileService.ActiveProfile.ImageFileSettings.GetFilePattern(e.Image.MetaData.Image.ImageType);
                var imageSavePath = profileService.ActiveProfile.ImageFileSettings.FilePath;
                var imagePatterns = e.Image.GetImagePatterns();
                var fullFrameFileName = imagePatterns.GetImageFileString(patternTemplate) + ".fits";

                var cropDirectory = ResolveCropDirectory(imageSavePath);
                var outputPath = Path.Combine(cropDirectory, Path.GetFileName(fullFrameFileName));

                Logger.Info($"ContraDrift crop output: {outputPath}");

                var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                var fileSaveInfo = new FileSaveInfo {
                    FilePath = tempPath,
                    FileType = FileTypeEnum.FITS,
                    FilePattern = string.Empty
                };

                Directory.CreateDirectory(cropDirectory);

                var x = (e.Image.Properties.Width - cropSize) / 2;
                var y = (e.Image.Properties.Height - cropSize) / 2;
                var croppedImage = Crop(e.Image, x, y, cropSize, cropSize);

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(SaveTimeoutSeconds))) {
                    await croppedImage.SaveToDisk(fileSaveInfo, cts.Token);
                }

                var tempFitsPath = tempPath + ".fits";
                if (File.Exists(outputPath)) {
                    File.Delete(outputPath);
                }
                File.Move(tempFitsPath, outputPath);

                Logger.Info($"ContraDrift saved cropped FITS: {outputPath}");
            } catch (Exception ex) {
                Logger.Error(ex, "ContraDrift failed to crop/save image; NINA save will continue.");
            }
        }

        private string ResolveCropDirectory(string imageSavePath) {
            if (!string.IsNullOrWhiteSpace(CropPath)) {
                return CropPath.Trim();
            }
            return Path.Combine(imageSavePath, "crop");
        }

        private IImageData Crop(IImageData sourceImage, int x, int y, int width, int height) {
            var properties = sourceImage.Properties;
            int sourceWidth = properties.Width;
            int sourceHeight = properties.Height;

            if (x < 0 || y < 0 || width <= 0 || height <= 0 ||
                x + width > sourceWidth || y + height > sourceHeight) {
                throw new ArgumentException(
                    $"Invalid crop dimensions: {width}x{height} at ({x},{y}) for source {sourceWidth}x{sourceHeight}.");
            }

            ushort[] croppedData = new ushort[width * height];

            for (int i = 0; i < height; i++) {
                int sourceOffset = (y + i) * sourceWidth + x;
                int destinationOffset = i * width;
                Array.Copy(sourceImage.Data.FlatArray, sourceOffset, croppedData, destinationOffset, width);
            }

            return imageDataFactory.CreateBaseImageData(
                croppedData, width, height, properties.BitDepth, properties.IsBayered, sourceImage.MetaData);
        }

        private void BrowseCropPath() {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "Select folder for cropped FITS output (leave blank in plugin to use NINA image folder/crop/)";
                if (!string.IsNullOrWhiteSpace(CropPath) && Directory.Exists(CropPath)) {
                    dialog.SelectedPath = CropPath;
                }
                if (dialog.ShowDialog() == DialogResult.OK) {
                    CropPath = dialog.SelectedPath;
                }
            }
        }

        private void InitializeOptions() {
            RaisePropertyChanged(nameof(CropPath));
            RaisePropertyChanged(nameof(CropSize));
            RaisePropertyChanged(nameof(SaveTimeoutSeconds));
        }

        public void ResetDefaults() {
            CropSize = 1600;
            SaveTimeoutSeconds = 30f;
            CropPath = string.Empty;
        }

        public int CropSize {
            get => pluginSettings.GetValueInt32(nameof(CropSize), 1600);
            set {
                pluginSettings.SetValueInt32(nameof(CropSize), value);
                RaisePropertyChanged();
            }
        }

        public string CropPath {
            get => pluginSettings.GetValueString(nameof(CropPath), string.Empty);
            set {
                pluginSettings.SetValueString(nameof(CropPath), value ?? string.Empty);
                RaisePropertyChanged();
            }
        }

        public float SaveTimeoutSeconds {
            get => pluginSettings.GetValueSingle(nameof(SaveTimeoutSeconds), 30f);
            set {
                pluginSettings.SetValueSingle(nameof(SaveTimeoutSeconds), value);
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
