using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using NLog;
using PinPoint;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using nom.tam.fits;
using nom.tam.util;
using System.Runtime.InteropServices.ComTypes;



namespace ContraDrift
{
    public class FitsManager
    {
        public class FitsHeaders
        {
            public bool Solved { get; set; }
            public double PlateRa { get; set; }
            public double PlateDec { get; set; }
            public DateTime LocalTime { get; set; }
            public DateTime DateObs { get; set; }
            public DateTime DateAvg { get; set; }
            public double ExposureTime { get; set; }
            public double Airmass { get; set; }
            public float SolveTime { get; set; }
            public double FitsRa { get; set; }
            public double FitsDec { get; set; }
            public string Filter { get; set; }
            public string Object { get; set; }
            public double ObjectRa { get; set; }
            public double ObjectDec { get; set; }
            public string ImageType { get; set; }
            public int PixelsWide { get; set; }
            public int PixelsHigh { get; set; }
            public int XBinning { get; set; }
            public int YBinning { get; set; }
            public int Gain { get; set; }
            public int Offset { get; set; }
            public double PitchWide { get; set; }
            public double PitchHigh { get; set; }
            public string Instrument { get; set; }
            public double CameraTempSetPoint { get; set; }
            public double CameraTempCurrent { get; set; }
            public string ReadoutMode { get; set; }
            public double FocalLength { get; set; }
            public double AltitudeTelescope { get; set; }
            public double AzimuthTelescope { get; set; }
            public double PeirSide { get; set; }
        }

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        
        // Convert RA in 'H M S' format to degrees
        public static double ConvertRAtoDegrees(string ra)
        {
            string[] raParts = ra.Split(' ');
            int hours = int.Parse(raParts[0]);
            int minutes = int.Parse(raParts[1]);
            int seconds = int.Parse(raParts[2]);

            // Convert RA to degrees: (hours + minutes/60 + seconds/3600) * 15
            double raDegrees = (hours + (minutes / 60.0) + (seconds / 3600.0)) * 15.0;
            return raDegrees;
        }

        // Convert Dec in 'D M S' format to degrees
        public static double ConvertDECtoDegrees(string dec)
        {
            string[] decParts = dec.Split(' ');
            int degrees = int.Parse(decParts[0], NumberStyles.AllowLeadingSign);
            int minutes = int.Parse(decParts[1]);
            int seconds = int.Parse(decParts[2]);

            // Convert Dec to degrees: degrees + minutes/60 + seconds/3600
            double decDegrees = Math.Abs(degrees) + (minutes / 60.0) + (seconds / 3600.0);

            // Apply the sign of degrees to the result
            if (degrees < 0)
            {
                decDegrees *= -1;
            }

            return decDegrees;
        }

        public static void WaitForFileAccess(string filePath, int retryInterval = 1000, int maxRetries = 10)
        {
            int retries = 0;
            while (true)
            {
                try
                {
                    // Try to open the file for reading (shared mode)
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        // If successful, the file is ready to be accessed
                        break;
                    }
                }
                catch (IOException ex)
                {
                    // If the file is locked by another process, catch the IOException
                    if (IsFileLocked(ex))
                    {
                        retries++;
                        if (retries > maxRetries)
                        {
                            throw new Exception($"File {filePath} is still locked after {maxRetries} retries.");
                        }
                        Console.WriteLine($"File is locked. Retrying in {retryInterval}ms... (Attempt {retries}/{maxRetries})");
                        Thread.Sleep(retryInterval);  // Wait before retrying
                    }
                    else
                    {
                        // If another IOException occurs, rethrow the exception
                        throw;
                    }
                }
            }
        }
        // Helper method to check if the file is locked by another process
        private static bool IsFileLocked(IOException exception)
        {
            int errorCode = System.Runtime.InteropServices.Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33; // ERROR_SHARING_VIOLATION or ERROR_LOCK_VIOLATION
        }

