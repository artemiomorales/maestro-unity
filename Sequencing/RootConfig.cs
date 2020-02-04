using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    public class RootConfig : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        public AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        public InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;

        [SerializeField]
        private UserDataKeyReference _userKey;

        public UserDataKey userKey => _userKey.GetVariable() as UserDataKey;
        
        public SimpleEventTrigger sequenceModified =>
            appSettings.GetSequenceModified(this.gameObject, inputGroupKey);

        [Required]
        [SerializeField]
        private GameObject _masterSequenceContainer;

        public GameObject masterSequenceContainer => _masterSequenceContainer;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences => _masterSequences;

        [Required]
        [SerializeField]
        private Joiner _joiner;

        public Joiner joiner => _joiner;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<RootDataCollector> _rootDataCollectors = new List<RootDataCollector>();

        public List<RootDataCollector> rootDataCollectors => _rootDataCollectors;

        public bool appUtilsRequested => appSettings.GetAppUtilsRequested(this.gameObject, inputGroupKey);

        private void OnEnable()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));
            _userKey.PopulateVariable(this, nameof(_userKey));
#endif
            Configure();
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.4f, 1)]
        public void Configure()
        {
            for (int i = 0; i < masterSequences.Count; i++) {
                masterSequences[i].rootConfig = this;
                masterSequences[i].Init();
            }
            
            joiner.rootConfig = this;
            joiner.ConfigureData();

            for (int i = 0; i < rootDataCollectors.Count; i++) {
                rootDataCollectors[i].rootConfig = this;
                rootDataCollectors[i].ConfigureData();
            }
            
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(List<MasterSequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<RootDataCollector> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}