using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputGroupKeyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private InputGroupKey _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(InputGroupKey value)
        {
            _variable = value;
        }
        
        
        public InputGroupKeyReference()
        { }

        public InputGroupKeyReference(InputGroupKey value)
        {
            _variable = value;
        }
        
#if UNITY_EDITOR
        protected override bool ShouldPopulateReference()
        {
            if (_variable == null) {
                return true;
            }

            return false;
        }

        protected override ScriptableObject ReadVariable()
        {
            return _variable;
        }  
#endif        
    }
}