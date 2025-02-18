﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Events/Complex Event")]
    public class ComplexEvent : PersistentDataObject
    {
        protected override string title => nameof(ComplexEvent);
        
        [SerializeField]
        private List<ComplexEventParameters> requiredParameters = new List<ComplexEventParameters>();

        [SerializeField]
        private List<ComplexEventParameters> optionalParameters = new List<ComplexEventParameters>();

        private List<ComplexEventListenerBehaviour> listeners = new List<ComplexEventListenerBehaviour>();

        public void Raise()
        {
            if (CallerRegistered() == false) return;
            
            if(logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogCaller();
            }
            
            if (logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogListenersHeading(listeners.Count);
            }
            
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                    LogListenerOnRaise(listeners[i]);
                }
                listeners[i].OnEventRaised(ComplexPayload.CreateInstance());
            }
            
            if (logCallersOnRaise == true || logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogClosingLine();
            }
            ClearCaller();
        }

        public void Raise(object value)
        {
            if (CallerRegistered() == false) return;
            
            if (logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogCaller();
            }
            
            if (logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogListenersHeading(listeners.Count);
            }
            
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                    LogListenerOnRaise(listeners[i]);
                }

                if(value is ComplexPayload) {
                    listeners[i].OnEventRaised(value as ComplexPayload);
                } else {
                    listeners[i].OnEventRaised(ComplexPayload.CreateInstance(value));
                }
            }
            
            if (logCallersOnRaise == true || logListenersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                LogClosingLine();
            }
            ClearCaller();
        }

        protected override void LogCaller()
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2} triggered complex event...", callerScene, this.name, callerName), callerObject);
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", callerScene, this.name, this.name), this);
        }

        private void LogListenerOnRaise(ComplexEventListenerBehaviour complexEventListenerBehaviour)
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", complexEventListenerBehaviour.gameObject.scene.name, this.name, complexEventListenerBehaviour.name), complexEventListenerBehaviour.gameObject);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].gameObject.name, listeners[i].gameObject);
            }
        }

        public void RegisterListener(ComplexEventListenerBehaviour listener)
        {
            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to complex event " + this.name, this);
                Debug.Log(listener.gameObject.name, listener.gameObject);
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ComplexEventListenerBehaviour listener)
		{
            SanitizeListenerList();
            
			listeners.Remove(listener);
		}
        
        private void SanitizeListenerList()
        {
            listeners.RemoveAll(x => x == null);
        }

        [Serializable]
        class ComplexEventParameters
        {
            public string description;
            public DataType dataType;

            public bool customKey;

            [FormerlySerializedAs("eventPayloadKey"),ShowIf(nameof(customKey))]
            public CustomKey _customKey;
        }

	}
}