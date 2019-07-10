﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class SequenceTouchApplier : SequenceController
    {

        [Required]
        public SimpleEventTrigger momentumApplied;

        [ValidateInput("IsPopulated")]
        public FloatReference swipeModifier;

        [ValidateInput("IsPopulated")]
        public FloatReference momentumModifier;

        // Swipe variables
		[Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis xSwipeAxis;

		[Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis ySwipeAxis;

		[Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis zSwipeAxis;

		[Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis transitionAxis;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
		public V3Reference swipeForce;

        // Momentum variables
		[Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis xMomentumAxis;
		
        [Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis yMomentumAxis;

		[Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis zMomentumAxis;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public V3Reference momentumForce;

        [SerializeField]
        [Required]
        [BoxGroup("Android dependencies")]
        SimpleEventTrigger pauseSequenceComplete;

        public void UpdateSequenceWithSwipe()
        {
            for(int q=0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {

                    if(sequenceLists[q].sequences[i].Active == true) {

                        swipeModifier.Variable.SetValue(0);

					    if (ySwipeAxis.Active) {
						    //Debug.Log("y axis");
						    //Debug.Log(swipeForce.y);
                            swipeModifier.Variable.SetValue(swipeModifier.Value + swipeForce.Variable.Value.y);
					    }
					    if (xSwipeAxis.Active) {
                            //Debug.Log("x axis");
                            //Debug.Log(swipeForce.x);
                            swipeModifier.Variable.SetValue(swipeModifier.Value + swipeForce.Variable.Value.x);
                        }
					    if (zSwipeAxis.Active) {
                            //Debug.Log("z axis");
                            //Debug.Log(swipeForce.z);
                            swipeModifier.Variable.SetValue(swipeModifier.Value + swipeForce.Variable.Value.z);
                        }

                        if (swipeModifier > 0) {
                            isReversing.Variable.SetValue(false);
                        } else if (swipeModifier < 0) {
                            isReversing.Variable.SetValue(true);
                        }

                        if(sequenceLists[q].sequences[i].Invert == true) {
                            swipeModifier.Variable.SetValue(swipeModifier.Value * -1f);
                        }

                        if (sequenceLists[q].sequences[i].ForceForward == true) {
                            swipeModifier.Variable.SetValue(Mathf.Abs(swipeModifier.Value));
                        } else if (sequenceLists[q].sequences[i].ForceBackward == true) {
                            swipeModifier.Variable.SetValue(Mathf.Abs(swipeModifier) * -1f);
                        }

#if UNITY_ANDROID
                        if (sequenceList.sequences[i].VideoSequenceActive == true) {
                            if (internalIsReversingVal != isReversing.Value) {
                                internalIsReversingVal = isReversing.Value;
                                StartCoroutine(PauseSequence(sequenceList.sequences[i], swipeModifier));
                            } else {
                                sequenceList.sequences[i].ModifySequenceTime(swipeModifier);
                                sequenceModified.RaiseEvent(this.gameObject);
                            }
                        } else {
                            sequenceList.sequences[i].ModifySequenceTime(swipeModifier);
                            sequenceModified.RaiseEvent(this.gameObject);
                        }
#else
                        sequenceLists[q].sequences[i].ModifySequenceTime(swipeModifier);
                        sequenceModified.RaiseEvent(this.gameObject);
#endif

                    }
                }
            }
        }

        public void UpdateSequenceWithMomentum()
        {
            for (int q = 0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {

                    if (sequenceLists[q].sequences[i].Active == true && sequenceLists[q].sequences[i].pauseMomentumActive == false || appSettings.pauseMomentum.Value == false) {
#if UNITY_ANDROID
                        if(sequenceList.sequences[i].MomentumDisabled == true) {
                            return;
                        }
#endif
                        momentumModifier.Variable.SetValue(0);

                        if (yMomentumAxis.Active) {
                            //Debug.Log("y axis");
                            //Debug.Log(swipeForce.y);
                            momentumModifier.Variable.SetValue(momentumModifier.Value + momentumForce.Variable.Value.y);
                        }
                        if (xMomentumAxis.Active) {
                            //Debug.Log("x axis");
                            //Debug.Log(swipeForce.x);
                            momentumModifier.Variable.SetValue(momentumModifier.Value + momentumForce.Variable.Value.x);
                        }
                        if (zMomentumAxis.Active) {
                            //Debug.Log("z axis");
                            //Debug.Log(swipeForce.z);
                            momentumModifier.Variable.SetValue(momentumModifier.Value + momentumForce.Variable.Value.z);
                        }

                        if (sequenceLists[q].sequences[i].Invert == true) {
                            momentumModifier.Variable.SetValue(momentumModifier.Value * -1f);
                        }

                        if (sequenceLists[q].sequences[i].ForceForward == true) {
                            momentumModifier.Variable.SetValue(Mathf.Abs(momentumModifier.Value));
                        } else if (sequenceLists[q].sequences[i].ForceBackward == true) {
                            momentumModifier.Variable.SetValue(Mathf.Abs(momentumModifier) * -1f);
                        }

                        sequenceLists[q].sequences[i].ModifySequenceTime(momentumModifier);
                        sequenceModified.RaiseEvent(this.gameObject);
                    }
                }
            }
            momentumApplied.RaiseEvent(this.gameObject);
        }

        public void RefreshSequenceAttributes()
        {
            foreach (SequenceList sequenceList in sequenceLists) {

                for (int i = 0; i < sequenceList.sequences.Count; i++) {
                    if (sequenceList.sequences[i].Active == true && sequenceList.sequences[i].hasPauseMomentum == true) {

                        RefreshVideoSequenceSwitches(sequenceList.sequences[i]);
                        RefreshPauseMomentumSwitches(sequenceList.sequences[i]);
                    }
                }
            }
        }

        void RefreshVideoSequenceSwitches(Sequence targetSequence)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinVideoThreshold = false;

            for (int q = 0; q < targetSequence.autoplayThresholds.Count; q++) {
                if (targetSequence.currentTime >= targetSequence.autoplayThresholds[q].startTime &&
                  targetSequence.currentTime <= targetSequence.autoplayThresholds[q].endTime &&
                    targetSequence.autoplayThresholds[q].isVideoSequence == true) {
                    withinVideoThreshold = true;
                    break;
                }
            }

            if (withinVideoThreshold == true) {
                targetSequence.VideoSequenceActive = true;
            } else {
                targetSequence.VideoSequenceActive = false;
            }
        }

        void RefreshPauseMomentumSwitches(Sequence targetSequence)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < targetSequence.pauseMomentumThresholds.Count; q++) {
                if(targetSequence.currentTime >= targetSequence.pauseMomentumThresholds[q].startTime &&
                  targetSequence.currentTime <= targetSequence.pauseMomentumThresholds[q].endTime) {
                    withinThreshold = true;
                    break;
                }
            }

            if(withinThreshold == true) {
                targetSequence.pauseMomentumActive = true;
            } else {
                targetSequence.pauseMomentumActive = false;
            }
        }

#if UNITY_ANDROID
        // On Android devices, we need to briefly pause the sequence to allow the device
        // to catch up when in a video sequence and abruptly changing directions.
        protected IEnumerator PauseSequence(Sequence targetSequence, float modifier)
        {
            targetSequence.Active = false;
            targetSequence.MomentumDisabled = true;
            triggerSpinnerShow.RaiseEvent(this.gameObject);
            yield return new WaitForSeconds(.5f);
            targetSequence.Active = true;
            pauseSequenceComplete.RaiseEvent(this.gameObject);
            yield return new WaitForSeconds(.5f);
            triggerSpinnerHide.RaiseEvent(this.gameObject);
            yield return new WaitForSeconds(1f);
            targetSequence.MomentumDisabled = false;
        }
#endif

        private static bool IsPopulated(V3Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<Sequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
