using System;
using System.Collections.Generic;

namespace ContraDrift
{
    public class GuidingPidSettings
    {
        public double KpRa;
        public double KiRa;
        public double KdRa;
        public double KpDec;
        public double KiDec;
        public double KdDec;
        public double KpRaFilter;
        public double KiRaFilter;
        public double KdRaFilter;
        public double KpDecFilter;
        public double KiDecFilter;
        public double KdDecFilter;
        public double NfiltRa;
        public double NfiltDec;
        public int BufferFitsCount;
    }

    public class GuidingPidResult
    {
        public double RaRate;
        public double DecRate;
        public double RaP;
        public double RaI;
        public double RaD;
        public double DecP;
        public double DecI;
        public double DecD;
        public bool NeedsPrecalc;
        public int JournalCount;
    }

    /// <summary>
    /// RA/Dec guiding PID with traditional and IIR filtered-derivative modes.
    /// Filter mode uses a 2nd-order IIR on position error and a 1st-order IIR on the derivative term.
    /// </summary>
    public class GuidingPidController
    {
        private readonly LinkedList<double> proportionalJournalRa = new LinkedList<double>();
        private readonly LinkedList<double> proportionalJournalDec = new LinkedList<double>();

        private double errorNewRa;
        private double errorLastRa;
        private double errorThirdRa;
        private double der0Ra;
        private double der1Ra;

        private double errorNewDec;
        private double errorLastDec;
        private double errorThirdDec;
        private double der0Dec;
        private double der1Dec;

        public void Reset()
        {
            proportionalJournalRa.Clear();
            proportionalJournalDec.Clear();
            errorNewRa = errorLastRa = errorThirdRa = 0;
            der0Ra = der1Ra = 0;
            errorNewDec = errorLastDec = errorThirdDec = 0;
            der0Dec = der1Dec = 0;
        }

        public GuidingPidResult Compute(
            double plateRaArcSec,
            double plateDecArcSec,
            double plateRaArcSecOld,
            double plateDecArcSecOld,
            double plateRaReference,
            double plateDecReference,
            double dtSec,
            bool useFilterMode,
            GuidingPidSettings settings)
        {
            double raP = (plateRaArcSec - plateRaArcSecOld) / dtSec;
            proportionalJournalRa.AddLast(raP);
            double raI = plateRaArcSec - plateRaReference;
            double raD = (raP - proportionalJournalRa.First.Value) / dtSec;

            double decP = (plateDecArcSec - plateDecArcSecOld) / dtSec;
            proportionalJournalDec.AddLast(decP);
            double decI = plateDecArcSec - plateDecReference;
            double decD = (decP - proportionalJournalDec.First.Value) / dtSec;

            var result = new GuidingPidResult
            {
                RaP = raP,
                RaI = raI,
                RaD = raD,
                DecP = decP,
                DecI = decI,
                DecD = decD,
                JournalCount = proportionalJournalDec.Count
            };

            if (proportionalJournalDec.Count <= settings.BufferFitsCount)
            {
                result.NeedsPrecalc = true;
                result.RaRate = 0;
                result.DecRate = 0;
                return result;
            }

            if (proportionalJournalRa.Count > settings.BufferFitsCount)
            {
                proportionalJournalRa.RemoveFirst();
            }
            if (proportionalJournalDec.Count > settings.BufferFitsCount)
            {
                proportionalJournalDec.RemoveFirst();
            }

            if (useFilterMode)
            {
                result.RaRate = ComputeFilteredRate(raP, raI, plateRaArcSec, plateRaReference, dtSec,
                    settings.NfiltRa, settings.KpRaFilter, settings.KiRaFilter, settings.KdRaFilter,
                    ref errorNewRa, ref errorLastRa, ref errorThirdRa, ref der0Ra, ref der1Ra);
                result.DecRate = ComputeFilteredRate(decP, decI, plateDecArcSec, plateDecReference, dtSec,
                    settings.NfiltDec, settings.KpDecFilter, settings.KiDecFilter, settings.KdDecFilter,
                    ref errorNewDec, ref errorLastDec, ref errorThirdDec, ref der0Dec, ref der1Dec);
            }
            else
            {
                result.RaRate = settings.KpRa * raP + settings.KiRa * raI + settings.KdRa * raD;
                result.DecRate = settings.KpDec * decP + settings.KiDec * decI + settings.KdDec * decD;
            }

            result.NeedsPrecalc = false;
            return result;
        }

        private static double ComputeFilteredRate(
            double proportional,
            double integral,
            double positionArcSec,
            double referenceArcSec,
            double dtSec,
            double nfilt,
            double kp,
            double ki,
            double kd,
            ref double errorNew,
            ref double errorLast,
            ref double errorThird,
            ref double der0,
            ref double der1)
        {
            errorThird = errorLast;
            errorLast = errorNew;
            errorNew = positionArcSec - referenceArcSec;

            double tau = nfilt * dtSec;
            double alpha = tau <= 0 ? 0 : tau / (tau + dtSec);

            double a0 = (1 - alpha) * (1 - alpha);
            double a1 = 2 * alpha * (1 - alpha);
            double a2 = alpha * alpha;

            double filteredError = a0 * errorNew + a1 * errorLast + a2 * errorThird;

            double rawDerivative = proportional;
            der1 = der0;
            der0 = alpha * der0 + (1 - alpha) * rawDerivative;

            return kp * proportional + ki * filteredError + kd * der0;
        }
    }
}
