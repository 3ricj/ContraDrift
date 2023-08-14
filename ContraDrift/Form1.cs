using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ASCOM.DriverAccess;
using ASCOM.DeviceInterface;
using System.IO;
using PinPoint;
using System.Runtime.InteropServices;
using NLog;


namespace ContraDrift
{
    public partial class Form1 : Form
    {
        Logger log = LogManager.GetCurrentClassLogger();
        static ContraDrift.Properties.Settings settings = Properties.Settings.Default;
        BackgroundWorker worker = new BackgroundWorker();
        FileSystemWatcher watcher = new FileSystemWatcher();
        TaskFactory tFactory = new TaskFactory();
        private Telescope telescope;
        private bool FirstImage = true;

        private double PID_previous_error_RA = 0;
        // state variables for standard PID loop for RA
        private double PID_propotional_RA = 0;
        private double PID_integral_RA = 0;
        private double PID_error_RA = 0;
        private double PID_derivative_RA = 0;

        // state variables for IIF filtered derivativePID loop for RA
        private double PID_error_new_RA = 0;
        private double PID_error_last_RA = 0;
        private double PID_error_third_RA = 0;
        private double PID_der0_RA = 0;
        private double PID_der1_RA = 0;

        // state variables for standard PID loop for DEC
        private double PID_previous_error_DEC = 0;
        private double PID_propotional_DEC = 0;
        private double PID_integral_DEC = 0;
        private double PID_error_DEC = 0;
        private double PID_derivative_DEC = 0;

        // state variables for IIF filtered derivativePID loop for DEC
        private double PID_error_new_DEC = 0;
        private double PID_error_last_DEC = 0;
        private double PID_error_third_DEC = 0;
        private double PID_der0_DEC = 0;
        private double PID_der1_DEC = 0;

        private double PlateRaReference;
        private double PlateDecReference;

        private DateTime LastExposureTime;



