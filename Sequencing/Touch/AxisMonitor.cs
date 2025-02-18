using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSalt.Maestro.Sequencing.Touch
{
    [ExecuteInEditMode]
    public class AxisMonitor : Touch_Module
    {
        public bool axisTransitionActive
        {
            get => touchController.appSettings.GetAxisTransitionActive(this.gameObject, inputGroupKey);
            private set => touchController.appSettings.SetAxisTransitionActive(this.gameObject, inputGroupKey, value);
        }
        
        public float axisTransitionSpread => touchController.appSettings.GetAxisTransitionSpread(this.gameObject, inputGroupKey);

        [SerializeField]
        private TouchExtentsCollection _touchExtentsCollection = new TouchExtentsCollection();

        private TouchExtentsCollection touchExtentsCollection => _touchExtentsCollection;

        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private BranchKey _yNorthKey;

        public BranchKey yNorthKey
        {
            get => _yNorthKey;
            set => _yNorthKey = value;
        }
        
        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private BranchKey _ySouthKey;

        public BranchKey ySouthKey
        {
            get => _ySouthKey;
            set => _ySouthKey = value;
        }

        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private BranchKey _xEastKey;

        public BranchKey xEastKey
        {
            get => _xEastKey;
            set => _xEastKey = value;
        }

        [TitleGroup("Branch Keys")]
        [SerializeField]
        [ReadOnly]
        [Required]
        private BranchKey _xWestKey;

        public BranchKey xWestKey
        {
            get => _xWestKey;
            set => _xWestKey = value;
        }
        
        protected override void Start()
        {
            base.Start();
            axisTransitionActive = false;
        }

        public void SetTransitionStatus(bool targetStatus)
        {
            axisTransitionActive = targetStatus;
        }

//#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            PopulateBranchKeys();
            touchExtentsCollection.Clear();

            for (int i = 0; i < touchController.touchDataList.Count; i++)
            {
                var touchData = touchController.touchDataList[i];
                
                ConfigTrack inputConfigTrack = touchData.inputConfigTrack;
                if (inputConfigTrack == null) continue;
                
                IEnumerable<IMarker> markers = inputConfigTrack.GetMarkers().OrderBy(s => s.time);

                List<TouchExtents> rawExtents = CreateExtentsList(this, touchData, markers);
                
                if (touchExtentsCollection.ContainsKey(touchData.sequence.sequenceController.masterSequence) == false) {
                    touchExtentsCollection.Add(touchData.sequence.sequenceController.masterSequence, rawExtents);
                } else {
                    touchExtentsCollection[touchData.sequence.sequenceController.masterSequence].AddRange(rawExtents);
                }
            }

            foreach (KeyValuePair<MasterSequence, List<TouchExtents>> touchExtentsList in touchExtentsCollection) {
                ConfigureTouchExtents(touchExtentsList.Value);
            }
#if UNITY_EDITOR            
            EditorUtility.SetDirty(this);
#endif
        }

        private void PopulateBranchKeys()
        {
#if UNITY_EDITOR
            if (yNorthKey == null) {
                yNorthKey = Utils.GetScriptableObject(nameof(VarDependencies.yNorthBranch)) as BranchKey;
            }

            if (ySouthKey == null) {
                ySouthKey = Utils.GetScriptableObject(nameof(VarDependencies.ySouthBranch)) as BranchKey;
            }

            if (xEastKey == null) {
                xEastKey = Utils.GetScriptableObject(nameof(VarDependencies.xEastBranch)) as BranchKey;
            }

            if (xWestKey == null) {
                xWestKey = Utils.GetScriptableObject(nameof(VarDependencies.xWestBranch)) as BranchKey;
            }
#endif
        }

        private static List<TouchExtents> CreateExtentsList(AxisMonitor axisMonitor, Touch_Data touchData,
            IEnumerable<IMarker> markers)
        {
            List<TouchExtents> touchExtentsData = new List<TouchExtents>();
            
            foreach (IMarker marker in markers)
            {
                if (marker is AxisMarker axisMarker) {
                    touchExtentsData.Add(new AxisExtents(axisMonitor, touchData, axisMarker));
                }
            }

            Joiner.ForkDataCollection forkDataCollection = axisMonitor.touchController.joiner.forkDataCollection;
            
            if (forkDataCollection.ContainsKey(touchData.sequence) && forkDataCollection[touchData.sequence][0].fork is TouchFork) {
                for (int i = 0; i < forkDataCollection[touchData.sequence].Count; i++) {
                    touchExtentsData.Add(new TouchForkExtents(axisMonitor, touchData, forkDataCollection[touchData.sequence][i]));
                }
            }
            
            touchExtentsData.Sort(new AxisExtentsSort());
            return touchExtentsData;
        }

        private class AxisExtentsSort : Comparer<TouchExtents>
        {
            public override int Compare(TouchExtents x, TouchExtents y)
            {
                
                switch (x) {
                    case AxisExtents x1 when y is AxisExtents y1:
                        return x1.markerMasterTime.CompareTo(y1.markerMasterTime);
                    case AxisExtents x2 when y is TouchForkExtents y2:
                        return x2.markerMasterTime.CompareTo(y2.startTime);
                    case TouchForkExtents x3 when y is AxisExtents y3:
                        return x3.startTime.CompareTo(y3.markerMasterTime);
                    case TouchForkExtents x4 when y is TouchForkExtents y4:
                        return x4.startTime.CompareTo(y4.startTime);
                }
                
                throw new System.Exception("Unable to sort axis extents");
            }
        }

        // Whereas we can populate Fork Extents with all of their data upon creation, we need to
        // know the order of Axis Extents first to set up the transitions correctly.
        // Given an ordered list, we then populate the adjacent extents, which allow us
        // to define start times, end times, and transition times between the axis intervals 
        private static List<TouchExtents> ConfigureTouchExtents(List<TouchExtents> touchExtentsData)
        {
            for (int j = 0; j < touchExtentsData.Count; j++) {

                TouchExtents touchExtents = touchExtentsData[j];
                    
                if (touchExtentsData.Count == 1) {
                    touchExtents.Configure(null, null);
                    break;
                }
                    
                if (j == 0) {
                    touchExtents.Configure(null, touchExtentsData[j + 1]);
                }
                else if (j == touchExtentsData.Count - 1)  {
                    touchExtents.Configure(touchExtentsData[j - 1], null);
                }
                else {
                    touchExtents.Configure( touchExtentsData[j - 1],  touchExtentsData[j + 1]);
                }
                    
            }

            return touchExtentsData;
        }
//#endif
        
        public void RefreshAxes()
        {
            if (Application.isPlaying == false) return;
            
            if (moduleActive == false) {
                return;
            }
            
            foreach (KeyValuePair<MasterSequence, List<TouchExtents>> touchExtentsItem in touchExtentsCollection) {

                if (touchExtentsItem.Key.hasActiveSequence == false) continue;
                
                double masterTime = touchExtentsItem.Key.elapsedTime;

                if (TouchExtents.TimeWithinExtents(masterTime, touchExtentsItem.Value, out var currentExtents)) {
                    
                    if (currentExtents is AxisExtents axisExtents) {
                        AxisUtils.ActivateAxisExtents(masterTime, axisExtents);
                    } else if (currentExtents is TouchForkExtents forkExtents) {
                        TouchForkUtils.ActivateTouchFork(masterTime, forkExtents);
                    }
                    
                }
            }
        }

        public void CallResetBranchStates()
        {
            if (Application.isPlaying == false) return;
            
            if (moduleActive == false) {
                return;
            }
            
            foreach (KeyValuePair<MasterSequence, List<TouchExtents>> touchExtentsItem in touchExtentsCollection) {
                
                double masterTime = touchExtentsItem.Key.elapsedTime;
                
                if (TouchExtents.TimeWithinExtents(masterTime, touchExtentsItem.Value, out var currentExtents)) {
                    
                    if (currentExtents is TouchForkExtents forkExtents) {
                        TouchForkUtils.ResetAllBranches(forkExtents);
                    }
                }
            }
        }

        [Serializable]
        [HideReferenceObjectPicker]
        public class TouchExtentsCollection : SerializableDictionaryBase<MasterSequence, List<TouchExtents>> { }
        

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}