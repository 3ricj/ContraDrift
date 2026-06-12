using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using ASCOM.DriverAccess;
using NLog;
using NLog.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace ContraDrift
{
    public partial class Form1 : Form
    {
        private static Logger log;
        static ContraDrift.Properties.Settings settings = Properties.Settings.Default;
        BackgroundWorker worker = new BackgroundWorker();
        FileSystemWatcher watcher = new FileSystemWatcher();
        FitsManager fitsManager;
        private DataManager dataManager;
        private readonly GuidingPidController pidController = new GuidingPidController();

        public FrameList frames;
        public FrameList framesOld;

        private Telescope telescope;
        private bool FirstImage = true;

        Task<FitsManager.SolveResults> plateSolveTask = null;
        CancellationTokenSource solveCts = null;
        CancellationTokenSource sessionCts;

        private double PlateRaReference;
        private double PlateDecReference;
        private double PlateRaPrevious = -1;
        private double PlateDecPrevious = -1;

        double ScopeRa, ScopeDec;
        double ObsRa, ObsDec;

        private double dt_sec;

        private DateTime LastExposureCenter;
        private string PendingMessage;
        private DateTime ProcessingStartDateTime;

        private readonly System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        private string pendingFilePath = null;
        private readonly object pendingLock = new object();
        private int processingPumpRunning = 0;
        private string currentProcessingPath = null;

        public Form1()
        {
            InitializeComponent();
            ConfigureLogger();
            SetupCharts();
            RefreshFitsManagerConfig();

            Excel.Application xlApp = new Excel.Application();
            dataManager = new DataManager(dataGridView1, xlApp);

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            textBox1.Text = settings.TelescopeProgId;
            textBox2.Text = settings.WatchFolder;
            textBox3.Text = settings.UCAC4_path;
            AstapExePathTextBox.Text = settings.AstapExePath;
            VppwExePathTextBox.Text = settings.VppwExePath;
            EnableParentDirSolveCheckBox.Checked = settings.EnableParentDirSolve;

            PID_Setting_Kp_RA_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kp_RA_filter);
            PID_Setting_Ki_RA_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Ki_RA_filter);
            PID_Setting_Kd_RA_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kd_RA_filter);
            PID_Setting_Nfilt_RA.Text = String.Format("{0:0.000000}", settings.PID_Setting_Nfilt_RA);
            PID_Setting_Kp_DEC_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kp_DEC_filter);
            PID_Setting_Ki_DEC_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Ki_DEC_filter);
            PID_Setting_Kd_DEC_filter.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kd_DEC_filter);
            PID_Setting_Nfilt_DEC.Text = String.Format("{0:0.000000}", settings.PID_Setting_Nfilt_DEC);

            PID_Setting_Kp_RA.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kp_RA);
            PID_Setting_Ki_RA.Text = String.Format("{0:0.000000}", settings.PID_Setting_Ki_RA);
            PID_Setting_Kd_RA.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kd_RA);
            PID_Setting_Kp_DEC.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kp_DEC);
            PID_Setting_Ki_DEC.Text = String.Format("{0:0.000000}", settings.PID_Setting_Ki_DEC);
            PID_Setting_Kd_DEC.Text = String.Format("{0:0.000000}", settings.PID_Setting_Kd_DEC);

            RaRateLimitTextBox.Text = settings.RaRateLimitSetting.ToString();
            DecRateLimitTextBox.Text = settings.DecRateLimitSetting.ToString();
            BufferFitsCount.Text = settings.BufferFitsCount.ToString();

            if (settings.ProcessingTraditional) { ProcessingTraditional.Checked = true; } else { ProcessingFilter.Checked = true; }

            settings.Status = "Stopped";
        }

        private void RefreshFitsManagerConfig()
        {
            fitsManager = new FitsManager(new FitsManagerConfig
            {
                Ucac4Path = settings.UCAC4_path,
                AstapExePath = settings.AstapExePath,
                VppwExePath = settings.VppwExePath,
                EnableParentDirSolve = settings.EnableParentDirSolve
            });
        }

        private GuidingPidSettings BuildPidSettings()
        {
            return new GuidingPidSettings
            {
                KpRa = settings.PID_Setting_Kp_RA,
                KiRa = settings.PID_Setting_Ki_RA,
                KdRa = settings.PID_Setting_Kd_RA,
                KpDec = settings.PID_Setting_Kp_DEC,
                KiDec = settings.PID_Setting_Ki_DEC,
                KdDec = settings.PID_Setting_Kd_DEC,
                KpRaFilter = settings.PID_Setting_Kp_RA_filter,
                KiRaFilter = settings.PID_Setting_Ki_RA_filter,
                KdRaFilter = settings.PID_Setting_Kd_RA_filter,
                KpDecFilter = settings.PID_Setting_Kp_DEC_filter,
                KiDecFilter = settings.PID_Setting_Ki_DEC_filter,
                KdDecFilter = settings.PID_Setting_Kd_DEC_filter,
                NfiltRa = settings.PID_Setting_Nfilt_RA,
                NfiltDec = settings.PID_Setting_Nfilt_DEC,
                BufferFitsCount = settings.BufferFitsCount
            };
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sessionCts?.Cancel();

            if (dataManager.Count() > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to save the data before exiting?", "Save Data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string filePath = dataManager.GenerateFileName(".xlsx");
                    dataManager.SaveToExcel(filePath);
                    MessageBox.Show($"Data saved to {filePath}");
                    log.Info($"Data saved to {filePath}");
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            dataManager.Shutdown();
            log.Info("Shutting down..");
        }

        private void ConfigureLogger()
        {
            if (log == null) log = LogManager.GetCurrentClassLogger();

            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "${specialfolder:folder=MyDocuments}\\ContraDriftLog\\ContraDriftLog-${date:format=yyyy-MM-dd}-${processid}.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            RichTextBoxTarget rtbTarget = new RichTextBoxTarget();
            rtbTarget.AutoScroll = true;
            rtbTarget.Width = 2000;

            config.AddTarget("richTextBox1", rtbTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, rtbTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            log.Info("Starting up. Build: " + Convert.ToString(System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)));
            AddMessage("Build:" + Convert.ToString(System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)) + ",");
        }

        private void SelectTelescopeButton_Click(object sender, EventArgs e)
        {
            ASCOM.Utilities.Chooser selector = new ASCOM.Utilities.Chooser();
            selector.DeviceType = "Telescope";
            settings.TelescopeProgId = selector.Choose(settings.TelescopeProgId);
            settings.Save();
            textBox1.Text = settings.TelescopeProgId;
            log.Info("Setting Telescope to {@scope}", settings.TelescopeProgId);
        }

        private void WatchFolderBrowseButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = settings.WatchFolder;
            folderBrowserDialog1.ShowDialog();
            textBox2.Text = folderBrowserDialog1.SelectedPath;
            settings.WatchFolder = textBox2.Text;
            settings.Save();
            log.Info("Setting Watch folder to {@folder}", settings.WatchFolder);
        }

        private void save_settings()
        {
            try
            {
                settings.PID_Setting_Kp_RA_filter = float.Parse(PID_Setting_Kp_RA_filter.Text);
                settings.PID_Setting_Ki_RA_filter = float.Parse(PID_Setting_Ki_RA_filter.Text);
                settings.PID_Setting_Kd_RA_filter = float.Parse(PID_Setting_Kd_RA_filter.Text);
                settings.PID_Setting_Nfilt_RA = float.Parse(PID_Setting_Nfilt_RA.Text);
                settings.PID_Setting_Kp_DEC_filter = float.Parse(PID_Setting_Kp_DEC_filter.Text);
                settings.PID_Setting_Ki_DEC_filter = float.Parse(PID_Setting_Ki_DEC_filter.Text);
                settings.PID_Setting_Kd_DEC_filter = float.Parse(PID_Setting_Kd_DEC_filter.Text);
                settings.PID_Setting_Nfilt_DEC = float.Parse(PID_Setting_Nfilt_DEC.Text);
                settings.PID_Setting_Kp_RA = float.Parse(PID_Setting_Kp_RA.Text);
                settings.PID_Setting_Ki_RA = float.Parse(PID_Setting_Ki_RA.Text);
                settings.PID_Setting_Kd_RA = float.Parse(PID_Setting_Kd_RA.Text);
                settings.PID_Setting_Kp_DEC = float.Parse(PID_Setting_Kp_DEC.Text);
                settings.PID_Setting_Ki_DEC = float.Parse(PID_Setting_Ki_DEC.Text);
                settings.PID_Setting_Kd_DEC = float.Parse(PID_Setting_Kd_DEC.Text);
                settings.RaRateLimitSetting = float.Parse(RaRateLimitTextBox.Text);
                settings.DecRateLimitSetting = float.Parse(DecRateLimitTextBox.Text);
                settings.BufferFitsCount = int.Parse(BufferFitsCount.Text);
                settings.UCAC4_path = textBox3.Text;
                settings.AstapExePath = AstapExePathTextBox.Text;
                settings.VppwExePath = VppwExePathTextBox.Text;
                settings.EnableParentDirSolve = EnableParentDirSolveCheckBox.Checked;

                settings.Save();
                RefreshFitsManagerConfig();
            }
            catch { log.Debug("Problems with save settings"); }

            AddMessage("BufferFitsCount:" + BufferFitsCount.Text + ",");
            AddMessage("RaRateLimit:" + RaRateLimitTextBox.Text + ",");
            AddMessage("PID_Setting_Kp_RA:" + PID_Setting_Kp_RA.Text + ",");
            AddMessage("PID_Setting_Ki_RA:" + PID_Setting_Ki_RA.Text + ",");
            AddMessage("PID_Setting_Kd_RA:" + PID_Setting_Kd_RA.Text + ",");
            AddMessage("DecRateLimit:" + DecRateLimitTextBox.Text + ",");
            AddMessage("PID_Setting_Kp_DEC:" + PID_Setting_Kp_DEC.Text + ",");
            AddMessage("PID_Setting_Ki_DEC:" + PID_Setting_Ki_DEC.Text + ",");
            AddMessage("PID_Setting_Kd_DEC:" + PID_Setting_Kd_DEC.Text + ",");

            if (ProcessingTraditional.Checked) { settings.ProcessingTraditional = true; } else { settings.ProcessingTraditional = false; }
            log.Debug("Settings saved..");
        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
            if (settings.Status == "Running")
            {
                log.Info("Stopping");
                settings.Status = "Stopped";
                StartStopButton.Text = "Start";
                SelectTelescopeButton.Enabled = true;
                WatchFolderBrowseButton.Enabled = true;
                BufferFitsCount.Enabled = true;

                sessionCts?.Cancel();

                watcher.EnableRaisingEvents = false;
                watcher.Dispose();

                if (telescope != null)
                {
                    if (telescope.Tracking)
                    {
                        telescope.RightAscensionRate = 0;
                        telescope.DeclinationRate = 0;
                    }
                    telescope.Connected = false;
                    telescope.Dispose();
                    telescope = null;
                }

                ResetAll();
            }
            else
            {
                log.Info("Starting");
                try
                {
                    telescope = new Telescope(settings.TelescopeProgId);
                    telescope.Connected = true;
                    if (!telescope.Connected)
                    {
                        throw new InvalidOperationException("Mount did not connect.");
                    }
                    telescope.Tracking = true;
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Failed to connect to telescope");
                    MessageBox.Show("Failed to connect to telescope: " + ex.Message, "Start Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    telescope?.Dispose();
                    telescope = null;
                    settings.Status = "Stopped";
                    StartStopButton.Text = "Start";
                    return;
                }

                settings.Status = "Running";
                StartStopButton.Text = "Stop";
                SelectTelescopeButton.Enabled = false;
                WatchFolderBrowseButton.Enabled = false;
                BufferFitsCount.Enabled = false;

                framesOld = new FrameList(settings.BufferFitsCount);
                frames = new FrameList(settings.BufferFitsCount, framesOld);

                save_settings();
                ProcessingStartDateTime = DateTime.Now;
                sessionCts = new CancellationTokenSource();

                watcher = new FileSystemWatcher();
                watcher.Path = textBox2.Text;
                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;
                watcher.Created += new FileSystemEventHandler(ProcessNewFits);
                watcher.Renamed += new RenamedEventHandler(ProcessNewFits);
                watcher.Filter = "*.*";
                watcher.EnableRaisingEvents = true;
                log.Debug("Watcher Enabled: " + watcher.Path);
            }
        }

        void ProcessNewFits(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            if (Path.GetExtension(path) != ".fitscsv" && Path.GetExtension(path) != ".fits")
            {
                log.Trace("not a fits file, not processing: " + path);
                return;
            }

            lock (pendingLock)
            {
                if (path == currentProcessingPath)
                {
                    log.Trace("File already processing, skipping duplicate event: " + path);
                    return;
                }
                pendingFilePath = path;
            }

            TryStartProcessingPump();
        }

        private void TryStartProcessingPump()
        {
            if (Interlocked.CompareExchange(ref processingPumpRunning, 1, 0) != 0)
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        string path;
                        lock (pendingLock)
                        {
                            path = pendingFilePath;
                            pendingFilePath = null;
                            if (path == null)
                            {
                                break;
                            }
                            currentProcessingPath = path;
                        }

                        try
                        {
                            await ProcessSingleFileAsync(path, sessionCts?.Token ?? CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, "Unhandled error processing file {path}", path);
                        }
                        finally
                        {
                            lock (pendingLock)
                            {
                                if (currentProcessingPath == path)
                                {
                                    currentProcessingPath = null;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref processingPumpRunning, 0);
                    lock (pendingLock)
                    {
                        if (pendingFilePath != null)
                        {
                            TryStartProcessingPump();
                        }
                    }
                }
            });
        }

        private async Task ProcessSingleFileAsync(string inputFilename, CancellationToken sessionToken)
        {
            PendingMessage = "";

            double scopeRaRate, scopeDecRate;
            DateTime exposureCenter;
            FitsManager.SolveResults solveResult = null;

            log.Trace("Processing file: " + inputFilename);

            var newheaders = fitsManager.ReadFitsHeader(inputFilename);

            if (newheaders.ExposureTime < 1)
            {
                log.Error("Exposure too short, skipping it.");
                dataManager.AddRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = inputFilename,
                    Type = "SHORTFRAME",
                    ExpTime = newheaders.ExposureTime,
                    Filter = newheaders.Filter,
                    PendingMessage = PendingMessage
                });
                return;
            }

            if (plateSolveTask != null && !plateSolveTask.IsCompleted)
            {
                log.Warn("Previous plate solve is still running. Cancelling it.");
                if (solveCts != null)
                {
                    solveCts.Cancel();
                }
                try
                {
                    await plateSolveTask;
                }
                catch (TaskCanceledException)
                {
                    log.Warn("Previous plate solve task was canceled.");
                }
                catch (Exception ex)
                {
                    log.Warn(ex, "Previous plate solve task faulted during cancel");
                }
            }

            dataManager.AddRecord(new DataManager.DataRecord
            {
                Timestamp = newheaders.LocalTime,
                Filename = inputFilename,
                Type = "PROCESSING",
                ExpTime = newheaders.ExposureTime,
                Filter = newheaders.Filter,
                PendingMessage = PendingMessage
            });

            try
            {
                solveCts = CancellationTokenSource.CreateLinkedTokenSource(sessionToken);
                plateSolveTask = fitsManager.PlateSolveAllAsync(inputFilename, PlateRaPrevious, PlateDecPrevious, solveCts.Token);
                solveResult = await plateSolveTask;

                if (solveResult.Solved)
                {
                    log.Info($"Plate solve succeeded for file: {inputFilename}. RA: {solveResult.PlateRa}, DEC: {solveResult.PlateDec}");
                    PlateRaPrevious = solveResult.PlateRa;
                    PlateDecPrevious = solveResult.PlateDec;
                }
                else
                {
                    log.Warn($"Plate solve failed for file: {inputFilename}");
                }
            }
            catch (TaskCanceledException)
            {
                log.Warn($"Plate solve canceled for file: {inputFilename}");
                return;
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Plate solve faulted for file: {inputFilename}");
                dataManager.UpdateRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = inputFilename,
                    Type = "SOLVEFAIL",
                    Filter = newheaders.Filter,
                    ExpTime = newheaders.ExposureTime,
                    PendingMessage = PendingMessage
                });
                UpdateChartXY(0, 0, 0, 0, true);
                return;
            }

            if (solveResult == null || !solveResult.Solved)
            {
                dataManager.UpdateRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = inputFilename,
                    Type = "SOLVEFAIL",
                    Filter = newheaders.Filter,
                    ExpTime = newheaders.ExposureTime,
                    PendingMessage = PendingMessage
                });
                UpdateChartXY(0, 0, 0, 0, true);
                return;
            }

            double plateRaArcSec = solveResult.PlateRa * 15 * 3600;
            double plateDecArcSec = solveResult.PlateDec * 3600;
            double plateRaArcSecOld, plateDecArcSecOld;

            if (ObsRa != 0 && newheaders.ObjectRa != 0 && Math.Abs(newheaders.ObjectRa - ObsRa) > 0.001f
                && ObsDec != 0 && newheaders.ObjectDec != 0 && Math.Abs(newheaders.ObjectDec - ObsDec) > 0.001f)
            {
                log.Info("Position changed! Resetting All!");
                framesOld = new FrameList(settings.BufferFitsCount);
                frames = new FrameList(settings.BufferFitsCount, framesOld);
                pidController.Reset();
                FirstImage = true;
            }
            ObsRa = newheaders.ObjectRa;
            ObsDec = newheaders.ObjectDec;

            if (telescope == null)
            {
                log.Error("Telescope not connected; skipping mount update.");
                return;
            }

            ScopeRa = telescope.RightAscension;
            ScopeDec = telescope.Declination;
            scopeRaRate = telescope.RightAscensionRate / 15;
            scopeDecRate = telescope.DeclinationRate;

            log.Debug("ScopeRa: " + ScopeRa + ",ScopeDec: " + ScopeDec + ",ScopeRaRate: " + scopeRaRate + ",ScopeDecRate: " + scopeDecRate);

            frames.AddPlateCollection(plateRaArcSec, plateDecArcSec, newheaders.LocalTime, newheaders.ExposureTime);

            (plateRaArcSec, plateDecArcSec) = frames.GetPlateCollectionAverage();
            PlateRaReference = newheaders.ObjectRa * 3600;
            PlateDecReference = newheaders.ObjectDec * 3600;

            if (!framesOld.IsBufferFull())
            {
                log.Debug("Buffer not full.. Buffer size: " + (framesOld.Count() + frames.Count()) + " Fullsize: " + settings.BufferFitsCount * 2);

                if (frames.IsBufferFull() && FirstImage)
                {
                    PlateRaReference = newheaders.ObjectRa * 3600;
                    PlateDecReference = newheaders.ObjectDec * 3600;
                    exposureCenter = frames.GetPlateCollectionLocalExposureTimeCenter();
                    log.Debug("FirstImage:  ExposureCenter: " + exposureCenter + ", PlateRa: " + solveResult.PlateRa + " ,PlateDec: " + solveResult.PlateDec);
                    AddMessage("Reference image. RaRef:" + PlateRaReference + " DecRef: " + PlateDecReference);
                    FirstImage = false;
                    dataManager.UpdateRecord(new DataManager.DataRecord
                    {
                        Timestamp = newheaders.LocalTime,
                        Filename = inputFilename,
                        Type = "BUFFER-" + (framesOld.Count() + frames.Count()) + "-REF",
                        ExpTime = newheaders.ExposureTime,
                        Filter = newheaders.Filter,
                        PlateRa = solveResult.PlateRa,
                        PlateDec = solveResult.PlateDec,
                        PlateRaArcSecBuf = PlateRaReference,
                        PlateDecArcSecBuf = PlateDecReference,
                        FitsHeaderRa = ObsRa,
                        FitsHeaderDec = ObsDec,
                        PendingMessage = PendingMessage
                    });
                }
                else
                {
                    dataManager.UpdateRecord(new DataManager.DataRecord
                    {
                        Timestamp = newheaders.LocalTime,
                        Filename = inputFilename,
                        Type = "BUFFER-" + (framesOld.Count() + frames.Count()),
                        ExpTime = newheaders.ExposureTime,
                        Filter = newheaders.Filter,
                        PlateRa = solveResult.PlateRa,
                        PlateDec = solveResult.PlateDec,
                        FitsHeaderRa = ObsRa,
                        FitsHeaderDec = ObsDec,
                        PendingMessage = PendingMessage
                    });
                }
                return;
            }

            (plateRaArcSecOld, plateDecArcSecOld) = framesOld.GetPlateCollectionAverage();
            LastExposureCenter = framesOld.GetPlateCollectionLocalExposureTimeCenter();
            exposureCenter = frames.GetPlateCollectionLocalExposureTimeCenter();

            dt_sec = ((exposureCenter - LastExposureCenter).TotalMilliseconds) / 1000;
            log.Debug("ExposureCenter: " + exposureCenter + " LastExposureCenter: " + LastExposureCenter + " dt_sec: " + dt_sec);

            if (dt_sec < 0.1)
            {
                log.Warn("Invalid dt_sec {dt}; skipping PID update", dt_sec);
                dataManager.UpdateRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = inputFilename,
                    Type = "BADTIMING",
                    ExpTime = newheaders.ExposureTime,
                    Filter = newheaders.Filter,
                    DtSec = dt_sec,
                    PlateRa = solveResult.PlateRa,
                    PlateDec = solveResult.PlateDec,
                    PendingMessage = PendingMessage
                });
                return;
            }

            var pidResult = pidController.Compute(
                plateRaArcSec, plateDecArcSec,
                plateRaArcSecOld, plateDecArcSecOld,
                PlateRaReference, PlateDecReference,
                dt_sec, ProcessingFilter.Checked, BuildPidSettings());

            log.Debug("PID_RA: RaP=" + pidResult.RaP + ", RaI=" + pidResult.RaI + ", RaD=" + pidResult.RaD + ", new_RA_rate=" + pidResult.RaRate);
            log.Debug("PID_DEC: DecP=" + pidResult.DecP + ", DecI=" + pidResult.DecI + ", DecD=" + pidResult.DecD + ", new_DEC_rate=" + pidResult.DecRate);

            if (pidResult.NeedsPrecalc)
            {
                dataManager.UpdateRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = inputFilename,
                    Type = "PRECALC-" + pidResult.JournalCount,
                    ExpTime = newheaders.ExposureTime,
                    Filter = newheaders.Filter,
                    DtSec = dt_sec,
                    PlateRa = solveResult.PlateRa,
                    PlateRaArcSecBuf = plateRaArcSec,
                    RaP = pidResult.RaP * dt_sec,
                    RaI = pidResult.RaI,
                    PlateDec = solveResult.PlateDec,
                    PlateDecArcSecBuf = plateDecArcSec,
                    DecP = pidResult.DecP * dt_sec,
                    DecI = pidResult.DecI,
                    PendingMessage = PendingMessage,
                });
                log.Debug("Precalc:  Loading up the PropotionalJournal");
                return;
            }

            double newRaRate = pidResult.RaRate;
            double newDecRate = pidResult.DecRate;

            if (TamperRaRate.Checked)
            {
                log.Debug("TamperingRaRate Checked");
                AddMessage("TamperingRaRate Checked");
                if (dataGridView1.Rows.Count % 2 == 0) { newRaRate = +0.5; } else { newRaRate = -0.5; }
            }

            if (TamperDecRate.Checked)
            {
                log.Debug("TamperingDecRate Checked");
                AddMessage("TamperingDecRate Checked");
                if (dataGridView1.Rows.Count % 2 == 0) { newDecRate = +0.5; } else { newDecRate = -0.5; }
            }

            if (telescope.Tracking && telescope.CanSetRightAscensionRate && telescope.CanSetDeclinationRate)
            {
                if (newRaRate < float.Parse(RaRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Rate of " + newRaRate); newRaRate = float.Parse(RaRateLimitTextBox.Text) * -1; }
                if (newRaRate > float.Parse(RaRateLimitTextBox.Text)) { log.Debug("Refusing to set extreme Rate of " + newRaRate); newRaRate = float.Parse(RaRateLimitTextBox.Text); }
                telescope_RightAscensionRate(newRaRate / 15);

                log.Debug("Setting RightAscensionRate: " + newRaRate);
                if (newDecRate < float.Parse(DecRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Dec Rate of " + newDecRate); newDecRate = float.Parse(DecRateLimitTextBox.Text) * -1; }
                if (newDecRate > float.Parse(DecRateLimitTextBox.Text)) { log.Debug("Refusing to set extreme Dec Rate of " + newDecRate); newDecRate = float.Parse(DecRateLimitTextBox.Text); }
                telescope_DeclinationRate(newDecRate / 0.9972695677);

                log.Debug("Setting DeclinationRate: " + newDecRate);
            }
            else
            {
                log.Error("Telescope is not tracking!!! not setting tracking rates!!! Resetting everything.");
                FirstImage = true;
            }

            dataManager.UpdateRecord(new DataManager.DataRecord
            {
                Timestamp = newheaders.LocalTime,
                Filename = inputFilename,
                Type = "LIGHT",
                ExpTime = newheaders.ExposureTime,
                Filter = newheaders.Filter,
                DtSec = dt_sec,
                PlateRa = solveResult.PlateRa,
                PlateRaArcSecBuf = plateRaArcSec,
                RaP = pidResult.RaP * dt_sec,
                RaI = pidResult.RaI,
                RaD = pidResult.RaD,
                NewRaRate = newRaRate,
                PlateDec = solveResult.PlateDec,
                PlateDecArcSecBuf = plateDecArcSec,
                DecP = pidResult.DecP * dt_sec,
                DecI = pidResult.DecI,
                DecD = pidResult.DecD,
                NewDecRate = newDecRate,
                ScopeRa = ScopeRa,
                ScopeDec = ScopeDec,
                FitsHeaderRa = ObsRa,
                FitsHeaderDec = ObsDec,
                PendingMessage = PendingMessage,
                RateUpdateTimeStamp = DateTime.Now,
            });
            UpdateChartXY(pidResult.RaP * dt_sec, pidResult.RaI, pidResult.DecP * dt_sec, pidResult.DecI);
        }

        private void telescope_RightAscensionRate(double raRate)
        {
            Stopwatch.Restart();
            telescope.RightAscensionRate = raRate;
            if (Stopwatch.ElapsedMilliseconds > 100) { log.Error("Telescope.RightAscensionRate Latency: " + Stopwatch.ElapsedMilliseconds.ToString() + " ms"); }
            Stopwatch.Stop();
        }

        private void telescope_DeclinationRate(double decRate)
        {
            Stopwatch.Restart();
            telescope.DeclinationRate = decRate;
            if (Stopwatch.ElapsedMilliseconds > 100) { log.Error("Telescope.DeclinationRate Latency: " + Stopwatch.ElapsedMilliseconds.ToString() + " ms"); }
            Stopwatch.Stop();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            save_settings();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            textBox3.Text = folderBrowserDialog2.SelectedPath;
            settings.UCAC4_path = textBox3.Text;
            settings.Save();
            RefreshFitsManagerConfig();
            log.Info("Setting UCAC4 path to {@folder}", settings.UCAC4_path);
        }

        private void AstapExeBrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialogSolver.FileName = AstapExePathTextBox.Text;
            if (openFileDialogSolver.ShowDialog() == DialogResult.OK)
            {
                AstapExePathTextBox.Text = openFileDialogSolver.FileName;
            }
        }

        private void VppwExeBrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialogSolver.FileName = VppwExePathTextBox.Text;
            if (openFileDialogSolver.ShowDialog() == DialogResult.OK)
            {
                VppwExePathTextBox.Text = openFileDialogSolver.FileName;
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = DateTime.Now.ToString("yyyy-MM-dd HHMMss") + " - ContraDrift.csv";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            dataManager.SaveToCSV(sfd.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string filePath = dataManager.GenerateFileName(".xlsx");
            dataManager.SaveToExcel(filePath);
            System.Diagnostics.Process.Start(filePath);
        }

        private void AddMessage(string incomingMsg)
        {
            PendingMessage = PendingMessage + incomingMsg;
        }

        private void UpdateChartXY(double raP, double raI, double decP, double decI, bool isSolveFailed = false)
        {
            BeginInvoke(new System.Action(() =>
            {
                int rowIndex = dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling();

                double lastRaP = (ChartRa.Series[0].Points.Count > 0) ? ChartRa.Series[0].Points.Last().YValues[0] : 0;
                double lastDecP = (ChartDec.Series[0].Points.Count > 0) ? ChartDec.Series[0].Points.Last().YValues[0] : 0;

                double currentRaP = isSolveFailed ? lastRaP : raP;
                double currentDecP = isSolveFailed ? lastDecP : decP;

                var raPPoint = ChartRa.Series[0].Points.AddXY(rowIndex, currentRaP);
                if (isSolveFailed)
                {
                    ChartRa.Series[0].Points[raPPoint].Color = System.Drawing.Color.Red;
                }

                ChartRa.Series[1].Points.AddXY(rowIndex, raI);
                ChartRa.ChartAreas[0].RecalculateAxesScale();

                var decPPoint = ChartDec.Series[0].Points.AddXY(rowIndex, currentDecP);
                if (isSolveFailed)
                {
                    ChartDec.Series[0].Points[decPPoint].Color = System.Drawing.Color.Red;
                }

                ChartDec.Series[1].Points.AddXY(rowIndex, decI);
                ChartDec.ChartAreas[0].RecalculateAxesScale();
            }));
        }

        private void SetupCharts()
        {
            ChartRa.Legends.Clear();
            ChartRa.Legends.Add("Ra-Arcsec");
            ChartRa.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            ChartRa.Series[0].Name = "RaP";
            ChartRa.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            ChartRa.Series[0].BorderWidth = 3;
            ChartRa.Series[0].BorderColor = System.Drawing.Color.Red;

            ChartRa.Series.Add("RaI");
            ChartRa.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            ChartRa.Series[1].BorderWidth = 3;
            ChartRa.Series[1].BorderColor = System.Drawing.Color.Blue;

            ChartRa.ChartAreas[0].AxisY.IsStartedFromZero = false;
            ChartRa.ChartAreas[0].AxisX.IsStartedFromZero = false;

            ChartDec.Legends.Clear();
            ChartDec.Legends.Add("Dec-Arcsec");
            ChartDec.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            ChartDec.Series[0].Name = "DecP";
            ChartDec.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            ChartDec.Series[0].BorderWidth = 3;
            ChartDec.Series[0].BorderColor = System.Drawing.Color.Red;

            ChartDec.Series.Add("DecI");
            ChartDec.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            ChartDec.Series[1].BorderWidth = 3;
            ChartDec.Series[1].BorderColor = System.Drawing.Color.Blue;

            ChartDec.ChartAreas[0].AxisY.IsStartedFromZero = false;
            ChartDec.ChartAreas[0].AxisX.IsStartedFromZero = false;
        }

        private void ClearCharts()
        {
            ChartRa.Series[0].Points.Clear();
            ChartRa.Series[1].Points.Clear();
            ChartRa.ChartAreas[0].RecalculateAxesScale();

            ChartDec.Series[0].Points.Clear();
            ChartDec.Series[1].Points.Clear();
            ChartDec.ChartAreas[0].RecalculateAxesScale();
        }

        private void ResetAll()
        {
            dataManager.Reset();
            ClearCharts();

            framesOld = new FrameList(settings.BufferFitsCount);
            frames = new FrameList(settings.BufferFitsCount, framesOld);
            pidController.Reset();
            FirstImage = true;
            PlateRaPrevious = -1;
            PlateDecPrevious = -1;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetAll();
        }
    }
}
