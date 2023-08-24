using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Windows.Forms;

namespace ContraDrift
{
    public class FrameList
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        public FrameList (int BufferMax, FrameList Givesto= null) { PlateCollectionMax = BufferMax; GivesTo = Givesto; }
        private FrameList GivesTo;

        LinkedList<framedata> framelist = new LinkedList<framedata>();

        private int PlateCollectionMax;

        private void DiscardOldest () {
            while (framelist.Count > PlateCollectionMax)
            {
                framedata Oldest = framelist.First.Value;
                framelist.RemoveFirst();
                if (GivesTo != null) { GivesTo.AddPlateCollection(Oldest.PositionRaArcSec, Oldest.PositionDecArcSec, Oldest.StartTime, Oldest.ExposureDuration); }
            }
        }
        public struct framedata {
            public double PositionRaArcSec;
            public double PositionDecArcSec;
            public DateTime StartTime;
            public double ExposureDuration;
            public DateTime EndTime() { return StartTime.AddSeconds(ExposureDuration); }

        }
        public int Count()
        {
            return framelist.Count;
        }

        /*public void SetMaxBufferCount(int frames) {
            PlateCollectionMax = frames;
            DiscardOldest();

        }*/

        public void AddPlateCollection(double RaArcSec, double DecArcSec, DateTime starttime, double exposureduration)
        {
            framelist.AddLast(new framedata()
            {
                PositionRaArcSec = RaArcSec,
                PositionDecArcSec = DecArcSec,
                StartTime = starttime,
                ExposureDuration = exposureduration
            });
            DiscardOldest();
        }
        public bool IsBufferFull()
        {
            log.Debug("FrameListBuffer: " + framelist.Count);
            if (framelist.Count >= (PlateCollectionMax)) { return true; } else { return false; }
        }
        public int PlateCollectionCeiling()
        {
            return PlateCollectionMax;
        }
        public (double, double) GetPlateCollectionAverage()
        {
            DiscardOldest();
            double CollectionRaSumRecentWindow = 0;
            double CollectionDecSumRecentWindow = 0;
            int WindowIndex = 0;
            foreach (framedata data in framelist)
            {
                CollectionRaSumRecentWindow += data.PositionRaArcSec;
                CollectionDecSumRecentWindow += data.PositionDecArcSec;
                WindowIndex++;
            }
            return (
                CollectionRaSumRecentWindow / framelist.Count,
                CollectionDecSumRecentWindow / framelist.Count
                );
        }

        public DateTime GetPlateCollectionLocalExposureTimeCenter() {
            if (framelist.Count == 0) { return (DateTime.Now); }

        
            double HalfSpan = (framelist.Last.Value.EndTime() - framelist.First.Value.StartTime).TotalSeconds / 2;
            return (framelist.First.Value.StartTime.AddSeconds(HalfSpan));

        }

    }
}
