using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContraDrift
{
    class SettingsManager
    {
        static ContraDrift.Properties.Settings settings = Properties.Settings.Default;

        public struct SettingsData
        {

            String TelescopeProgId;
            String WatchFolder;
            String UCAC4_path;



            Double PID_Setting_Kp_RA_filter;
            Double PID_Setting_Ki_RA_filter;
            Double PID_Setting_Kd_RA_filter;
            Double PID_Setting_Nfilt_RA;
            Double PID_Setting_Kp_DEC_filter;
            Double PID_Setting_Ki_DEC_filter;
            Double PID_Setting_Kd_DEC_filter;
            Double PID_Setting_Nfilt_DEC;

            Double PID_Setting_Kp_RA;
            Double PID_Setting_Ki_RA;
            Double PID_Setting_Kd_RA;
            Double PID_Setting_Kp_DEC;
            Double PID_Setting_Ki_DEC;
            Double PID_Setting_Kd_DEC;

            Double RaRateLimitSetting;
            Double DecRateLimitSetting;
            int BufferFitsCount;

        }
        public void SaveSettings()
        {
            settings.Save();

        }
    }
}