        public Form1()
        {
            InitializeComponent();
            //LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            //log = LogManager.GetCurrentClassLogger();

            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "${specialfolder:folder=MyDocuments}\\ContraDriftLog\\ContraDriftLog-${date:format=yyyy-MM-dd}.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;

            log.Info("Starting up");


            textBox1.Text = settings.TelescopeProgId;
            textBox2.Text = settings.WatchFolder;
            PID_Setting_Kp_RA.Text = settings.PID_Setting_Kp_RA.ToString();
            PID_Setting_Ki_RA.Text = settings.PID_Setting_Ki_RA.ToString();
            PID_Setting_Kd_RA.Text = settings.PID_Setting_Kd_RA.ToString();
            PID_Setting_Nfilt_RA.Text = settings.PID_Setting_Nfilt_RA.ToString();
            PID_Setting_Kp_DEC.Text = settings.PID_Setting_Kp_DEC.ToString();
            PID_Setting_Ki_DEC.Text = settings.PID_Setting_Ki_DEC.ToString();
            PID_Setting_Kd_DEC.Text = settings.PID_Setting_Kd_DEC.ToString();
            PID_Setting_Nfilt_DEC.Text = settings.PID_Setting_Nfilt_DEC.ToString();


            settings.Status = "Stopped";


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
                telescope.Connected = false;
                telescope.Dispose();



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

                double alt, az;

                alt = telescope.Altitude;
                az = telescope.Azimuth;
                log.Debug("alt = " + alt + "\t az = " + az);

                worker.DoWork += new DoWorkEventHandler(worker_backgroundProcess);
                worker.RunWorkerAsync();
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
                watcher.Changed += new FileSystemEventHandler(ProcessNewFits);

                watcher.Filter = "*.fits";
                watcher.EnableRaisingEvents = true;
                log.Debug("Watcher Enabled");
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

                double PlateRa = 0, PlateDec = 0, PlateExposureTime;
                DateTime PlateLocaltime;
                double ScopeRa, ScopeDec;
                double ScopeRaRate, ScopeDecRate;
                double dt_sec;

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

                string InputFilename = e.FullPath;
                log.Debug("New File: " + InputFilename);
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                Plate p = new Plate();
                try { p.AttachFITS(InputFilename); } catch (Exception ex) { log.Debug(ex); }

                p.ArcsecPerPixelHoriz = (Convert.ToDouble(p.ReadFITSValue("XPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
                p.ArcsecPerPixelVert = (Convert.ToDouble(p.ReadFITSValue("YPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
                p.RightAscension = p.TargetRightAscension;
                p.Declination = p.TargetDeclination;
                p.Catalog = (CatalogType)11;
                p.CatalogPath = "J:/UCAC4";

                try
                {
                    p.Solve();
                    log.Debug(p.RightAscension + ",");
                    log.Debug(p.Declination + ",");
                    PlateRa = p.RightAscension;
                    PlateDec = p.Declination;
                    PlateLocaltime = p.ExposureStartTime;
                    PlateExposureTime = p.ExposureInterval;
                    log.Info("Platesolve: {@InputFilename},{@RightAscension},{@PlateDec},{@PlateLocaltime},{@PlateExposureTime},{@AirMass},{@SolveTime},", InputFilename, PlateRa, PlateDec, PlateLocaltime, p.Airmass, stopwatch.ElapsedMilliseconds / 1000);
                    
                }
                catch (Exception ex) { log.Debug(ex); }


                stopwatch.Stop();
                log.Debug((float)stopwatch.ElapsedMilliseconds / 1000);
                p.DetachFITS();
                _ = Marshal.ReleaseComObject(p); // important or the com object leaks memory

                telescope.SlewToCoordinates(20.7333995798772, 45.3667092514744); //a blocking call for testing; 

                ScopeRa = telescope.RightAscension;
                ScopeDec = telescope.Declination;
                ScopeRaRate = telescope.RightAscensionRate / 15; //  arcsec to RA Sec per sidereal second divide by 15.  
                ScopeDecRate = telescope.DeclinationRate;

                log.Debug("ScopeRa: " + ScopeRa);
                log.Debug("ScopeDec: " + ScopeDec);
                log.Debug("ScopeRaRate: " + ScopeRaRate);
                log.Debug("ScopeDecRate: " + ScopeDecRate);


                if (FirstImage)
                {
                    if (p.Solved)
                    {
                        FirstImage = false;
                        PlateRaReference = PlateRa;
                        PlateDecReference = PlateDec;
                        LastExposureTime = p.ExposureStartTime.AddSeconds(p.ExposureInterval / 2);
                    }
                }
                else
                {
                    dt_sec = ((p.ExposureStartTime.AddSeconds(p.ExposureInterval / 2) - LastExposureTime).TotalMilliseconds)*1000;

                    // PID control for RA
                    PID_error_RA = ScopeRa - PlateRa;
                    PID_propotional_RA = PID_error_RA;
                    PID_integral_RA = PlateRa - PlateRaReference;
                    PID_derivative_RA = (PID_error_RA - PID_previous_error_RA) / (dt_sec);
                    new_RA_rate = settings.PID_Setting_Kp_RA * PID_propotional_RA + settings.PID_Setting_Ki_RA * PID_integral_RA + settings.PID_Setting_Kd_RA * PID_derivative_RA;

                    log.Debug("PID_propotional_RA: " + PID_propotional_RA);
                    log.Debug("PID_integral_RA: " + PID_integral_RA);
                    log.Debug("PID_derivative_RA: " + PID_derivative_RA);
                    log.Debug("new_RA_rate: " + new_RA_rate);


                    // PID control for RA with IIF filtered derivative - set up dt dependent constants
                    A0_RA = settings.PID_Setting_Kp_RA + settings.PID_Setting_Ki_RA * dt_sec + settings.PID_Setting_Kd_RA / dt_sec;
                    A1_RA = -settings.PID_Setting_Kp_RA;
                    A0d_RA = settings.PID_Setting_Kd_RA / dt_sec;
                    A1d_RA = -2.0 * settings.PID_Setting_Kd_RA / dt_sec;
                    A2d_RA = settings.PID_Setting_Kd_RA / dt_sec;
                    Nfilt_RA = settings.PID_Setting_Nfilt_RA;
                    tau_RA = settings.PID_Setting_Kd_RA / (settings.PID_Setting_Kp_RA * Nfilt_RA);
                    alpha_RA = dt_sec / (2.0 * tau_RA);

                    // Perform RA PID with IIF filtered derivative
                    PID_error_third_RA = PID_error_last_RA;
                    PID_error_last_RA = PID_previous_error_RA;
                    PID_error_new_RA = PID_error_RA;
                    PID_der1_RA = PID_der0_RA;
                    PID_der0_RA = A0d_RA * PID_error_new_RA + A1d_RA * PID_error_last_RA + A2d_RA * PID_error_third_RA;
                    fder0_RA = (alpha_RA / (alpha_RA + 1)) * (PID_der0_RA + PID_der1_RA) - ((alpha_RA - 1) / (alpha_RA + 1)) * PID_der1_RA;
                    new_RA_rate_filtder = A0_RA * PID_error_new_RA + A1_RA * PID_error_last_RA + fder0_RA;



                    // standard PID control for DEC
                    PID_error_DEC = ScopeDec - PlateDec;
                    PID_propotional_DEC = PID_error_DEC;
                    PID_integral_RA = PlateDec - PlateDecReference;
                    PID_derivative_DEC = (PID_error_DEC - PID_previous_error_DEC) / (dt_sec);
                    new_DEC_rate = settings.PID_Setting_Kp_DEC * PID_propotional_DEC + settings.PID_Setting_Ki_DEC * PID_integral_DEC + settings.PID_Setting_Kd_DEC * PID_derivative_DEC;


                    log.Debug("PID_propotional_DEC: " + PID_propotional_DEC);
                    log.Debug("PID_integral_DEC: " + PID_integral_DEC);
                    log.Debug("PID_derivative_DEC: " + PID_derivative_DEC);
                    log.Debug("new_DEC_rate: " + new_DEC_rate);

                    // PID control for DEC with IIF filtered derivative - set up dt dependent constants
                    A0_DEC = settings.PID_Setting_Kp_DEC + settings.PID_Setting_Ki_DEC * dt_sec + settings.PID_Setting_Kd_DEC / dt_sec;
                    A1_DEC = -settings.PID_Setting_Kp_DEC;
                    A0d_DEC = settings.PID_Setting_Kd_DEC / dt_sec;
                    A1d_DEC = -2.0 * settings.PID_Setting_Kd_DEC / dt_sec;
                    A2d_DEC = settings.PID_Setting_Kd_DEC / dt_sec;
                    Nfilt_DEC = settings.PID_Setting_Nfilt_DEC;
                    tau_DEC = settings.PID_Setting_Kd_DEC / (settings.PID_Setting_Kp_DEC * Nfilt_DEC);
                    alpha_DEC = dt_sec / (2.0 * tau_DEC);

                    // Perform DEC PID with IIF filtered derivative
                    PID_error_third_DEC = PID_error_last_DEC;
                    PID_error_last_DEC = PID_previous_error_DEC;
                    PID_error_new_DEC = PID_error_DEC;
                    PID_der1_DEC = PID_der0_DEC;
                    PID_der0_DEC = A0d_DEC * PID_error_new_DEC + A1d_DEC * PID_error_last_DEC + A2d_DEC * PID_error_third_DEC;
                    fder0_DEC = (alpha_DEC / (alpha_DEC + 1)) * (PID_der0_DEC + PID_der1_DEC) - ((alpha_DEC - 1) / (alpha_DEC + 1)) * PID_der1_DEC;
                    new_DEC_rate_filtder = A0_DEC * PID_error_new_DEC + A1_DEC * PID_error_last_DEC + fder0_DEC;

                }

                // TODO: Check that the mount is tracking, and if telescope.RARateIsSettable is true. 

                telescope.RightAscensionRate = new_RA_rate;
                telescope.DeclinationRate = new_DEC_rate;



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
    }
        
}
    

