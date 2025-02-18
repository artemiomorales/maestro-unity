﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class ResponsiveAutoScale : ResponsiveRectTransform
    {
        [SerializeField]
        [InfoBox("Automatically sets the X scale to the current scene width, with option to " +
                 "modify resulting value via a multiplier. (Height can also be modified, though height " +
                 "is generally fixed and shouldn't need to be modified)")]
        List<float> widthMultipliers = new List<float>();

        [SerializeField]
        List<float> heightMultipliers = new List<float>();


#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBreakpointDependencies();
        }

        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (widthMultipliers.Count == 0) {
                widthMultipliers.Add(1f);
            }
            if (heightMultipliers.Count == 0) {
                heightMultipliers.Add(1f);
            }
            if (widthMultipliers.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(widthMultipliers, aspectRatioBreakpoints.Count);
            }
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            if(hasBreakpoints == true) {
                SetValue(breakpointIndex);
            } else {
                SetValue(0);
            }
        }

        void SetValue(int activeIndex)
        {
            double width = Utils.GetResponsiveWidth(sceneHeight, sceneWidth);
            float height = Utils.pageHeight * heightMultipliers[activeIndex];

            rectTransform.localScale = new Vector2((float)width * widthMultipliers[activeIndex], height);
        }
    }
}