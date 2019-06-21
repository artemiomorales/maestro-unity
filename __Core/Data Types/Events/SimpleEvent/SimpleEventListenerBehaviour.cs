﻿using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class SimpleEventListenerBehaviour : MonoBehaviour, ISimpleEventListener, ISkipRegistration
    {
        [Required]
        public SimpleEvent Event;

        [ValidateInput("IsPopulated")]
        public UnityEvent Response;

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        bool doNotRecord;

        public bool DoNotRecord {
            get {
                return doNotRecord;
            }
        }

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

		private void OnDisable()
		{
            Event.UnregisterListener(this);
		}

        public void OnEventRaised()
        {
            Response.Invoke();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + gameObject, gameObject);
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}