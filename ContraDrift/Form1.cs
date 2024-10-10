using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASCOM.DriverAccess;
using System.IO;
using PinPoint;
using System.Runtime.InteropServices;
using NLog;
using NLog.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows.Forms.DataVisualization;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace ContraDrift
{
    public partial class Form1 : Form
    {
        //Logger log = LogManager.GetCurrentClassLogger();
        private static Logger log;
        static ContraDrift.Properties.Settings settings = Properties.Settings.Default;
        BackgroundWorker worker = new BackgroundWorker();
        FileSystemWatcher watcher = new FileSystemWatcher();
        TaskFactory tFactory = new TaskFactory();
        FitsManager fitsManager = new FitsManager();


        System.Data.DataTable datatable = new System.Data.DataTable();
        private DataManager dataManager;



        public FrameList frames;
        public FrameList framesOld;

        private Telescope telescope;
        private bool FirstImage = true;

        Task<FitsManager.SolveResults> plateSolveTask = null;
        CancellationTokenSource solveCts = null;

        private double PID_previous_3rd_propotional_RA = 0;
        private double PID_previous_propotional_RA = 0;
        // state variables for standard PID loop for RA
        private double PID_propotional_RA = 0;
        private double PID_integral_RA = 0;
        private double PID_derivative_RA = 0;

        // state variables for IIF filtered derivativePID loop for RA
        private double PID_error_new_RA = 0;
        private double PID_error_last_RA = 0;
        private double PID_error_third_RA = 0;
        private double PID_der0_RA = 0;
        private double PID_der1_RA = 0;

        // state variables for standard PID loop for DEC
        private double PID_previous_3rd_propotional_DEC = 0;
        private double PID_previous_propotional_DEC = 0;
        private double PID_propotional_DEC = 0;
        private double PID_integral_DEC = 0;
        private double PID_derivative_DEC = 0;

        // state variables for IIF filtered derivativePID loop for DEC
        private double PID_error_new_DEC = 0;
        private double PID_error_last_DEC = 0;
        private double PID_error_third_DEC = 0;
        private double PID_der0_DEC = 0;
        private double PID_der1_DEC = 0;

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
        LinkedList<double> PropotionalJournal_RA = new LinkedList<double>();
        LinkedList<double> PropotionalJournal_DEC = new LinkedList<double>();

        private System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();


        

        public Form1()
        {
            InitializeComponent();
            ConfigureLogger();
            //SetupDataGridView();
            SetupCharts();
            //SetupExcelWriter();

            Excel.Application xlApp = new Excel.Application();
            dataManager = new DataManager(dataGridView1, xlApp);
//            dataManager.SetupDataGridView();


            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);


            textBox1.Text = settings.TelescopeProgId;
            textBox2.Text = settings.WatchFolder;
            textBox3.Text = settings.UCAC4_path;

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prompt the user if they want to save data before closing (optional)
            if (dataManager.Count() > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to save the data before exiting?", "Save Data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Save to Excel
                    string filePath = dataManager.GenerateFileName(".xlsx");
                    dataManager.SaveToExcel(filePath);

                    MessageBox.Show($"Data saved to {filePath}");
                    log.Info($"Data saved to {filePath}");
                }
                else if (result == DialogResult.Cancel)
                {
                    // Cancel the close operation if the user clicks Cancel
                    e.Cancel = true;
                    return;
                }
            }
            // Safely shut down the DataManager (cleanup Excel resources)
            dataManager.Shutdown();
            log.Info("Shutting down..");
        }

        private void ConfigureLogger()
        {
            //log = LogManager.GetCurrentClassLogger();
            if (log == null) log = LogManager.GetCurrentClassLogger();


            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "${specialfolder:folder=MyDocuments}\\ContraDriftLog\\ContraDriftLog-${date:format=yyyy-MM-dd}-${processid}.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            //var logbox = new NLog.Targets

            // Rules for mapping loggers to targets            
            RichTextBoxTarget rtbTarget = new RichTextBoxTarget();
            rtbTarget.AutoScroll = true;
            rtbTarget.Width = 2000;
            //rtbTarget.AllowAccessoryFormCreation = false;

            config.AddTarget("richTextBox1", rtbTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, rtbTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;

            log.Info("Starting up. Build: " + Convert.ToString(System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            AddMessage("Build:" + Convert.ToString(System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location)) + ",");


        }

        private void SelectTelescopeButton_Click(object sender, EventArgs e)
        {
            //ASCOM.Utilities.Chooser chsr = new ASCOM.Utilities.Chooser();
            //chsr.DeviceType = "Telescope";
            //string scopeID = chsr.Choose();
            //            scope = new scopeID;

            ASCOM.Utilities.Chooser selector;
            selector = new ASCOM.Utilities.Chooser();
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

                settings.Save();
            }
            catch { log.Debug("Problems with save settings"); } // some garbage input we just toss if we can't parse it. 

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
                // stop
                log.Info("Stopping");
                settings.Status = "Stopped";
                StartStopButton.Text = "Start";
                SelectTelescopeButton.Enabled = true;
                WatchFolderBrowseButton.Enabled = true;
                BufferFitsCount.Enabled = true;

                watcher.EnableRaisingEvents = false;
                watcher.Dispose();

                if (telescope.Tracking) { 
                    telescope.RightAscensionRate = 0;
                    telescope.DeclinationRate = 0;
                }

                telescope.Connected = false;
                telescope.Dispose();
                ResetAll();
            }
            else
            {
                // start
                log.Info("Starting");
                settings.Status = "Running";
                StartStopButton.Text = "Stop";
                SelectTelescopeButton.Enabled = false;
                WatchFolderBrowseButton.Enabled = false;
                BufferFitsCount.Enabled = false;

                telescope = new Telescope(settings.TelescopeProgId);
                telescope.Connected = true;
                telescope.Tracking = true;

                framesOld = new FrameList(settings.BufferFitsCount);
                frames = new FrameList(settings.BufferFitsCount, framesOld);

                save_settings();

                ProcessingStartDateTime = DateTime.Now;

                //worker.DoWork += new DoWorkEventHandler(worker_backgroundProcess);
                //worker.RunWorkerAsync();
                watcher = new FileSystemWatcher();

                if (watcher.Path != textBox2.Text)
                {
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
                    //watcher.Changed += new FileSystemEventHandler(ProcessNewFits);
                    watcher.Filter = "*.*";


                }
                watcher.EnableRaisingEvents = true;
                log.Debug("Watcher Enabled: " + watcher.Path);

            }
        }

        void worker_backgroundProcess(object sender, DoWorkEventArgs e)
        {
            // for any background processing here... outside of new image processing. 
        }

        void ProcessNewFits(object sender, FileSystemEventArgs e)
        {
            _ = tFactory.StartNew(async () =>
            {



                double ScopeRaRate, ScopeDecRate;

                //Temp variables for filtered derivative RA PID
                double new_RA_rate = 0;
                double new_RA_rate_filtder = 0;
                double A0_RA, A1_RA, A2_RA;
                double A0d_RA, A1d_RA, A2d_RA, fder0_RA;
                double tau_RA, alpha_RA, Nfilt_RA;
                //Temp variables for filtered derivative DEC PID
                double new_DEC_rate = 0;
                double new_DEC_rate_filtder = 0;
                double A0_DEC, A1_DEC, A2_DEC;
                double A0d_DEC, A1d_DEC, A2d_DEC, fder0_DEC;
                double tau_DEC, alpha_DEC, Nfilt_DEC;
                DateTime ExposureCenter;

                string InputFilename = e.FullPath;
                log.Trace("New File: " + InputFilename);
                if (Path.GetExtension(InputFilename) != ".fitscsv" && Path.GetExtension(InputFilename) != ".fits") { log.Trace("not a fits file, not processing"); return; }

                //(bool Solved, double PlateRa, double PlateDec, DateTime PlateLocaltime, double PlateExposureTime, double Airmass, float Solvetime, double NewFitsRa, double NewFitsDec) = SolveFits(InputFilename, PlateRaPrevious, PlateDecPrevious);

                //fitsManager.PlateSolveAllAsync(InputFilename, PlateRaPrevious, PlateDecPrevious);
                var newheaders =  fitsManager.ReadFitsHeader(InputFilename);

                if (newheaders.ExposureTime < 1)
                {
                    log.Error("Exposure too short, skipping it.  ");
                    dataManager.AddRecord(new DataManager.DataRecord
                    {
                        Timestamp = newheaders.LocalTime,
                        Filename = InputFilename,
                        Type = "SHORTFRAME",
                        ExpTime = newheaders.ExposureTime,
                        Filter = newheaders.Filter
                    });
                    return;
                }

                if (plateSolveTask != null && !plateSolveTask.IsCompleted)
                {
                    log.Warn("Previous plate solve is still running. Cancelling it and moving on to the next file.");
                    solveCts.Cancel(); // Cancel the previous plate solve
                    try
                    {
                        await plateSolveTask; // Ensure proper cleanup of the task
                    }
                    catch (TaskCanceledException)
                    {
                        log.Warn("Previous plate solve task was canceled.");
                    }
                }


                // Set up new cancellation token for the next plate solve
                solveCts = new CancellationTokenSource();
//                solveCts.Cancel();
                // Start plate solving for the current file
                plateSolveTask = fitsManager.PlateSolveAllAsync(InputFilename, solveCts.Token);

                // Log the result of the completed solve task
                var solveResult = await plateSolveTask;

                if (solveResult.Solved)
                {
                    log.Info($"Plate solve succeeded for file: {InputFilename}. RA: {solveResult.PlateRa}, DEC: {solveResult.PlateDec}");
                }
                else
                {
                    log.Warn($"Plate solve failed for file: {InputFilename}");
                }

                double PlateRaArcSec = solveResult.PlateRa * 15 * 3600; // convert from hours to arcsec
                double PlateDecArcSec = solveResult.PlateDec * 3600; // convert from degrees to arcsec
                double PlateDecArcSecOld, PlateRaArcSecOld;
                
                if (ObsRa != 0 && newheaders.ObjectRa != 0 && Math.Abs(newheaders.ObjectRa - ObsRa) > 0.001f
                    && ObsDec != 0 && newheaders.ObjectDec != 0 && Math.Abs(newheaders.ObjectDec - ObsDec) > 0.001f)
                {
                    log.Info("Position changed! Resetting All!");
                    log.Debug("ObjRa: " + ObsRa + ",NewObsRa: " + newheaders.ObjectRa + ",Delta:" + Math.Abs(newheaders.ObjectRa - ObsRa) +
                        ",ObsDec: " + ObsDec + ",NewObsDec: " + newheaders.ObjectDec + ",Delta:" + Math.Abs(newheaders.ObjectDec - ObsDec));
                    framesOld = new FrameList(settings.BufferFitsCount);
                    frames = new FrameList(settings.BufferFitsCount, framesOld);
                    PropotionalJournal_RA = new LinkedList<double>();
                    PropotionalJournal_DEC = new LinkedList<double>();
                    FirstImage = true;
                }
                ObsRa = newheaders.ObjectRa;
                ObsDec = newheaders.ObjectDec;


                ScopeRa = telescope.RightAscension;
                ScopeDec = telescope.Declination;
                ScopeRaRate = telescope.RightAscensionRate / 15; //  arcsec to RA Sec per sidereal second divide by 15.  
                ScopeDecRate = telescope.DeclinationRate;

                log.Debug("ScopeRa: " + ScopeRa + ",ScopeDec: " + ScopeDec + ",ScopeRaRate: " + ScopeRaRate + ",ScopeDecRate: " + ScopeDecRate);


                if (!solveResult.Solved) {
                    log.Error("Platesolved failed! ");
                    dataManager.AddRecord(new DataManager.DataRecord
                    {
                        Timestamp = newheaders.LocalTime,
                        Filename = InputFilename,
                        Type = "SOLVEFAIL",
                        Filter = newheaders.Filter,
                        ExpTime = newheaders.ExposureTime
                    });
                    return; }


                frames.AddPlateCollection(PlateRaArcSec, PlateDecArcSec, newheaders.LocalTime, newheaders.ExposureTime);
           
                (PlateRaArcSec, PlateDecArcSec) = frames.GetPlateCollectionAverage();
                PlateRaReference = newheaders.ObjectRa  * 3600;
                PlateDecReference = newheaders.ObjectDec * 3600;


            if (!framesOld.IsBufferFull()) {
                    log.Debug("Buffer not full.. Buffer size: " + (framesOld.Count() + frames.Count()) + " Fullsize: " + settings.BufferFitsCount * 2);
                    //                    AddDataGridStruct(new DataGridElement { timestamp = DateTime.Now, filename = "test.fits", type = "test", dtsec = 5.222 });

                    if (frames.IsBufferFull() && FirstImage)
                    {
                        PlateRaReference = newheaders.ObjectRa * 3600;
                        PlateDecReference = newheaders.ObjectDec * 3600;
                        ExposureCenter = frames.GetPlateCollectionLocalExposureTimeCenter();
                        log.Debug("FirstImage:  ExposureCenter: " + ExposureCenter + ", PlateRa: " + solveResult.PlateRa + " ,PlateDec: " + solveResult.PlateDec + ",PlateLocaltime: " + newheaders.LocalTime + ",PlateExposureTime: " + newheaders.ExposureTime);
                        PID_propotional_RA = 0; PID_integral_RA = 0; PID_derivative_RA = 0; PID_previous_propotional_RA = 0;
                        AddMessage("Reference image. RaRef:" + PlateRaReference + " DecRef: " + PlateDecReference);
                        FirstImage = false;
                        dataManager.AddRecord(new DataManager.DataRecord
                        {
                            Timestamp = newheaders.LocalTime,
                            Filename = InputFilename,
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


                        dataManager.AddRecord(new DataManager.DataRecord
                        {
                            Timestamp = newheaders.LocalTime,
                            Filename = InputFilename,
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

                (PlateRaArcSecOld, PlateDecArcSecOld) = framesOld.GetPlateCollectionAverage();
                LastExposureCenter = framesOld.GetPlateCollectionLocalExposureTimeCenter();
                ExposureCenter = frames.GetPlateCollectionLocalExposureTimeCenter();

                dt_sec = ((ExposureCenter - LastExposureCenter).TotalMilliseconds) / 1000;

                log.Debug("ExposureCenter: " + ExposureCenter+ " LastExposureCenter: " + LastExposureCenter+ " dt_sec: " + dt_sec);


                // PID control for RA
                PID_propotional_RA = (PlateRaArcSec - PlateRaArcSecOld) / (dt_sec);
                PropotionalJournal_RA.AddLast(PID_propotional_RA);

                PID_integral_RA = (PlateRaArcSec - PlateRaReference);
                PID_derivative_RA = (PID_propotional_RA - PropotionalJournal_RA.First.Value) / (dt_sec);
                new_RA_rate = settings.PID_Setting_Kp_RA * PID_propotional_RA + settings.PID_Setting_Ki_RA * PID_integral_RA + settings.PID_Setting_Kd_RA * PID_derivative_RA;



                log.Debug("PID_RA_settings:  PID_Setting_Kp_RA: " + settings.PID_Setting_Kp_RA + ",PID_Setting_Ki_RA: " + settings.PID_Setting_Ki_RA + ",PID_Setting_Kd_RA: " + settings.PID_Setting_Kd_RA);
                log.Debug("PID_RA:  PID_previous_propotional_RA: " + PID_previous_propotional_RA + ",PID_propotional_RA: " + PID_propotional_RA + ",PID_integral_RA: " + PID_integral_RA + ",PID_derivative_RA: " + PID_derivative_RA + ",new_RA_rate: " + new_RA_rate);

                // standard PID control for DEC
                PID_propotional_DEC = (PlateDecArcSec - PlateDecArcSecOld) / (dt_sec);
                PropotionalJournal_DEC.AddLast(PID_propotional_DEC);
                PID_integral_DEC = (PlateDecArcSec - PlateDecReference);
                PID_derivative_DEC = (PID_propotional_DEC - PropotionalJournal_DEC.First.Value) / (dt_sec);
                new_DEC_rate = settings.PID_Setting_Kp_DEC * PID_propotional_DEC + settings.PID_Setting_Ki_DEC * PID_integral_DEC + settings.PID_Setting_Kd_DEC * PID_derivative_DEC;


                log.Debug("PID_DEC:  PID_previous_propotional_DEC: " + PID_previous_propotional_DEC + ",PID_propotional_DEC: " + PID_propotional_DEC + ",PID_integral_DEC: " + PID_integral_DEC + ",PID_derivative_DEC: " + PID_derivative_DEC + ",new_DEC_rate: " + new_DEC_rate);
                log.Debug("PID_DEC_settings:  PID_Setting_Kp_DEC: " + settings.PID_Setting_Kp_DEC + ",PID_Setting_Ki_DEC: " + settings.PID_Setting_Ki_DEC + ",PID_Setting_Kd_DEC: " + settings.PID_Setting_Kd_DEC);

                if (PropotionalJournal_DEC.Count <= settings.BufferFitsCount) {
                    dataManager.AddRecord(new DataManager.DataRecord
                    {
                        Timestamp = newheaders.LocalTime,
                        Filename = InputFilename,
                        Type = "PRECALC-" + PropotionalJournal_DEC.Count,
                        ExpTime = newheaders.ExposureTime,
                        Filter = newheaders.Filter,
                        DtSec = dt_sec,
                        PlateRa = solveResult.PlateRa,
                        PlateRaArcSecBuf = PlateRaArcSec,
                        RaP = PID_propotional_RA * dt_sec,
                        RaI = PID_integral_RA,
                        PlateDec = solveResult.PlateDec,
                        PlateDecArcSecBuf = PlateDecArcSec,
                        DecP = PID_propotional_DEC * dt_sec,
                        DecI = PID_integral_DEC,
                        PendingMessage = PendingMessage,
                    });
                    log.Debug("Precalc:  Loading up the PropotionalJournal");
                    return;
                }
                if (PropotionalJournal_RA.Count > settings.BufferFitsCount) { PropotionalJournal_RA.RemoveFirst(); }
                if (PropotionalJournal_DEC.Count > settings.BufferFitsCount) { PropotionalJournal_DEC.RemoveFirst(); }


                if (ProcessingFilter.Checked)
                {
                    new_DEC_rate = new_DEC_rate_filtder;
                    new_RA_rate = new_RA_rate_filtder;
                    log.Debug("Processing Filter mode enabled, override new_RA_rate to: " + new_RA_rate + " ,new_DEC_rate: " + new_DEC_rate);

                }

                if (TamperRaRate.Checked)
                {
                    log.Debug("TamperingRaRate Checked");
                    AddMessage("TamperingRaRate Checked");
                    if (dataGridView1.Rows.Count % 2 == 0) { new_RA_rate = +0.5; } else { new_RA_rate = -0.5; }
                }

                if (TamperDecRate.Checked)
                {
                    log.Debug("TamperingDecRate Checked");
                    AddMessage("TamperingDecRate Checked");

                    if (dataGridView1.Rows.Count % 2 == 0) { new_DEC_rate = +0.5; } else { new_DEC_rate = -0.5; }
                }



                // TODO: Check that the mount is tracking, and if telescope.RARateIsSettable is true. 
                if (telescope.Tracking && telescope.CanSetRightAscensionRate && telescope.CanSetDeclinationRate)
                {
                    if (new_RA_rate < float.Parse(RaRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Rate of " + new_RA_rate); new_RA_rate = float.Parse(RaRateLimitTextBox.Text) * -1; }
                    if (new_RA_rate > float.Parse(RaRateLimitTextBox.Text)) { log.Debug("Refusing to set extreme Rate of " + new_RA_rate); new_RA_rate = float.Parse(RaRateLimitTextBox.Text); }
                    telescope_RightAscensionRate(new_RA_rate / 15);

                    log.Debug("Setting RightAscensionRate: " + new_RA_rate);
                    if (new_DEC_rate < float.Parse(DecRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Dec Rate of " + new_DEC_rate); new_DEC_rate = float.Parse(DecRateLimitTextBox.Text) * -1; }
                    if (new_DEC_rate > float.Parse(DecRateLimitTextBox.Text)) { log.Debug("Refusing to set extreme Dec Rate of " + new_DEC_rate); new_DEC_rate = float.Parse(DecRateLimitTextBox.Text); }
                    telescope_DeclinationRate(new_DEC_rate / 0.9972695677);

                    log.Debug("Setting DeclinationRate: " + new_DEC_rate);
                }
                else
                {
                    log.Error("Telescope is not tracking!!! not setting tracking rates!!! Resetting everything.");
                    FirstImage = true; // reset everything, reference image etc.  

                }
                dataManager.AddRecord(new DataManager.DataRecord
                {
                    Timestamp = newheaders.LocalTime,
                    Filename = InputFilename,
                    Type = "LIGHT",
                    ExpTime = newheaders.ExposureTime,
                    Filter = newheaders.Filter, // old code has this commented out? Not sure why. 
                    DtSec = dt_sec,
                    PlateRa = solveResult.PlateRa,
                    PlateRaArcSecBuf = PlateRaArcSec,
                    RaP = PID_propotional_RA * dt_sec,
                    RaI = PID_integral_RA,
                    RaD = PID_derivative_RA,
                    NewRaRate = new_RA_rate,
                    PlateDec = solveResult.PlateDec,
                    PlateDecArcSecBuf = PlateDecArcSec,
                    DecP = PID_propotional_DEC * dt_sec,
                    DecI = PID_integral_DEC,
                    DecD = PID_derivative_DEC,
                    NewDecRate = new_DEC_rate,
                    ScopeRa = ScopeRa,
                    ScopeDec = ScopeDec,
                    FitsHeaderRa = ObsRa,
                    FitsHeaderDec = ObsDec,
                    PendingMessage = PendingMessage,
                    RateUpdateTimeStamp = DateTime.Now,
                });
            UpdateChartXY(PID_propotional_RA * dt_sec, PID_integral_RA, PID_propotional_DEC * dt_sec, PID_integral_DEC);

            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // EXCEPTION IF THREAD IS FAULT
                    throw t.Exception;
                }
                {
                    //PROCESS IMAGES AND DISPLAY
                }
            });

        }
        private void telescope_RightAscensionRate(double RaRate)
        {
            Stopwatch.Restart();
            telescope.RightAscensionRate = RaRate;
            if (Stopwatch.ElapsedMilliseconds > 100) { log.Error("Telescope.RightAscensionRate Latency: " + Stopwatch.ElapsedMilliseconds.ToString() + " ms"); }
            Stopwatch.Stop();
        }
        private void telescope_DeclinationRate(double DecRate)
        {
            Stopwatch.Restart();
            telescope.DeclinationRate = DecRate;
            if (Stopwatch.ElapsedMilliseconds > 100) { log.Error("Telescope.DeclinationRate Latency: " + Stopwatch.ElapsedMilliseconds.ToString() + " ms"); }
            Stopwatch.Stop();
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            save_settings();
        }
        
        public struct Platesolve
        {
            public string CatalogPath;
            public CatalogType CatalogType;

        };

        private void button4_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            textBox3.Text = folderBrowserDialog2.SelectedPath;
            settings.UCAC4_path = textBox3.Text;
            settings.Save();
            log.Info("Setting UCAC4 path to {@folder}", settings.WatchFolder);

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
            //MessageBox.Show($"Data saved to {filePath}");

            //string file = ExcelSave();
            System.Diagnostics.Process.Start(filePath);
        }
        private void AddMessage(string incomingMsg)
        {
            PendingMessage = PendingMessage + incomingMsg;
        }

        /*
        struct DataGridElement
        {
            public DateTime timestamp;
            public string type;
            public string filename;
            public double exptime;
            public string filter;
            public double dtsec;
            public double platera;
            public double plateraarcsecbuf;
            public double rap;
            public double rai;
            public double rad;
            public double newrarate;
            public double platedec;
            public double platedecarcsecbuf;
            public double decp;
            public double deci;
            public double decd;
            public double newdecrate;
            public string pendingmessage;
            public double scopera;
            public double scopedec;
            public double fitsheaderra;
            public double fitsheaderdec;
            public DateTime RateUpdateTimeStamp;

        }*/
        private void UpdateChartXY (double RaP, double RaI, double DecP, double DecI)
        {
                BeginInvoke(new System.Action(() =>
                {
                    ChartRa.Series[0].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), RaP);
                    ChartRa.Series[1].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), RaI);
                    ChartRa.ChartAreas[0].RecalculateAxesScale();

                    ChartDec.Series[0].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), DecP);
                    ChartDec.Series[1].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), DecI);
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
            //ExcelSave();
            //SetupDataGridView();
            
            //xlWorkBook.Close(false);
            //xlApp.Quit();
            //Marshal.ReleaseComObject(xlApp);
            //xlApp = new Excel.Application();

            //SetupExcelWriter();
            dataManager.Reset();

            ClearCharts();

            framesOld = new FrameList(settings.BufferFitsCount);
            frames = new FrameList(settings.BufferFitsCount, framesOld);
            PropotionalJournal_RA = new LinkedList<double>();
            PropotionalJournal_DEC = new LinkedList<double>();
            FirstImage = true;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            ResetAll();
        }
    }

}
    

