using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContraDrift
{
    public class FrameList
    {

        public FrameList (int BufferMax) { PlateCollectionMax = BufferMax; }

        LinkedList<framedata> framelist = new LinkedList<framedata>();
        private int PlateCollectionMax;

        public struct framedata {
            public double PositionRaArcSec;
            public double PositionDecArcSec;
            public DateTime StartTime;
            public double ExposureDuration;
            public DateTime EndTime() { return StartTime.AddSeconds(ExposureDuration); }

        }

        public void SetMaxBufferCount(int frames) {
            PlateCollectionMax = frames;
            while (framelist.Count > PlateCollectionMax*2) { framelist.RemoveFirst(); }

        }
        public void AddPlateCollection(double RaArcSec, double DecArcSec, DateTime starttime, double exposureduration)
        {
            framelist.AddLast(new framedata()
            {
                PositionRaArcSec = RaArcSec,
                PositionDecArcSec = DecArcSec,
                StartTime = starttime,
                ExposureDuration = exposureduration
            });
            while (framelist.Count > PlateCollectionMax*2) { framelist.RemoveFirst(); }
        }
        public bool IsBufferFull()
        {
            Log.Debug("FrameListBuffer: " + framelist.Count);
            if (framelist.Count >= (PlateCollectionMax * 2)) { return true; } else { return false; }
        }
        public (double, double, double, double) GetPlateCollectionAverage()
        {
            while (framelist.Count > PlateCollectionMax*2) { framelist.RemoveFirst(); }
            double CollectionRaSumRecentWindow = 0;
            double CollectionRaSumOldWindow = 0;
            double CollectionDecSumRecentWindow = 0;
            double CollectionDecSumOldWindow = 0;
            int WindowIndex = 0;
            foreach (framedata data in framelist)
            {
                if (WindowIndex < PlateCollectionMax)
                {
                    CollectionRaSumOldWindow += data.PositionRaArcSec;
                    CollectionDecSumOldWindow += data.PositionDecArcSec;

                } else
                {
                    CollectionRaSumRecentWindow += data.PositionRaArcSec;
                    CollectionDecSumRecentWindow += data.PositionDecArcSec;

                }
                WindowIndex++;
            }
            return (
                CollectionRaSumOldWindow / PlateCollectionMax,
                CollectionDecSumOldWindow / PlateCollectionMax,
                CollectionRaSumRecentWindow / PlateCollectionMax,
                CollectionDecSumRecentWindow / PlateCollectionMax
                );
        }

        public (DateTime, DateTime) GetPlateCollectionLocalExposureTimeCenter() {
            if (framelist.Count == 0) { return (DateTime.Now, DateTime.Now); }

            double OldWindowHalfSpan = (framelist.ElementAt(PlateCollectionMax).EndTime() -  framelist.First.Value.StartTime).TotalSeconds / 2;
        
            double HalfSpan = (framelist.Last.Value.EndTime() - framelist.ElementAt(PlateCollectionMax).StartTime).TotalSeconds / 2;
            return (framelist.First.Value.StartTime.AddSeconds(OldWindowHalfSpan), 
                framelist.ElementAt(PlateCollectionMax).StartTime.AddSeconds(HalfSpan));
            
            //return DateTime.Now;

        }

    }
}
