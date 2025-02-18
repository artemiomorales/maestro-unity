﻿using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V3Reference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Vector3 _constantValue;

        public Vector3 constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }
        
        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private V3Variable _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(V3Variable value)
        {
            _variable = value;
        }

        public V3Reference()
        { }

        public V3Reference(Vector3 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector3 GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as V3Variable).value;
        }
        
#if UNITY_EDITOR
        protected override bool ShouldPopulateReference()
        {
            if (useConstant == false && _variable == null) {
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