﻿using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class LerpVariable
    {
        [SerializeField]
        public ModifiableEditorVariable variableTarget;

        [SerializeField]
        [ShowIf(nameof(FloatPopulated))]
        public float targetFloat;

        [SerializeField]
        [ShowIf(nameof(ColorPopulated))]
        public Color targetColor;

        [SerializeField]
        [ShowIf(nameof(V3Populated))]
        public Vector3 targetV3;

        [SerializeField]
        [ShowIf(nameof(IntPopulated))]
        public int targetInt;

        [SerializeField]
        public float duration = float.NaN;

        public delegate void VariableCallbackDelegate();
        public static VariableCallbackDelegate variableCallbackDelegate = () => { };

        public LerpVariable(ModifiableEditorVariable variableTarget, object targetValue, float duration)
        {
            this.variableTarget = variableTarget;

            if(targetValue is float value) {
                this.targetFloat = value;
            } else if(targetValue is Color) {
                this.targetColor = (Color)targetValue;
            } else if(targetValue is Vector3) {
                this.targetV3 = (Vector3)targetValue;
            } else if(targetValue is int) {
                this.targetInt = (int)targetValue;
            }

            this.duration = duration;
        }

        public ModifiableEditorVariable LerpToTargetValue(GameObject callingObject)
        {
            switch(variableTarget.GetType().Name) {

                case nameof(FloatVariable): {
                        FloatVariable variable = variableTarget as FloatVariable;
                        DOTween.To(() => variable.value, x => variable.SetValue(callingObject, x), targetFloat, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(ColorVariable): {
                        ColorVariable variable = variableTarget as ColorVariable;
                        DOTween.To(() => variable.value, x => variable.SetValue(callingObject, x), targetColor, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(V3Variable): {
                        V3Variable variable = variableTarget as V3Variable;
                        DOTween.To(() => variable.value, x => variable.SetValue(callingObject, x), targetV3, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }

                case nameof(IntVariable): {
                        IntVariable variable = variableTarget as IntVariable;
                        DOTween.To(() => variable.value, x => variable.SetValue(callingObject, x), targetInt, duration).OnComplete(() => {
                            variableCallbackDelegate.Invoke();
                        });
                        break;
                    }
            }

            return variableTarget;
        }

        private bool FloatPopulated()
        {
            if (variableTarget is FloatVariable) {
                return true;
            }

            return false;
        }

        private bool ColorPopulated()
        {
            if (variableTarget is ColorVariable) {
                return true;
            }

            return false;
        }

        private bool V3Populated()
        {
            if (variableTarget is V3Variable) {
                return true;
            }

            return false;
        }

        private bool IntPopulated()
        {
            if (variableTarget is IntVariable) {
                return true;
            }

            return false;
        }
    }

}
