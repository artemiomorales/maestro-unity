﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class TextFamilyConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("Text Family Reference")]
        [HideReferenceObjectPicker]
        private TextFamilyReference _textFamilyReference = new TextFamilyReference();

        private TextFamily textFamilyReference => _textFamilyReference.GetVariable() as TextFamily;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [Title("Text Family Status Condition")]
        [HideReferenceObjectPicker]
        private BoolReference _activeTextFamilyCondition;

        private BoolReference activeTextFamilyCondition => _activeTextFamilyCondition;
        
        public TextFamilyConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath) : base(parentObject, serializedPropertyPath) { }
        
        public override ConditionResponseBase PopulateReferences()
        {
            string referencePath = serializedPropertyPath + $".{nameof(_textFamilyReference)}";
            _textFamilyReference.PopulateVariable(parentObject, referencePath.Split('.'));
            
            string conditionPath = serializedPropertyPath + $".{nameof(_activeTextFamilyCondition)}";
            _activeTextFamilyCondition.PopulateVariable(parentObject, conditionPath.Split('.'));
            
            return this;
        }
        
        public override void SyncConditionHeading(Object callingObject)
        {
            CheckPopulateReferences();
            
            base.SyncConditionHeading(callingObject);
            if (textFamilyReference == null) {
                conditionEventTitle = "Please populate a text family as your condition.";
                return;
            }

            if (activeTextFamilyCondition.GetVariable() == null && activeTextFamilyCondition.useConstant == false) {
                conditionEventTitle = "Please populate a condition for your text family.";
                return;
            }

            string newTitle = $"Text family {textFamilyReference.name} active is ";

            if (activeTextFamilyCondition.useConstant == true) {
                newTitle += activeTextFamilyCondition.GetValue();
            }
            else {
                newTitle += $"equal to {activeTextFamilyCondition.GetVariable().name}";
            }

            conditionEventTitle = newTitle;
        }
        
        public virtual ConditionResponseBase PopulateReferences(UnityEngine.Object parentObject,
            string serializedPropertyPath)
        {
            string referencePath = serializedPropertyPath + $".{nameof(_textFamilyReference)}";
            _textFamilyReference.PopulateVariable(parentObject, referencePath.Split('.'));
            
            string conditionPath = serializedPropertyPath + $".{nameof(_activeTextFamilyCondition)}";
            _activeTextFamilyCondition.PopulateVariable(parentObject, conditionPath.Split('.'));
            
            return this;
        }

        public override bool CheckCondition(Object callingObject)
        {
            CheckPopulateReferences();
            
            base.CheckCondition(callingObject);
            
            if (textFamilyReference.active == activeTextFamilyCondition.GetValue()) {
                return true;
            }

            return false;
        }

        public TextFamily GetReference()
        {
            CheckPopulateReferences();
            
            return textFamilyReference;
        }
        
        public BoolReference GetCondition()
        {
            CheckPopulateReferences();
            
            return activeTextFamilyCondition;
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}