        // ReadFitsHeader method to extract key values from the FITS header file
        public FitsHeaders ReadFitsHeader(string inputFilename)
        {
            FitsHeaders newheader = new FitsHeaders();
            float timeOffset = 0;

            WaitForFileAccess(inputFilename);

            Fits fits = null;
            BufferedFile bf = null;

            // Check if the file is a valid FITS file
            if (Path.GetExtension(inputFilename) != ".fitscsv" && Path.GetExtension(inputFilename) != ".fits")
            {
                log.Debug("Other file detected, not processed: " + inputFilename);
                return (newheader); // (false, 0, 0, DateTime.Now, 0, 0, 0, 0, 0, filter);
            }

            // Handle .fitscsv files (assumed to be CSV format)
            if (Path.GetExtension(inputFilename) == ".fitscsv")
            {
                try
                {
                    using (TextFieldParser csvParser = new TextFieldParser(inputFilename))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        // Skip the row with the column names
                        csvParser.ReadLine();

                        // Process the CSV data
                        while (!csvParser.EndOfData)
                        {
                            string[] fields = csvParser.ReadFields();
                            timeOffset = float.Parse(fields[0]);
                            newheader.PlateRa = double.Parse(fields[1]) / 15;  // Convert degrees into hour angles
                            newheader.PlateDec = double.Parse(fields[2]);
                        }
                    }

                    log.Debug($"Parsing fitscsv: {inputFilename}, timeOffset: {timeOffset}, PlateRa: {newheader.PlateRa}, PlateDec: {newheader.PlateDec}");
                    return (newheader); // true, plateRa, plateDec, DateTime.Now, 0, 0, 0, 0, 0, filter);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error processing FITS CSV file");
                    return (newheader); // (false, 0, 0, DateTime.Now, 0, 0, 0, 0, 0, filter);
                }
            }

            // Handle .fits files (process as FITS header)
            try
            {
                // Check if the file exists before proceeding
                if (!File.Exists(inputFilename))
                {
                    log.Error($"FITS file not found: {inputFilename}");
                    return (newheader); // (false, 0, 0, DateTime.Now, 0, 0, 0, 0, 0, filter);
                }

                // Open the FITS file using CSharpFITS
                log.Info($"Opening FITS file: {inputFilename}");

                bf = new BufferedFile(inputFilename);
                fits = new Fits(bf);

                // Read the HDU (Header Data Unit) from the FITS file
                BasicHDU hdu = fits.ReadHDU();
                if (hdu == null)
                {
                    log.Error("No HDU found in FITS file.");
                    return (newheader);
                }

                Header header = hdu.Header;

                // Extract necessary header values

                // Read the RA from the header (CRVAL1)
                if (header.ContainsKey("CRVAL1"))
                {
                    newheader.FitsRa = header.GetDoubleValue("CRVAL1", 0.0) / 15.0; // Convert to hours
                }

                // Read the Dec from the header (CRVAL2)
                if (header.ContainsKey("CRVAL2"))
                {
                    newheader.FitsDec = header.GetDoubleValue("CRVAL2", 0.0);
                }

                // Exposure time (EXPTIME)
                if (header.ContainsKey("EXPTIME"))
                {
                    newheader.ExposureTime = header.GetDoubleValue("EXPTIME", 0.0);
                }

                // Date and Time of Observation (DATE-OBS)
                if (header.ContainsKey("DATE-LOC"))
                {
                    string dateLoc = header.GetStringValue("DATE-LOC");
                    if (DateTime.TryParse(dateLoc, out DateTime observationTime))
                    {
                        //                        newheader.LocalTime = observationTime.ToLocalTime();
                        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

                        // Convert UTC to local time zone.
                        DateTimeOffset localTime = TimeZoneInfo.ConvertTime(observationTime, localTimeZone);

                        newheader.LocalTime = localTime.DateTime;
                    }
                }
                // Date and Time of Observation (DATE-OBS)

                // Date and Time of Observation (DATE-OBS)
                if (header.ContainsKey("DATE-OBS"))
                {
                    string dateObs = header.GetStringValue("DATE-OBS");
                    if (DateTime.TryParse(dateObs, out DateTime observationTime))
                    {
                        newheader.DateObs = observationTime; // utcTime.DateTime;
                    }
                }

                if (header.ContainsKey("DATE-AVG"))
                {
                    string dateObs = header.GetStringValue("DATE-AVG");
                    if (DateTime.TryParse(dateObs, out DateTime observationTime))
                    {
                        newheader.DateAvg = observationTime;
                    }
                }


                // Airmass (AIRMASS)
                if (header.ContainsKey("AIRMASS"))
                {
                    newheader.Airmass = header.GetDoubleValue("AIRMASS", 1.0); // Default to 1.0 if not found
                }

                // Read filter information (FILTER)
                if (header.ContainsKey("FILTER"))
                {
                    newheader.Filter = header.GetStringValue("FILTER");//, "Unknown");
                }
                if (header.ContainsKey("DEC"))
                {
                    newheader.FitsDec = header.GetDoubleValue("DEC", 0);
                }
                if (header.ContainsKey("RA"))
                {
                    newheader.FitsRa = header.GetDoubleValue("RA", 0);
                }
                if (header.ContainsKey("OBJECT"))
                {
                    newheader.Object = header.GetStringValue("OBJECT");
                }
                if (header.ContainsKey("OBJCTRA"))
                {
                    newheader.ObjectRa = ConvertRAtoDegrees(header.GetStringValue("OBJCTRA"));
                }
                if (header.ContainsKey("OBJCTDEC"))
                {
                    newheader.ObjectDec = ConvertDECtoDegrees(header.GetStringValue("OBJCTDEC"));
                }


                // Log the extracted header information
                log.Info($"FITS Header Parsed: RA: {newheader.FitsRa}, DEC: {newheader.FitsDec}, ExposureTime: {newheader.ExposureTime}, Airmass: {newheader.Airmass}, Filter: {newheader.Filter}");

                // Properly close and clean up resources
                header = hdu.Header;

                return (newheader); // (true, fitsRa, fitsDec, plateLocaltime, plateExposureTime, airmass, solvetime, fitsRa, fitsDec, filter);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error processing FITS header");
                return (newheader);
            }

