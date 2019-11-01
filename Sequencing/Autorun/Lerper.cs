using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Autorun
{
    public class Lerper : Autorun_Module
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _frameStepValue;
        
        private IEnumerator coroutine;

        private float frameStepValue
        {
            get => _frameStepValue.Value;
        }
        
        private delegate Autorun_Data CoroutineCallback(Autorun_Controller autorunController, Autorun_Data autorunData, Autorun_Interval interval);

        public void TriggerLerpSequence(Sequence targetSequence)
        {
            if(CanLerpSequence(targetSequence, autorunController, out var autorunData) == false) return;

            if (autorunData.sequence.active == true && autorunData.isLerping == false)
            {
                if (Autorun_Interval.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false) return;

                autorunData.isLerping = true;

                float timeModifier = CalculateLerpModifier(frameStepValue, autorunController.isReversing);

                coroutine = LerpSequence(autorunController, this, autorunData,
                    timeModifier, currentInterval, CheckEndLerp);
                StartCoroutine(coroutine);
            }
        }

        private Autorun_Data CheckEndLerp(Autorun_Controller autorunController, Autorun_Data autorunData, Autorun_Interval interval)
        {
            if (autorunController.isReversing == false)
            {
                if (autorunData.sequence.currentTime > interval.endTime)
                {
                    autorunData.isLerping = false;
                    StopCoroutine(coroutine);
                    TriggerInputActionComplete();
                }
            }
            else
            {
                if (autorunData.sequence.currentTime < interval.startTime) {
                    autorunData.isLerping = false;
                    StopCoroutine(coroutine);
                    TriggerInputActionComplete();
                }    
            }

            return autorunData;
        }
        
        private static bool CanLerpSequence(Sequence targetSequence, Autorun_Controller autorunController, out Autorun_Data autorunData)
        {
            for (int i = 0; i < autorunController.autorunData.Count; i++)
            {
                if (targetSequence == autorunController.autorunData[i].sequence)
                {
                    autorunData = autorunController.autorunData[i];
                    return true;
                }
            }
            
            throw new Exception("Target sequence not found. Did you forget to assign an InputController or MasterController?");
        }


        private static float CalculateLerpModifier(float lerpValue, bool isReversing)
        {
            if (isReversing == true)
            {
                return lerpValue * -1f;
            }

            return lerpValue;
        }

        private static IEnumerator LerpSequence(Autorun_Controller autorunController, Input_Module source,
            Autorun_Data autorunData, float timeModifier, Autorun_Interval currentInterval, CoroutineCallback callback)
        {
            while(true) {

                EventPayload eventPayload = EventPayload.CreateInstance();
                eventPayload.Set(DataType.scriptableObjectType, autorunData.sequence);
                eventPayload.Set(DataType.intType, source.priority);
                eventPayload.Set(DataType.stringType, source.gameObject.name);
                
                if (Application.platform == RuntimePlatform.Android)
                {
                    eventPayload.Set(DataType.floatType, timeModifier * 3f);
                }
                else
                {
                    eventPayload.Set(DataType.floatType, timeModifier);
                }
                
                autorunController.requestModifyToSequence.RaiseEvent(source.gameObject, eventPayload);
                callback(autorunController, autorunData, currentInterval);
                yield return new WaitForEndOfFrame();
            }
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}