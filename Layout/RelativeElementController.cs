﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class RelativeElementController : MonoBehaviour, IDynamicLayoutElement
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;
        
        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }

        private RelativeElement[] _relativeElements;

        private RelativeElement[] relativeElements
        {
            get => _relativeElements;
            set => _relativeElements = value;
        }

        private Dictionary<int, RelativeElement> _elementCollection = new Dictionary<int, RelativeElement>();

        private Dictionary<int, RelativeElement> elementCollection => _elementCollection;

        private int _elementCount;

        private int elementCount => _elementCount;

        private List<int> _elementKeys = new List<int>();

        private List<int> elementKeys
        {
            get => _elementKeys;
            set => _elementKeys = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private List<RelativeElement> _sortedElements = new List<RelativeElement>();

        private List<RelativeElement> sortedElements => _sortedElements;

        public string elementName => gameObject.name;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _dynamicElementEnable = new ComplexEventTrigger();

        public ComplexEventTrigger dynamicElementEnable => _dynamicElementEnable;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _dynamicElementDisable = new ComplexEventTrigger();

        public ComplexEventTrigger dynamicElementDisable => _dynamicElementDisable;

        public Scene parentScene => gameObject.scene;
        
        [SerializeField]
        private int _priority;

        public int priority => _priority;
        
        [SerializeField]
        private bool _logElementOnLayoutUpdate = false;

        public bool logElementOnLayoutUpdate {
            get {
                if (appSettings.logResponsiveElementActions == true) {
                    return true;
                } else {
                    return _logElementOnLayoutUpdate;
                }
            }
        }

        private void Start()
        {
            RefreshElements();
        }
        
        private void OnEnable()
        {
            dynamicElementEnable.RaiseEvent(this.gameObject, this);
        }

//        private void OnDisable()
//        {
//            dynamicElementDisable.RaiseEvent(this.gameObject, this);
//        }

        private void RefreshElements()
        {
            GetUnsortedElements();
            StoreSortedElements();
            CallExecuteLayoutUpdate(this.gameObject);
        }

        private void GetUnsortedElements()
        {
            relativeElements = GetComponentsInChildren<RelativeElement>();
        }

        private void StoreSortedElements()
        {
            elementCollection.Clear();
            for (int i = 0; i < relativeElements.Length; i++) {
                elementCollection.Add(relativeElements[i].gameObject.transform.GetSiblingIndex(), relativeElements[i]);
            }
            elementKeys = elementCollection.Keys.ToList();
            elementKeys.Sort();

            // Clear our list before populating it again
            sortedElements.Clear();

            for (int i = 0; i < elementKeys.Count; i++) {
                sortedElements.Add(elementCollection[elementKeys[i]]);
            }
        }

        [InfoBox("Trigger relative action.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(8)]
        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }
            
            for (int i = 0; i < sortedElements.Count; i++) {
                if (logElementOnLayoutUpdate == true) {
                    Debug.Log("Relative object updated: " + sortedElements[i].gameObject.name, sortedElements[i].gameObject);
                }
                sortedElements[i].ExecuteRelativeAction();
            }
        }

#if UNITY_EDITOR

        private void OnGUI()
        {
            if(ElementsChanged()) {
                StoreSortedElements();
                CallExecuteLayoutUpdate(this.gameObject);
            }
        }

        private bool ElementsChanged()
        {
            GetUnsortedElements();
            if (relativeElements.Length != elementCount) {
                return true;
            } else {
                return false;
            }
        }
#endif
        
        private static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}