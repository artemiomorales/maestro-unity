using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    public abstract class Input_Module : MonoBehaviour
    {
        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _inputActionComplete;

        private ComplexEventManualTrigger inputActionComplete
        {
            get => _inputActionComplete;
            set => _inputActionComplete = value;
        }

        private void OnEnable()
        {
            _inputActionComplete.PopulateVariable(this, nameof(_inputActionComplete));
        }

        public void TriggerInputActionComplete()
        {
            ComplexPayload complexPayload = ComplexPayload.CreateInstance();
            complexPayload.Set(DataType.stringType, this.gameObject.name);
            inputActionComplete.RaiseEvent(this.gameObject, complexPayload);
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}