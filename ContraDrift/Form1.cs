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
        public FrameList frames = new FrameList(4);
        private Telescope telescope;
        private bool FirstImage = true;


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


        private double PID_previous_PlateRa;   //averaged
        private double PID_previous_PlateDec; // averaged
        private double dt_sec;

        private DateTime LastExposureCenter;
        




        public Form1()
        {
            InitializeComponent();
            ConfigureLogger();
            //LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            //log = LogManager.GetCurrentClassLogger();


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
            frames.SetMaxBufferCount(settings.BufferFitsCount);


            if (settings.ProcessingTraditional) { ProcessingTraditional.Checked = true; } else { ProcessingFilter.Checked = true; }


            settings.Status = "Stopped";


        }

        private void ConfigureLogger()
        {
            //log = LogManager.GetCurrentClassLogger();
            if (log == null) log = LogManager.GetCurrentClassLogger();


            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "${specialfolder:folder=MyDocuments}\\ContraDriftLog\\ContraDriftLog-${date:format=yyyy-MM-dd}.txt" };
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


        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
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
                settings.DecRateLimitSetting = float.Parse(DecRateLimitTextBox.Text);
                settings.RaRateLimitSetting = float.Parse(RaRateLimitTextBox.Text);
                settings.BufferFitsCount = int.Parse(BufferFitsCount.Text);
                frames.SetMaxBufferCount(int.Parse(BufferFitsCount.Text));

                settings.Save();
            }
            catch { log.Debug("Problems with save settings"); } // some garbage input we just toss if we can't parse it. 

            if (ProcessingTraditional.Checked) { settings.ProcessingTraditional = true; } else { settings.ProcessingTraditional = false; }
            log.Debug("Settings saved..");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (settings.Status == "Running")
            {
                // stop
                log.Info("Stopping");
                settings.Status = "Stopped";
                button3.Text = "Start";
                button1.Enabled = true;
                button2.Enabled = true;

                watcher.EnableRaisingEvents = false;
                //watcher.Dispose();

                telescope.RightAscensionRate = 0;
                telescope.DeclinationRate = 0;
                telescope.Connected = false;
                telescope.Dispose();

                FirstImage = true;

            }
            else
            {
                // start
                log.Info("Starting");
                settings.Status = "Running";
                button3.Text = "Stop";
                button1.Enabled = false;
                button2.Enabled = false;

                telescope = new Telescope(settings.TelescopeProgId);
                telescope.Connected = true;
                telescope.Tracking = true;
                //telescope.SlewToCoordinatesAsync(20.7333995798772, 45.3667092514744); //a blocking call for testing; 
                

                save_settings();

                //worker.DoWork += new DoWorkEventHandler(worker_backgroundProcess);
                //worker.RunWorkerAsync();

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

                    watcher.Filter = "*.fits";

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
            _ = tFactory.StartNew(() =>
            {



            double ScopeRa, ScopeDec;
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
            log.Debug("New File: " + InputFilename);
            (bool Solved, double PlateRa, double PlateDec, DateTime PlateLocaltime, double PlateExposureTime, double Airmass, float Solvetime) = SolveFits(InputFilename, PlateRaPrevious, PlateDecPrevious);


            double PlateRaArcSec = PlateRa * 15 * 3600; // convert from hours to arcsec
            double PlateDecArcSec = PlateDec * 3600; // convert from degrees to arcsec
            double PlateDecArcSecOld, PlateRaArcSecOld;


            ScopeRa = telescope.RightAscension;
            ScopeDec = telescope.Declination;
            ScopeRaRate = telescope.RightAscensionRate / 15; //  arcsec to RA Sec per sidereal second divide by 15.  
            ScopeDecRate = telescope.DeclinationRate;

            log.Debug("ScopeRa: " + ScopeRa + ",ScopeDec: " + ScopeDec + ",ScopeRaRate: " + ScopeRaRate + ",ScopeDecRate: " + ScopeDecRate);

            if (!Solved) { log.Error("Platesolved failed! "); return; }

            frames.AddPlateCollection(PlateRaArcSec, PlateDecArcSec, PlateLocaltime, PlateExposureTime);

            if (!frames.IsBufferFull()) { log.Debug("Buffer not full.. "); return; }

            (PlateRaArcSecOld, PlateDecArcSecOld, PlateRaArcSec, PlateDecArcSec) = frames.GetPlateCollectionAverage();

            if (FirstImage)
            {
                if (Solved)
                {
                    FirstImage = false;
                    PlateRaReference = PlateRaArcSec;
                    PlateDecReference = PlateDecArcSec;
                    (LastExposureCenter, ExposureCenter )= frames.GetPlateCollectionLocalExposureTimeCenter();
                    log.Debug("FirstImage:  LastExposureCenter: " + LastExposureCenter + ", PlateRa: " + PlateRa + " ,PlateDec: " + PlateDec + ",PlateLocaltime: " + PlateLocaltime + ",PlateExposureTime: " + PlateExposureTime);
                    PID_previous_PlateRa = PlateRaArcSec;
                    PID_previous_PlateDec = PlateDecArcSec;
                    PID_propotional_RA = 0; PID_integral_RA = 0; PID_derivative_RA = 0; PID_previous_propotional_RA = 0;
                }

            }
            else
            {
                    //dt_sec = ((PlateLocaltime.AddSeconds(PlateExposureTime / 2) - LastExposureTime).TotalMilliseconds) / 1000;

                    (LastExposureCenter, ExposureCenter) = frames.GetPlateCollectionLocalExposureTimeCenter();
                    log.Debug("ExposureCenter: " + ExposureCenter);
                    log.Debug("LastExposureCenter: " + LastExposureCenter);
                    dt_sec = ((ExposureCenter - LastExposureCenter).TotalMilliseconds) / 1000;
                    log.Debug("dt_sec: " + dt_sec);

                    // dt_sec = (GetPlateCollectionLocalExposureTimeCenter() - LastExposureTime).TotalMilliseconds) / 1000;

                    // PID control for RA
                    PID_propotional_RA = (PlateRaArcSec - PID_previous_PlateRa) / (dt_sec); ;
                    PID_integral_RA = (PlateRaArcSec - PlateRaReference) ;
                    PID_derivative_RA = (PID_propotional_RA - PID_previous_propotional_RA) / (dt_sec);
                    new_RA_rate = settings.PID_Setting_Kp_RA * PID_propotional_RA + settings.PID_Setting_Ki_RA * PID_integral_RA + settings.PID_Setting_Kd_RA * PID_derivative_RA;


                    // PID control for RA with IIF filtered derivative - set up dt dependent constants
                    A0_RA = settings.PID_Setting_Kp_RA_filter + settings.PID_Setting_Ki_RA_filter * dt_sec + settings.PID_Setting_Kd_RA_filter / dt_sec;
                    A1_RA = -settings.PID_Setting_Kp_RA_filter;
                    A0d_RA = settings.PID_Setting_Kd_RA_filter / dt_sec;
                    A1d_RA = -2.0 * settings.PID_Setting_Kd_RA_filter / dt_sec;
                    A2d_RA = settings.PID_Setting_Kd_RA_filter / dt_sec;
                    Nfilt_RA = settings.PID_Setting_Nfilt_RA;
                    tau_RA = settings.PID_Setting_Kd_RA_filter / (settings.PID_Setting_Kp_RA_filter * Nfilt_RA);
                    alpha_RA = dt_sec / (2.0 * tau_RA);

                    // Perform RA PID with IIF filtered derivative
                    PID_error_third_RA = PID_error_last_RA;
                    PID_error_last_RA = PID_previous_propotional_RA;
                    PID_error_new_RA = PID_propotional_RA;
                    PID_der1_RA = PID_der0_RA;
                    PID_der0_RA = A0d_RA * PID_error_new_RA + A1d_RA * PID_error_last_RA + A2d_RA * PID_error_third_RA;
                    fder0_RA = (alpha_RA / (alpha_RA + 1)) * (PID_der0_RA + PID_der1_RA) - ((alpha_RA - 1) / (alpha_RA + 1)) * PID_der1_RA;
                    new_RA_rate_filtder = A0_RA * PID_error_new_RA + A1_RA * PID_error_last_RA + fder0_RA;

                    log.Debug("PID_RA:  PID_previous_propotional_RA: " + PID_previous_propotional_RA + ",PID_propotional_RA: " + PID_propotional_RA + ",PID_integral_RA: " + PID_integral_RA + ",PID_derivative_RA: " + PID_derivative_RA + ",new_RA_rate: " + new_RA_rate);
                    log.Debug("PID_RA_settings:  PID_Setting_Kp_RA: " + settings.PID_Setting_Kp_RA + ",PID_Setting_Ki_RA: " + settings.PID_Setting_Ki_RA + ",PID_Setting_Kd_RA: " + settings.PID_Setting_Kd_RA);
                    log.Debug("PID_RA_filter settings:  PID_Setting_Kp_RA_filter: " + settings.PID_Setting_Kp_RA_filter + ",PID_Setting_Ki_RA_filter: " + settings.PID_Setting_Ki_RA_filter + ",PID_Setting_Kd_RA_filter: " + settings.PID_Setting_Kd_RA_filter + ",PID_Setting_Nfilt_RA: " + settings.PID_Setting_Nfilt_RA);
                    log.Debug("PID_RA_Filter:  fder0_RA: " + fder0_RA);



                    // standard PID control for DEC
                    PID_propotional_DEC = (PlateDecArcSec - PID_previous_PlateDec) / (dt_sec); ;
                    PID_integral_DEC = (PlateDecArcSec - PlateDecReference) ;
                    PID_derivative_DEC = (PID_propotional_DEC - PID_previous_propotional_DEC) / (dt_sec);
                    new_DEC_rate = settings.PID_Setting_Kp_DEC * PID_propotional_DEC + settings.PID_Setting_Ki_DEC * PID_integral_DEC + settings.PID_Setting_Kd_DEC * PID_derivative_DEC;



                    // PID control for DEC with IIF filtered derivative - set up dt dependent constants
                    A0_DEC = settings.PID_Setting_Kp_DEC_filter + settings.PID_Setting_Ki_DEC_filter * dt_sec + settings.PID_Setting_Kd_DEC_filter / dt_sec;
                    A1_DEC = -settings.PID_Setting_Kp_DEC_filter;
                    A0d_DEC = settings.PID_Setting_Kd_DEC_filter / dt_sec;
                    A1d_DEC = -2.0 * settings.PID_Setting_Kd_DEC_filter / dt_sec;
                    A2d_DEC = settings.PID_Setting_Kd_DEC_filter / dt_sec;
                    Nfilt_DEC = settings.PID_Setting_Nfilt_DEC;
                    tau_DEC = settings.PID_Setting_Kd_DEC_filter / (settings.PID_Setting_Kp_DEC_filter * Nfilt_DEC);
                    alpha_DEC = dt_sec / (2.0 * tau_DEC);

                    // Perform DEC PID with IIF filtered derivative
                    PID_error_third_DEC = PID_error_last_DEC;
                    PID_error_last_DEC = PID_previous_propotional_DEC;
                    PID_error_new_DEC = PID_propotional_DEC;
                    PID_der1_DEC = PID_der0_DEC;
                    PID_der0_DEC = A0d_DEC * PID_error_new_DEC + A1d_DEC * PID_error_last_DEC + A2d_DEC * PID_error_third_DEC;
                    fder0_DEC = (alpha_DEC / (alpha_DEC + 1)) * (PID_der0_DEC + PID_der1_DEC) - ((alpha_DEC - 1) / (alpha_DEC + 1)) * PID_der1_DEC;
                    new_DEC_rate_filtder = A0_DEC * PID_error_new_DEC + A1_DEC * PID_error_last_DEC + fder0_DEC;

                    log.Debug("PID_DEC:  PID_previous_propotional_DEC: " + PID_previous_propotional_DEC + ",PID_propotional_DEC: " + PID_propotional_DEC + ",PID_integral_DEC: " + PID_integral_DEC + ",PID_derivative_DEC: " + PID_derivative_DEC + ",new_DEC_rate: " + new_DEC_rate);
                    log.Debug("PID_DEC_settings:  PID_Setting_Kp_DEC: " + settings.PID_Setting_Kp_DEC + ",PID_Setting_Ki_DEC: " + settings.PID_Setting_Ki_DEC + ",PID_Setting_Kd_DEC: " + settings.PID_Setting_Kd_DEC);
                    log.Debug("PID_DEC_filter settings:  PID_Setting_Kp_DEC_filter: " + settings.PID_Setting_Kp_DEC_filter + ",PID_Setting_Ki_DEC_filter: " + settings.PID_Setting_Ki_DEC_filter + ",PID_Setting_Kd_DEC_filter: " + settings.PID_Setting_Kd_DEC_filter + ",PID_Setting_Nfilt_DEC: " + settings.PID_Setting_Nfilt_DEC);
                    log.Debug("PID_DEC_Filter:  fder0_DEC: " + fder0_DEC);


                    PID_previous_3rd_propotional_RA = PID_previous_propotional_RA;
                    PID_previous_3rd_propotional_DEC = PID_previous_propotional_DEC;
                    PID_previous_propotional_RA = PID_propotional_RA;
                    PID_previous_propotional_DEC = PID_propotional_DEC;
                    PID_previous_PlateDec = PlateDecArcSec;
                    PID_previous_PlateRa = PlateRaArcSec;
                    LastExposureCenter = ExposureCenter;

                    PlateRaPrevious = PlateRa;
                    PlateDecPrevious = PlateDec;


                    //LastExposureTime = PlateLocaltime.AddSeconds(PlateExposureTime / 2);


                    log.Info("RA drift: " + PID_integral_RA);
                    log.Info("DEC drift: " + PID_integral_DEC);

                }

                if (ProcessingFilter.Checked)
                {
                    new_DEC_rate = new_DEC_rate_filtder;
                    new_RA_rate = new_RA_rate_filtder;
                    log.Debug("Processing Filter mode enabled, override new_RA_rate to: " + new_RA_rate + " ,new_DEC_rate: " + new_DEC_rate);

                }

            // TODO: Check that the mount is tracking, and if telescope.RARateIsSettable is true. 
            if (telescope.Tracking && telescope.CanSetRightAscensionRate && telescope.CanSetDeclinationRate )
            {
                if (new_RA_rate < float.Parse(RaRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Rate of " + new_RA_rate); new_RA_rate = float.Parse(RaRateLimitTextBox.Text) * -1; }
                if (new_RA_rate > float.Parse(RaRateLimitTextBox.Text) ) { log.Debug("Refusing to set extreme Rate of " + new_RA_rate); new_RA_rate = float.Parse(RaRateLimitTextBox.Text);  }
                telescope.RightAscensionRate = new_RA_rate / 15;
                log.Debug("Setting RightAscensionRate: " + new_RA_rate);
                if (new_DEC_rate < float.Parse(DecRateLimitTextBox.Text) * -1) { log.Debug("Refusing to set extreme Dec Rate of " + new_DEC_rate); new_DEC_rate = float.Parse(DecRateLimitTextBox.Text) * -1; }
                if (new_DEC_rate > float.Parse(DecRateLimitTextBox.Text)) { log.Debug("Refusing to set extreme Dec Rate of " + new_DEC_rate); new_DEC_rate = float.Parse(DecRateLimitTextBox.Text);  }
                telescope.DeclinationRate = new_DEC_rate / 0.9972695677;
                log.Debug("Setting DeclinationRate: " + new_DEC_rate);
            }
            else
            {
                log.Error("Telescope is not tracking!!! not setting tracking rates!!! Resetting everything.");
                FirstImage = true; // reset everything, reference image etc.  

            }


                dataGridView1.Invoke(new Action(() => { 
                dataGridView1.Rows.Add(
                    DateTime.Now,
                    InputFilename,
                    String.Format("{0:0.0}", dt_sec),
                    String.Format("{0:0.000000}", PlateRa),
                    String.Format("{0:0.00000}", PID_propotional_RA),
                    String.Format("{0:0.00000}", PID_integral_RA),
                    String.Format("{0:0.00000}", PID_derivative_RA),
                    String.Format("{0:0.0000}", new_RA_rate),


                    String.Format("{0:0.000000}", PlateDec),
                    String.Format("{0:0.00000}", PID_propotional_DEC),
                    String.Format("{0:0.00000}", PID_integral_DEC),
                    String.Format("{0:0.00000}", PID_derivative_DEC),
                    String.Format("{0:0.0000}", new_DEC_rate)
                );
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1; 
                }));
                //dataGridView1.Invoke(new Action(() =>  { dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.SelectedRows[0].Index; }));
    

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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            save_settings();
        }

        public (bool, double, double, DateTime, double, double, float) SolveFits(string InputFilename, double LastPlateRa = -1, double LastPlateDec = -1)
        {

            double PlateRa = 0, PlateDec = 0, PlateExposureTime = 0;
            DateTime PlateLocaltime = DateTime.Now;
            double Airmass = 0;
            float Solvetime = 0;
            bool Solved = false;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Plate p = new Plate();
            try {

                p.AttachFITS(InputFilename);
                p.ArcsecPerPixelHoriz = (Convert.ToDouble(p.ReadFITSValue("XPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
                p.ArcsecPerPixelVert = (Convert.ToDouble(p.ReadFITSValue("YPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
                if (LastPlateRa == -1) { p.RightAscension = p.TargetRightAscension; } else { p.RightAscension = LastPlateRa;  }
                if (LastPlateDec == -1) { p.Declination = p.TargetDeclination; } else { p.Declination = LastPlateDec; }

                p.Catalog = (CatalogType)11;
                p.CatalogPath = settings.UCAC4_path;
                p.CatalogExpansion = 0.4;
                p.Solve();
                PlateRa = p.RightAscension; // in hours
                PlateDec = p.Declination;  // in degrees
                PlateLocaltime = p.ExposureStartTime;
                PlateExposureTime = p.ExposureInterval;
                Airmass = p.Airmass;
                Solved = p.Solved;

                //log.Info("Platesolve: {@InputFilename},{@RightAscension},{@PlateDec},{@PlateLocaltime},{@PlateExposureTime},{@AirMass},{@SolveTime},", InputFilename, PlateRa, PlateDec, PlateLocaltime, p.Airmass, stopwatch.ElapsedMilliseconds / 1000);
                log.Info("Platesolve: Filename: " + InputFilename + " PlateRa: " + PlateRa + " PlateDec: " + PlateDec + " PlateLocaltime: " + PlateLocaltime + " Airmass: " + p.Airmass + " Solvetime: " + (float)stopwatch.ElapsedMilliseconds / 1000);

            }
            catch (Exception ex)
            {
                log.Debug(ex);
                _ = Marshal.ReleaseComObject(p);
                return (false, 0, 0, DateTime.Now, 0, 0, 0);

            }


            p.DetachFITS();
            _ = Marshal.ReleaseComObject(p); // important or the com object leaks memory


            stopwatch.Stop();
            //  (bool Solved, double PlateRa, double PlateDec, DateTime PlateLocaltime, double PlateExposureTime, double Airmass, float Solvetime) =  SolveFits(InputFilename);
            return (Solved, PlateRa, PlateDec, PlateLocaltime, PlateExposureTime, Airmass, Solvetime);



        }
        //public Solves
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

            string Filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "ContraDriftLog" + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy -MM-dd HHMMss") + " - ContraDrift.xls";
            if (dataGridView1.RowCount >0)
            {

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                int i = 0;
                int j = 0;

                for (j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                {
                    xlWorkSheet.Cells[1, j + 1] = dataGridView1.Columns[j].HeaderText.ToString();
                }

                for (i = 0; i <= dataGridView1.RowCount - 1; i++)
                {
                    for (j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                    {
                        DataGridViewCell cell = dataGridView1[j, i];
                        xlWorkSheet.Cells[i + 2, j + 1] = cell.Value;
                    }
                }
                xlWorkSheet.UsedRange.Columns.AutoFit();

                xlWorkBook.SaveAs(Filename, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                System.Diagnostics.Process.Start(@Filename);

            }
        }
      

    }

}
    

