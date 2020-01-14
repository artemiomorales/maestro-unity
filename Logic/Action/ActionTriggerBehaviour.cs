using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic.Action
{
    [ExecuteInEditMode]
    public class ActionTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ActionTrigger _actionTrigger;

        private ActionTrigger actionTrigger => _actionTrigger;

        private void Start()
        {
            if (actionTrigger.triggerOnStart == true) {
                CallPerformActions(this.gameObject);
            }
        }

        public void CallPerformActions(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            actionTrigger.PerformActions(this.gameObject);
        }

#if UNITY_EDITOR
        private void Update()
        {
            actionTrigger.CallSyncEditorActionHeadings();
            actionTrigger.CallSyncComplexSubheadings(this.gameObject,
                new SerializedObject(this).FindProperty(nameof(_actionTrigger)));
            actionTrigger.SyncFullActionDescription();
        }
#endif
    }
}