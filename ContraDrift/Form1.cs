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

        private System.Diagnostics.Stopwatch PID_dt; // delta time from last image, double seconds. 
        private double PID_previous_error_RA = 0;

        private double PID_propotional_RA = 0;
        private double PID_integral_RA = 0;
        private double PID_error_RA = 0;
        private double PID_derivative_RA = 0;

        private double PID_previous_error_DEC = 0;
        private double PID_propotional_DEC = 0;
        private double PID_integral_DEC = 0;
        private double PID_error_DEC = 0;
        private double PID_derivative_DEC = 0;
        private DateTime LastExposure; 



        public Form1()
        {
            InitializeComponent();
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            log = LogManager.GetCurrentClassLogger();

            textBox1.Text = settings.TelescopeProgId;
            textBox2.Text = settings.WatchFolder;
            PID_Setting_Kp_RA.Text = settings.PID_Setting_Kp_RA.ToString();
            PID_Setting_Ki_RA.Text = settings.PID_Setting_Ki_RA.ToString();
            PID_Setting_Kd_RA.Text = settings.PID_Setting_Kd_RA.ToString();
            PID_Setting_Kp_DEC.Text = settings.PID_Setting_Kp_DEC.ToString();
            PID_Setting_Ki_DEC.Text = settings.PID_Setting_Ki_DEC.ToString();
            PID_Setting_Kd_DEC.Text = settings.PID_Setting_Kd_DEC.ToString();

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

        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox2.Text = folderBrowserDialog1.SelectedPath;
            settings.WatchFolder = textBox2.Text;
            settings.Save();



        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (settings.Status == "Running")
            {
                // stop
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
                //Console.WriteLine("alt=" + alt + "\t az=" + az);
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
                Console.WriteLine("Watcher Enabled");
            }
        }

        void worker_backgroundProcess(object sender, DoWorkEventArgs e)
        {
            // for any background processing here... outside of new image processing. 
        }

        void ProcessNewFits(object sender, FileSystemEventArgs e)
        {
            tFactory.StartNew(() => {

            double PlateRa = 0, PlateDec = 0, PlateExposureTime;
            DateTime PlateLocaltime;
            double ScopeRa, ScopeDec;
            double ScopeRaRate, ScopeDecRate;

            string InputFilename = e.FullPath;
            Console.WriteLine("New File: " + InputFilename);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Plate p = new Plate();
            try { p.AttachFITS(InputFilename); } catch (Exception ex) { Console.WriteLine(ex); }

            p.ArcsecPerPixelHoriz = (Convert.ToDouble(p.ReadFITSValue("XPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
            p.ArcsecPerPixelVert = (Convert.ToDouble(p.ReadFITSValue("YPIXSZ")) / Convert.ToDouble(p.ReadFITSValue("FOCALLEN"))) * 206.2648062;
            p.RightAscension = p.TargetRightAscension;
            p.Declination = p.TargetDeclination;
            p.Catalog = (CatalogType)11;
            p.CatalogPath = "J:/UCAC4";

            try
            {
                p.Solve();
                Console.Write(p.RightAscension + ",");
                Console.Write(p.Declination + ",");
                PlateRa = p.RightAscension;
                PlateDec = p.Declination;
                PlateLocaltime = p.ExposureStartTime;
                PlateExposureTime = p.ExposureInterval;
                
            }
            catch (Exception ex) { Console.WriteLine(ex); }


            stopwatch.Stop();
            Console.WriteLine((float)stopwatch.ElapsedMilliseconds / 1000);
            p.DetachFITS();
            _ = Marshal.ReleaseComObject(p); // important or the com object leaks memory

            telescope.SlewToCoordinates(20.7333995798772, 45.3667092514744); //a blocking call for testing; 

            ScopeRa = telescope.RightAscension;
            ScopeDec = telescope.Declination;
            ScopeRaRate = telescope.RightAscensionRate / 15; //  arcsec to RA Sec per sidereal second divide by 15.  
            ScopeDecRate = telescope.DeclinationRate;

            Console.WriteLine("ScopeRa: " + ScopeRa);
            Console.WriteLine("ScopeDec: " + ScopeDec);
            Console.WriteLine("ScopeRaRate: " + ScopeRaRate);
            Console.WriteLine("ScopeDecRate: " + ScopeDecRate);

                // LastExposure = PlateLocaltime.AddSeconds(PlateExposureTime / 2);
                // TODO:  Change how DT is calucated. 



                // PID control for RA
                PID_error_RA = ScopeRa - PlateRa;
                PID_propotional_RA = PID_error_RA;
                PID_integral_RA = PID_integral_RA + PID_error_RA * ((float)PID_dt.ElapsedMilliseconds);
                PID_derivative_RA = (PID_error_RA - PID_previous_error_RA) / ((float)PID_dt.ElapsedMilliseconds);
                double new_RA_rate = settings.PID_Setting_Kp_RA * PID_propotional_RA + settings.PID_Setting_Ki_RA * PID_integral_RA + settings.PID_Setting_Kd_RA * PID_derivative_RA;

                Console.WriteLine("PID_propotional_RA: " + PID_propotional_RA);
                Console.WriteLine("PID_integral_RA: " + PID_integral_RA);
                Console.WriteLine("PID_derivative_RA: " + PID_derivative_RA);
                Console.WriteLine("new_RA_rate: " + new_RA_rate);


                // PID control for DEC
                PID_error_DEC = ScopeDec - PlateDec;
                PID_propotional_DEC = PID_error_DEC;
                PID_integral_DEC = PID_integral_DEC + PID_error_DEC * ((float)PID_dt.ElapsedMilliseconds);
                PID_derivative_DEC = (PID_error_DEC - PID_previous_error_DEC) / ((float)PID_dt.ElapsedMilliseconds);
                double new_DEC_rate = settings.PID_Setting_Kp_DEC * PID_propotional_DEC + settings.PID_Setting_Ki_DEC * PID_integral_DEC + settings.PID_Setting_Kd_DEC * PID_derivative_DEC;


                Console.WriteLine("PID_propotional_DEC: " + PID_propotional_DEC);
                Console.WriteLine("PID_integral_DEC: " + PID_integral_DEC);
                Console.WriteLine("PID_derivative_DEC: " + PID_derivative_DEC);
                Console.WriteLine("new_DEC_rate: " + new_DEC_rate);


                // TODO: Check that the mount is tracking, and if telescope.RARateIsSettable is true. 

                telescope.RightAscensionRate = new_RA_rate;
                telescope.DeclinationRate = new_DEC_rate;

                PID_dt.Restart(); // this restarts the stopwatch so dt has the correct value on the next image. 


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
    

