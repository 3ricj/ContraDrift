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
            while (framelist.Count > PlateCollectionMax) { framelist.RemoveFirst(); }

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
            while (framelist.Count > PlateCollectionMax) { framelist.RemoveFirst(); }
        }
        public (double, double) GetPlateCollectionAverage()
        {
            while (framelist.Count > PlateCollectionMax) { framelist.RemoveFirst(); }
            double CollectionRaSum = 0;
            double CollectionDecSum = 0;
            foreach (framedata data in framelist)
            {
                CollectionRaSum += data.PositionRaArcSec;
                CollectionDecSum += data.PositionDecArcSec;
            }
            return (CollectionRaSum / framelist.Count, CollectionDecSum / framelist.Count);
        }

        public DateTime GetPlateCollectionLocalExposureTimeCenter() {
            if (framelist.Count == 0) { return DateTime.Now; }

            double HalfSpan = (framelist.Last.Value.EndTime() - framelist.First.Value.StartTime).TotalSeconds / 2;
            return (framelist.First.Value.StartTime.AddSeconds(HalfSpan));
            
            //return DateTime.Now;

        }

    }
}