            return (newheader); // (solved, plateRa, plateDec, plateLocaltime, plateExposureTime, airmass, solvetime, fitsRa, fitsDec, filter);
        }

        public async Task<(bool Solved, double PlateRa, double PlateDec, float Solvetime)> PlateSolveAllAsync(string filename, CancellationToken cancellationToken)
        //       static async Task PlateSolveAllAsync(string filename, CancellationToken cancellationToken)
        {
            CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Define solve tasks for the current directory
            var solveTask1 = Task.Run(() => PlateSolveAstapAsync(filename, linkedCts.Token));
            var solveTask2 = Task.Run(() => PlateSolvePinPointWrapperpAsync(filename, linkedCts.Token));

            // Check if the file exists in the parent directory and create 2 additional tasks if it does
            var parentDirectory = Directory.GetParent(Directory.GetParent(Path.GetFullPath(filename)).ToString()).ToString();
            var parentFilePath = Path.Combine(parentDirectory, Path.GetFileName(filename));

            Task<(bool Solved, double PlateRa, double PlateDec, float Solvetime)> solveTask3 = null;
            Task<(bool Solved, double PlateRa, double PlateDec, float Solvetime)> solveTask4 = null;

            if (File.Exists(parentFilePath))
            {
                log.Info($"File found in parent directory: {parentFilePath}");
                solveTask3 = Task.Run(() => PlateSolveAstapAsync(parentFilePath, linkedCts.Token));
                solveTask4 = Task.Run(() => PlateSolvePinPointWrapperpAsync(parentFilePath, linkedCts.Token));
            }

            // Await the first completed task
            var tasks = new[] { solveTask1, solveTask2, solveTask3, solveTask4 }.Where(t => t != null).ToArray();

            var firstCompleted = await Task.WhenAny(tasks);

            // Get the result of the first completed task
            var firstResult = await firstCompleted;

            // Check if the first completed task solved the plate
            if (firstResult.Solved)
            {
                log.Info("Plate solved. Cancelling other tasks.");
                linkedCts.Cancel();  // Cancel the other tasks if solved
                return firstResult;  // Return the result of the first successful solve
            }
            else
            {
                log.Info("First task did not solve. Waiting for other tasks.");
                // Await all remaining tasks
                var remainingTasks = tasks.Except(new[] { firstCompleted }).ToArray();
                foreach (var remainingTask in remainingTasks)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var result = await remainingTask;
                        if (result.Solved)
                        {
                            log.Info("Another task solved the plate.");
                            linkedCts.Cancel();  // Cancel remaining tasks
                            return result;  // Return the result of the successful solve
                        }
                    }
                }
            }

            // Dispose of the cancellation token source
            linkedCts.Dispose();
            return (false, 0.0, 0.0, 0.0f);
        }


        // PlateSolveAstapAsync method with timeout and cancellation support
        public async Task<(bool Solved, double PlateRa, double PlateDec, float Solvetime)>
            PlateSolvePinPointWrapperpAsync(string inputFilename, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                double plateRa = 0, plateDec = 0, fitsRa = 0, fitsDec = 0, plateExposureTime = 0, airmass = 0;
                DateTime plateLocaltime = DateTime.Now;
                bool solved = false;
                Stopwatch stopwatch = new Stopwatch();
                string vppwExePath = @"C:\VisualPinPointWrapper\VisualPinPointWrapper.exe"; // Update to the actual ASTAP CLI path on your system
                string outputFilename = Path.Combine(Path.GetDirectoryName(inputFilename), Path.GetFileNameWithoutExtension(inputFilename) + ".vppwini");

                try
                {
                    // Check if the ASTAP executable exists
                    if (!File.Exists(vppwExePath))
                    {
                        log.Error("VisualPinPointWrapper executable not found at: " + vppwExePath);
                        return (false, 0, 0, 0);
                    }

                    // Prepare process start info
                    ProcessStartInfo pInfo = new ProcessStartInfo
                    {
                        FileName = vppwExePath,
                        Arguments = $"-f \"{inputFilename}\"",  // Add any other necessary ASTAP arguments here
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Start the ASTAP process
                    log.Info($"Starting VisualPinPointWrapper plate solve for: {inputFilename}");
                    stopwatch.Restart();
                    using (var process = new Process { StartInfo = pInfo })
                    {
                        process.Start();

                        // Create a task to wait for the process exit
                        Task processWaitTask = Task.Run(() => process.WaitForExit());

                        // Wait for the process or timeout/cancellation
                        if (await Task.WhenAny(processWaitTask, Task.Delay(60000, cancellationToken)) == processWaitTask)
                        {
                            // Process completed in time
                            if (process.ExitCode != 0)
                            {
                                log.Error($"VisualPinPointWrapper plate solve failed. Exit Code: {process.ExitCode}. Error: {await process.StandardError.ReadToEndAsync()}");
                                return (false, 0, 0, 0);
                            }
                        }
                        else
                        {
                            // Timeout or cancellation
                            log.Warn("VisualPinPointWrapper plate solve timed out or was canceled.");
                            if (!process.HasExited)
                            {
                                process.Kill(); // Kill the process if it didn't exit within the timeout
                            }

                            return (false, 0, 0, 0);
                        }

                        // Parse the VisualPinPointWrapper output file (.ini)
                        if (File.Exists(outputFilename))
                        {
                            var dict = File.ReadLines(outputFilename)
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .Select(line => line.Split(new char[] { '=' }, 2))
                                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                            if (dict.ContainsKey("PLTSOLVD") && dict["PLTSOLVD"] == "T")
                            {
                                solved = true;
                                if (dict.TryGetValue("CRVAL1", out var raValue))
                                    plateRa = double.Parse(raValue, CultureInfo.InvariantCulture) / 15;  // In hours
                                if (dict.TryGetValue("CRVAL2", out var decValue))
                                    plateDec = double.Parse(decValue, CultureInfo.InvariantCulture);  // In degrees
                            }
                            else
                            {
                                log.Error("VisualPinPointWrapper plate solve failed, no solution found.");
                                return (false, 0, 0, 0);
                            }

                            // Read additional data from the FITS file, like RA, DEC, exposure time, and airmass
                            // Assuming you have logic elsewhere to read RA/DEC and other fields from the FITS file
                            // For demonstration purposes:
                            // You can integrate this part with the same FITS reading method as in PinPoint if needed

                            fitsRa = plateRa;  // Example: using the same RA as result
                            fitsDec = plateDec; // Example: using the same DEC as result
                            plateLocaltime = DateTime.Now;  // Placeholder, replace with actual FITS reading logic
                            plateExposureTime = 10;  // Placeholder, replace with actual FITS reading logic
                            airmass = 1.0;  // Placeholder, replace with actual FITS reading logic
                            log.Info("VisualPinPointWrapper_solvetime=" + stopwatch.Elapsed);

                            log.Info($"VisualPinPointWrapper plate solve succeeded. RA: {plateRa}, DEC: {plateDec}, ExposureTime: {plateExposureTime}, Airmass: {airmass}");
                        }
                        else
                        {
                            log.Error($"VisualPinPointWrapper output file not found: {outputFilename}");
                            return (false, 0, 0, 0);
                        }
                    }

                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error during VisualPinPointWrapper plate solving.");
                    return (false, 0, 0, 0);
                }

                return (solved, plateRa, plateDec, (float)stopwatch.ElapsedMilliseconds / 1000);
            }, cancellationToken);
        }

        // PlateSolveAstapAsync method with timeout and cancellation support
        public async Task<(bool Solved, double PlateRa, double PlateDec, float Solvetime)>
            PlateSolveAstapAsync(string inputFilename, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                double plateRa = 0, plateDec = 0, fitsRa = 0, fitsDec = 0, plateExposureTime = 0, airmass = 0;
                DateTime plateLocaltime = DateTime.Now;
                bool solved = false;
                Stopwatch stopwatch = new Stopwatch();
                string astapExePath = @"C:\Program Files\astap\astap_cli.exe"; // Update to the actual ASTAP CLI path on your system
                string outputFilename = Path.Combine(Path.GetDirectoryName(inputFilename), Path.GetFileNameWithoutExtension(inputFilename) + ".ini");


                try
                {

                    // Check if the ASTAP executable exists
                    if (!File.Exists(astapExePath))
                    {
                        log.Error("ASTAP executable not found at: " + astapExePath);
                        return (false, 0, 0, 0);
                    }

                    // Prepare process start info
                    ProcessStartInfo pInfo = new ProcessStartInfo
                    {
                        FileName = astapExePath,
                        Arguments = $"-f \"{inputFilename}\"",  // Add any other necessary ASTAP arguments here
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Start the ASTAP process
                    log.Info($"Starting ASTAP plate solve for: {inputFilename}");
                    stopwatch.Restart();
                    //Thread.Sleep(1000);
                    using (var process = new Process { StartInfo = pInfo })
                    {
                        process.Start();

                        // Create a task to wait for the process exit
                        Task processWaitTask = Task.Run(() => process.WaitForExit());

                        // Wait for the process or timeout/cancellation
                        if (await Task.WhenAny(processWaitTask, Task.Delay(60000, cancellationToken)) == processWaitTask)
                        {
                            // Process completed in time
                            if (process.ExitCode != 0)
                            {
                                log.Error($"ASTAP plate solve failed. Exit Code: {process.ExitCode}. Error: {await process.StandardError.ReadToEndAsync()}");
                                return (false, 0, 0, 0);
                            }
                        }
                        else
                        {
                            // Timeout or cancellation
                            log.Warn("ASTAP plate solve timed out or was canceled.");
                            if (!process.HasExited)
                            {
                                process.Kill(); // Kill the process if it didn't exit within the timeout
                            }

                            return (false, 0, 0, 0);
                        }

                        // Parse the ASTAP output file (.ini)
                        if (File.Exists(outputFilename))
                        {
                            var dict = File.ReadLines(outputFilename)
                                .Where(line => !string.IsNullOrWhiteSpace(line))
                                .Select(line => line.Split(new char[] { '=' }, 2))
                                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                            if (dict.ContainsKey("PLTSOLVD") && dict["PLTSOLVD"] == "T")
                            {
                                solved = true;
                                if (dict.TryGetValue("CRVAL1", out var raValue))
                                    plateRa = double.Parse(raValue, CultureInfo.InvariantCulture) / 15;  // In hours
                                if (dict.TryGetValue("CRVAL2", out var decValue))
                                    plateDec = double.Parse(decValue, CultureInfo.InvariantCulture);  // In degrees
                            }
                            else
                            {
                                log.Error("ASTAP plate solve failed, no solution found.");
                                return (false, 0, 0, 0);
                            }

                            // Read additional data from the FITS file, like RA, DEC, exposure time, and airmass
                            // Assuming you have logic elsewhere to read RA/DEC and other fields from the FITS file
                            // For demonstration purposes:
                            // You can integrate this part with the same FITS reading method as in PinPoint if needed

                            fitsRa = plateRa;  // Example: using the same RA as result
                            fitsDec = plateDec; // Example: using the same DEC as result
                            plateLocaltime = DateTime.Now;  // Placeholder, replace with actual FITS reading logic
                            plateExposureTime = 10;  // Placeholder, replace with actual FITS reading logic
                            airmass = 1.0;  // Placeholder, replace with actual FITS reading logic
                            log.Info("astap_solvetime=" + stopwatch.Elapsed);

                            log.Info($"ASTAP plate solve succeeded. RA: {plateRa}, DEC: {plateDec}, ExposureTime: {plateExposureTime}, Airmass: {airmass}");
                        }
                        else
                        {
                            log.Error($"ASTAP output file not found: {outputFilename}");
                            return (false, 0, 0, 0);
                        }
                    }

                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error during ASTAP plate solving.");
                    return (false, 0, 0, 0);
                }

                return (solved, plateRa, plateDec, (float)stopwatch.ElapsedMilliseconds / 1000);
            }, cancellationToken);
        }
    }
}

