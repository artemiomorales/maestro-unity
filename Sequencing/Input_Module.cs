using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class Input_Module : MonoBehaviour
    {
        protected abstract Input_Controller inputController { get; }
        
        protected UserDataKey userKey => inputController.rootConfig.userKey;
        
        protected InputGroupKey inputGroupKey => inputController.rootConfig.inputGroupKey;

        [SerializeField]
        private int _priority;

        public int priority => _priority;

        public bool appUtilsRequested => inputController.appUtilsRequested;

        private ComplexEventManualTrigger inputActionComplete =>
            inputController.appSettings.GetInputActionComplete(this.gameObject, inputController.inputGroupKey);
        

        protected virtual void Start()
        {
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