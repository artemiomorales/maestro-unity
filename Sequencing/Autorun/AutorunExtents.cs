using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [Serializable]
    public class AutorunExtents : Extents
    {
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _isEnd = false;

        public bool isEnd
        {
            get => _isEnd;
            set => _isEnd = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _isVideoSequence = false;
        
        public bool isVideoSequence {
            get => _isVideoSequence;
            set => _isVideoSequence = value;
        }

        public AutorunExtents(double startTime, double endTime) : base(startTime, endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
        }

        public AutorunExtents(double startTime, double endTime, string description) : base(startTime, endTime, description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
            this.description = description;
        }

        public static bool TimeWithinThresholdLowerBoundsInclusiveAscending(double sourceTime, List<AutorunExtents> intervals)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < intervals.Count; q++) {
                if(Mathf.Approximately((float)sourceTime, (float)intervals[q].startTime) ||
                   sourceTime > intervals[q].startTime &&
                   sourceTime < intervals[q].endTime) {
                    withinThreshold = true;
                    break;
                }
            }

            return withinThreshold;
        }
        
        public static bool TimeWithinThresholdUpperBoundsInclusive(double sourceTime, List<AutorunExtents> intervals, out AutorunExtents currentExtents)
        {
            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime > intervals[q].startTime &&
                   sourceTime < intervals[q].endTime || 
                   Mathf.Approximately((float)sourceTime, (float)intervals[q].endTime)) {
                    currentExtents = intervals[q];
                    return true;
                }
            }
            currentExtents = null;
            return false;
        }
        
        public static bool TimeWithinThresholdLowerBoundsInclusiveAscending(double sourceTime, List<AutorunExtents> intervals, out AutorunExtents currentExtents)
        {
            for (int q = 0; q < intervals.Count; q++) {
                if(Mathf.Approximately((float)sourceTime, (float)intervals[q].startTime) ||
                    sourceTime > intervals[q].startTime &&
                   sourceTime < intervals[q].endTime) {
                    currentExtents = intervals[q];
                    return true;
                }
            }
            currentExtents = null;
            return false;
        }
        
        public static bool TimeWithinThresholdLowerBoundsInclusiveDescending(double sourceTime, List<AutorunExtents> intervals, out AutorunExtents currentExtents)
        {
            for (int q = intervals.Count - 1; q >= 0; q--) {
                if(Mathf.Approximately((float)sourceTime, (float)intervals[q].startTime) ||
                   sourceTime > intervals[q].startTime &&
                   sourceTime < intervals[q].endTime) {
                    currentExtents = intervals[q];
                    return true;
                }
            }
            currentExtents = null;
            return false;
        }
        
        public static bool TimeWithinThresholdBothBoundsInclusive(double sourceTime, List<AutorunExtents> intervals, out AutorunExtents currentExtents)
        {
            for (int q = 0; q < intervals.Count; q++) {
                if(Mathf.Approximately((float)sourceTime, (float)intervals[q].startTime) ||
                   sourceTime > intervals[q].startTime &&
                   sourceTime < intervals[q].endTime ||
                   Mathf.Approximately((float)sourceTime, (float)intervals[q].endTime)) {
                    currentExtents = intervals[q];
                    return true;
                }
            }
            currentExtents = null;
            return false;
        }
    }
}