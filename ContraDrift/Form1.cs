﻿using System;
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
        //Excel.Application xlApp;
        private Excel.Application xlApp = new Excel.Application();

        Excel.Workbook xlWorkBook;
        Excel.Worksheet xlWorkSheet;


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
            SetupDataGridView();
            SetupCharts();
            SetupExcelWriter();

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
                    AddDataGridStruct(new DataGridElement
                    {
                        timestamp = newheaders.LocalTime,
                        filename = InputFilename,
                        type = "SHORTFRAME",
                        exptime = newheaders.ExposureTime,
                        filter = newheaders.Filter

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
                    AddDataGridStruct(new DataGridElement
                    {
                        timestamp = newheaders.LocalTime,
                        filename = InputFilename,
                        type = "SOLVEFAIL",
                        filter = newheaders.Filter,
                        exptime = newheaders.ExposureTime
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
                        AddDataGridStruct(new DataGridElement
                        {
                            timestamp = newheaders.LocalTime,
                            filename = InputFilename,
                            type = "BUFFER-" + (framesOld.Count() + frames.Count()) + "-REF",
                            exptime = newheaders.ExposureTime,
                            filter = newheaders.Filter,
                            platera = solveResult.PlateRa,
                            platedec = solveResult.PlateDec,
                            plateraarcsecbuf = PlateRaReference,
                            platedecarcsecbuf = PlateDecReference,
                            fitsheaderra = ObsRa,
                            fitsheaderdec = ObsDec,
                            pendingmessage = PendingMessage

                        }); 
                    }
                    else
                    {
                    


                        AddDataGridStruct(new DataGridElement
                        {
                            timestamp = newheaders.LocalTime, 
                            filename = InputFilename,
                            type = "BUFFER-" + (framesOld.Count() + frames.Count()),
                            exptime = newheaders.ExposureTime,
                            filter = newheaders.Filter,
                            platera = solveResult.PlateRa,
                            platedec = solveResult.PlateDec,
                            fitsheaderra = ObsRa,
                            fitsheaderdec = ObsDec,
                            pendingmessage = PendingMessage
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

                    AddDataGridStruct(new DataGridElement
                    {
                        timestamp = newheaders.LocalTime,
                        filename = InputFilename,
                        type = "PRECALC-" + PropotionalJournal_DEC.Count,
                        exptime = newheaders.ExposureTime,
                        filter = newheaders.Filter,
                        dtsec = dt_sec,
                        platera = solveResult.PlateRa,
                        plateraarcsecbuf = PlateRaArcSec,
                        rap = PID_propotional_RA * dt_sec,
                        rai = PID_integral_RA,
                        platedec = solveResult.PlateDec,
                        platedecarcsecbuf = PlateDecArcSec,
                        decp = PID_propotional_DEC * dt_sec,
                        deci = PID_integral_DEC,
                        pendingmessage = PendingMessage
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

                AddDataGridStruct(new DataGridElement
                {
                    timestamp = newheaders.LocalTime,
                    filename = InputFilename,
                    type = "LIGHT",
//                    filter = newheaders.Filter,
                    exptime = newheaders.ExposureTime,
                    filter = newheaders.Filter,
                    dtsec = dt_sec,
                    platera = solveResult.PlateRa,
                    plateraarcsecbuf = PlateRaArcSec,
                    rap = PID_propotional_RA * dt_sec,
                    rai = PID_integral_RA,
                    rad = PID_derivative_RA,
                    newrarate = new_RA_rate,
                    platedec = solveResult.PlateDec,
                    platedecarcsecbuf = PlateDecArcSec,
                    decp = PID_propotional_DEC * dt_sec,
                    deci = PID_integral_DEC,
                    decd = PID_derivative_DEC,
                    newdecrate = new_DEC_rate,
                    pendingmessage = PendingMessage,
                    scopera = ScopeRa,
                    scopedec = ScopeDec,
                    fitsheaderra = ObsRa,
                    fitsheaderdec = ObsDec,
                    RateUpdateTimeStamp = DateTime.Now
                }); ;


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
                            int columnCount = dataGridView1.Columns.Count;
                            string columnNames = "";
                            string[] outputCsv = new string[dataGridView1.Rows.Count + 1];
                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                            }
                            outputCsv[0] += columnNames;

                            for (int i = 1; (i - 1) < dataGridView1.Rows.Count; i++)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    outputCsv[i] += dataGridView1.Rows[i - 1].Cells[j].Value.ToString() + ",";
                                }
                            }

                            File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                            //MessageBox.Show("Data Exported Successfully !!!", "Info");
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
            string file = ExcelSave();
            System.Diagnostics.Process.Start(@file);
        }
        private void AddMessage(string incomingMsg)
        {
            PendingMessage = PendingMessage + incomingMsg;
        }
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

        }

        private void AddDataGridStruct(DataGridElement datagridelement)
        {
            dataGridView1.Invoke(new System.Action(() =>
            {
                datatable.Rows.Add(
                datagridelement.timestamp,
                datagridelement.filename,
                datagridelement.type,
                datagridelement.exptime,
                datagridelement.filter,
                datagridelement.dtsec,
                datagridelement.platera,
                datagridelement.plateraarcsecbuf,
                datagridelement.rap,
                datagridelement.rai,
                datagridelement.rad,
                datagridelement.newrarate,
                datagridelement.platedec,
                datagridelement.platedecarcsecbuf,
                datagridelement.decp,
                datagridelement.deci,
                datagridelement.decd,
                datagridelement.newdecrate,
                datagridelement.pendingmessage,
                datagridelement.scopera,
                datagridelement.scopedec,
                datagridelement.fitsheaderra,
                datagridelement.fitsheaderdec,
                datagridelement.RateUpdateTimeStamp
                );

                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                // datatable.AcceptChanges();
            }));

            //ChartRa.Series[0].Points.Add(datagridelement.plateraarcsecbuf);
            if (datagridelement.type == "LIGHT")
            {
                BeginInvoke(new System.Action(() =>
                {
                    ChartRa.Series[0].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), datagridelement.rap);
                    ChartRa.Series[1].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), datagridelement.rai);
                    ChartRa.ChartAreas[0].RecalculateAxesScale();

                    ChartDec.Series[0].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), datagridelement.decp);
                    ChartDec.Series[1].Points.AddXY((dataGridView1.RowCount - framesOld.PlateCollectionCeiling() - frames.PlateCollectionCeiling()), datagridelement.deci);
                    ChartDec.ChartAreas[0].RecalculateAxesScale();
                }));

            }
            // TODO update excel object.

            BeginInvoke(new System.Action(() =>
            {


                int i = dataGridView1.RowCount - 1;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Timestamp") + 1] = datagridelement.timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Timestamp") + 1].NumberFormat = "m/d/yyyy h:mm:ss.000";
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Filename") + 1] = datagridelement.filename;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Type") + 1] = datagridelement.type;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("ExpTime") + 1] = datagridelement.exptime;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Filter") + 1] = datagridelement.filter;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("dt_sec") + 1] = datagridelement.dtsec;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("PlateRa") + 1] = datagridelement.platera;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("PlateRaArcSecBuf") + 1] = datagridelement.plateraarcsecbuf;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("RaP") + 1] = datagridelement.rap;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("RaI") + 1] = datagridelement.rai;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("RaD") + 1] = datagridelement.rad;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("NewRaRate") + 1] = datagridelement.newrarate;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("PlateDec") + 1] = datagridelement.platedec;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("PlateDecArcSecBuf") + 1] = datagridelement.platedecarcsecbuf;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("DecP") + 1] = datagridelement.decp;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("DecI") + 1] = datagridelement.deci;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("DecD") + 1] = datagridelement.decd;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("NewDecRate") + 1] = datagridelement.newdecrate;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("Messages") + 1] = PendingMessage;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("ScopeRa") + 1] = datagridelement.scopera;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("ScopeDec") + 1] = datagridelement.scopedec;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("FitsHeaderRa") + 1] = datagridelement.fitsheaderra;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("FitsHeaderDec") + 1] = datagridelement.fitsheaderdec;
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("RateUpdateTimeStamp") + 1] = datagridelement.RateUpdateTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                xlWorkSheet.Cells[i + 2, datatable.Columns.IndexOf("RateUpdateTimeStamp") + 1].NumberFormat = "m/d/yyyy h:mm:ss.000";


            }));
            PendingMessage = "";


        }
        private void SetupExcelWriter() {
            object misValue = System.Reflection.Missing.Value;
            //xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            for (int j = 0; j <= dataGridView1.ColumnCount - 1; j++)
            {
                xlWorkSheet.Cells[1, j + 1] = dataGridView1.Columns[j].HeaderText.ToString();
            }
            
        }
        private string ExcelSave() {
            string Filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "ContraDriftLog" + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy -MM-dd HHMMss") + " - ContraDrift.xlsx";
            xlWorkSheet.UsedRange.Columns.AutoFit();
            xlWorkBook.SaveCopyAs(Filename);
            return Filename;
        }

        private void SetupDataGridView()
        {

            datatable = new System.Data.DataTable();
            //dataGridView1 = new DataGridView();


            dataGridView1.DataSource = datatable;

            datatable.Columns.Add(new DataColumn("Timestamp", typeof(DateTime)));
            dataGridView1.Columns[datatable.Columns.IndexOf("Timestamp")].DefaultCellStyle.Format = "HH:mm:ss.fff";
            dataGridView1.Columns[datatable.Columns.IndexOf("Timestamp")].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;//  .DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("Timestamp")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("Filename", typeof(String)));
            dataGridView1.Columns[datatable.Columns.IndexOf("Filename")].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridView1.Columns[datatable.Columns.IndexOf("Filename")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("Filename")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("Type", typeof(String)));
            dataGridView1.Columns[datatable.Columns.IndexOf("Type")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("Type")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("Type")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("ExpTime", typeof(String)));
            dataGridView1.Columns[datatable.Columns.IndexOf("ExpTime")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("ExpTime")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("ExpTime")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("ExpTime")].DefaultCellStyle.Format = "0.0";

            datatable.Columns.Add(new DataColumn("Filter", typeof(String)));
            dataGridView1.Columns[datatable.Columns.IndexOf("Filter")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("Filter")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("Filter")].SortMode = DataGridViewColumnSortMode.NotSortable;


            datatable.Columns.Add(new DataColumn("dt_sec", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("dt_sec")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("dt_sec")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("dt_sec")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("dt_sec")].DefaultCellStyle.Format = "0.0";

            datatable.Columns.Add(new DataColumn("PlateRa", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRa")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRa")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRa")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRa")].DefaultCellStyle.Format = "0.0000000";
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRa")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("PlateRaArcSecBuf", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRaArcSecBuf")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRaArcSecBuf")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRaArcSecBuf")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRaArcSecBuf")].DefaultCellStyle.Format = "0.00";
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateRaArcSecBuf")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("RaP", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("RaP")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaP")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaP")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaP")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("RaI", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("RaI")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaI")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaI")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaI")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("RaD", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("RaD")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaD")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaD")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("RaD")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("NewRaRate", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("NewRaRate")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewRaRate")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewRaRate")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewRaRate")].DefaultCellStyle.Format = "0.00000";


            datatable.Columns.Add(new DataColumn("PlateDec", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDec")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDec")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDec")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDec")].DefaultCellStyle.Format = "0.0000000";

            datatable.Columns.Add(new DataColumn("PlateDecArcSecBuf", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDecArcSecBuf")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDecArcSecBuf")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDecArcSecBuf")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDecArcSecBuf")].DefaultCellStyle.Format = "0.00";
            dataGridView1.Columns[datatable.Columns.IndexOf("PlateDecArcSecBuf")].SortMode = DataGridViewColumnSortMode.NotSortable;

            datatable.Columns.Add(new DataColumn("DecP", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("DecP")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecP")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecP")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecP")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("DecI", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("DecI")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecI")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecI")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecI")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("DecD", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("DecD")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecD")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecD")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("DecD")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("NewDecRate", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("NewDecRate")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewDecRate")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewDecRate")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("NewDecRate")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("Messages", typeof(String)));
            dataGridView1.Columns[datatable.Columns.IndexOf("Messages")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("Messages")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("Messages")].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            datatable.Columns.Add(new DataColumn("ScopeRa", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeRa")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeRa")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeRa")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeRa")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("ScopeDec", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeDec")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeDec")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeDec")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("ScopeDec")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("FitsHeaderRa", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderRa")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderRa")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderRa")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderRa")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("FitsHeaderDec", typeof(double)));
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderDec")].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderDec")].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderDec")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("FitsHeaderDec")].DefaultCellStyle.Format = "0.00000";

            datatable.Columns.Add(new DataColumn("RateUpdateTimeStamp", typeof(DateTime)));
            dataGridView1.Columns[datatable.Columns.IndexOf("RateUpdateTimeStamp")].DefaultCellStyle.Format = "HH:mm:ss.fff";
            dataGridView1.Columns[datatable.Columns.IndexOf("RateUpdateTimeStamp")].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[datatable.Columns.IndexOf("RateUpdateTimeStamp")].SortMode = DataGridViewColumnSortMode.NotSortable;


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
            ExcelSave();
            SetupDataGridView();
            
            xlWorkBook.Close(false);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            xlApp = new Excel.Application();

            SetupExcelWriter();
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
    

