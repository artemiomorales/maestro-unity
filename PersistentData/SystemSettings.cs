using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Settings/System Settings")]
    public class SystemSettings : ScriptableObject
    {
        [SerializeField, Required]
        private BoolReference _hasBeenOpened = new BoolReference();

        public BoolReference hasBeenOpened => _hasBeenOpened;
        
        [SerializeField, Required]
        private BoolReference _tutorialShown = new BoolReference();

        public BoolReference tutorialShown => _tutorialShown;

        [SerializeField, Required]
        private FloatReference _deviceAspectRatio = new FloatReference();
        
        public FloatReference deviceAspectRatio => _deviceAspectRatio;

        [SerializeField, Required]
        private FloatReference _deviceHeight = new FloatReference();
        
        public FloatReference deviceHeight => _deviceHeight;

        [SerializeField, Required]
        private FloatReference _deviceWidth = new FloatReference();
        
        public FloatReference deviceWidth => _deviceWidth;

        [SerializeField, Required]
        private StringReference _activeScene = new StringReference();
        
        public StringReference activeScene => _activeScene;
        
        [SerializeField, Required]
        private FloatReference _appAspectRatio = new FloatReference();
        
        public FloatReference appAspectRatio => _appAspectRatio;

        [SerializeField, Required]
        private FloatReference _appHeight = new FloatReference();
        
        public FloatReference appHeight => _appHeight;

        [SerializeField, Required]
        private FloatReference _appWidth = new FloatReference();
        
        public FloatReference appWidth => _appWidth;
        
        [SerializeField, Required]
        private FloatReference _currentSceneAspectRatio = new FloatReference();
        
        public FloatReference currentSceneAspectRatio => _currentSceneAspectRatio;

        [SerializeField, Required]
        private FloatReference _currentSceneHeight = new FloatReference();
        
        public FloatReference currentSceneHeight => _currentSceneHeight;

        [SerializeField, Required]
        private FloatReference _currentSceneWidth = new FloatReference();
        
        public FloatReference currentSceneWidth => _currentSceneWidth;

        [SerializeField, Required]
        private BoolReference _volumeEnabled = new BoolReference();

        public BoolReference volumeEnabled => _volumeEnabled;

        [SerializeField, Required]
        private BoolReference _musicEnabled = new BoolReference();

        public BoolReference musicEnabled => _musicEnabled;

        [SerializeField, Required]
        private FloatReference _masterVolume = new FloatReference();

        public FloatReference masterVolume => _masterVolume;

        [SerializeField, Required]
        private BoolReference _soundEffectsEnabled = new BoolReference();

        public BoolReference soundEffectsEnabled => _soundEffectsEnabled;

        [SerializeField, Required]
        private BoolReference _progressBarVisible = new BoolReference();

        public BoolReference progressBarVisible => _progressBarVisible;
        
        [SerializeField, Required]
        private BoolReference _paused = new BoolReference();

        public BoolReference paused => _paused;

        [SerializeField, Required]
        private IntReference _framesPerSecond = new IntReference();
        
        public IntReference framesPerSecond => _framesPerSecond;
        
        [SerializeField, Required]
        private FloatReference _timescale = new FloatReference();

        public FloatReference timescale => _timescale;
            
        [SerializeField]
        private BoolReference _hasBookmark = new BoolReference();

        public BoolReference hasBookmark => _hasBookmark;
        
        [SerializeField, Required]
        private StringReference _lastOpenedScene = new StringReference();
        
        public StringReference lastOpenedScene => _lastOpenedScene;
        
        [SerializeField, Required]
        private StringReference _lastLoadedSequence = new StringReference();
        
        public StringReference lastLoadedSequence => _lastLoadedSequence;
        
        [SerializeField, Required]
        private FloatReference _lastLoadedSequenceTime = new FloatReference();
        
        public FloatReference lastLoadedSequenceTime => _lastLoadedSequenceTime;
        
        [SerializeField, Required]
        private FloatReference _sceneLoadingProgress = new FloatReference();
        
        public FloatReference sceneLoadingProgress => _sceneLoadingProgress;
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if UNITY_EDITOR            
            for (int i = 0; i < fields.Length; i++) {
                var referenceFieldValue = fields[i].GetValue(this) as ReferenceBase;
                referenceFieldValue.isSystemReference = true;
            }
#endif

            (deviceHeight.GetVariable() as FloatVariable).resetOnGameRefresh = false;
            (deviceWidth.GetVariable() as FloatVariable).resetOnGameRefresh = false;
            (deviceAspectRatio.GetVariable() as FloatVariable).resetOnGameRefresh = false;
            
            (hasBeenOpened.GetVariable() as BoolVariable).defaultValue = false;
            (volumeEnabled.GetVariable() as BoolVariable).defaultValue = true;
            (musicEnabled.GetVariable() as BoolVariable).defaultValue = true;
            (soundEffectsEnabled.GetVariable() as BoolVariable).defaultValue = true;
            (progressBarVisible.GetVariable() as BoolVariable).defaultValue = false;
            (paused.GetVariable() as BoolVariable).defaultValue = false;
            (timescale.GetVariable() as FloatVariable).defaultValue = 1;
            
            // Bookmarking
            
            (hasBookmark.GetVariable() as BoolVariable).defaultValue = false;
            (lastOpenedScene.GetVariable() as StringVariable).defaultValue = "";
            (lastLoadedSequence.GetVariable() as StringVariable).defaultValue = "";
            (lastLoadedSequenceTime.GetVariable() as FloatVariable).defaultValue = 0;

            // Scene Loading Progress
            
            (sceneLoadingProgress.GetVariable() as FloatVariable).defaultValue = 0;
            (sceneLoadingProgress.GetVariable() as FloatVariable).resetOnGameRefresh = false;

            for (int i = 0; i < fields.Length; i++) {
                var variableReference = fields[i].GetValue(this);
                
                var variableField =
                    Utils.GetVariableFieldFromReference(fields[i], this, out var referenceValue);
                var variableValue = variableField.GetValue(referenceValue);

                if (variableValue is ModifiableEditorVariable modifiableEditorVariable) {
                    modifiableEditorVariable.StoreCaller(this, "updating from system settings", "app settings");
                    modifiableEditorVariable.SetToDefaultValue();
#if UNITY_EDITOR
                    EditorUtility.SetDirty(modifiableEditorVariable);                    
#endif
                }
            }
        }
        

#if UNITY_EDITOR
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshDependencies()
        {
            FieldInfo[] referenceFields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < referenceFields.Length; i++) {

                string name = referenceFields[i].Name.Replace("_", "").Capitalize();
                var variableField = Utils.GetVariableFieldFromReference(referenceFields[i], this, out var referenceValue);

                var variable = variableField.GetValue(referenceValue) as ScriptableObject;

                if (variable == null) {
                    variableField.SetValue(referenceValue, CreateSystemSetting(variableField.FieldType, $"{name}"));
                }
            }
            EditorUtility.SetDirty(this);
        }
        
        private static dynamic CreateSystemSetting(Type assetType, string name)
        {
            return Utils.ForceCreateScriptableObjectAsset(assetType, name, Utils.settingsPath + "/SystemSettings");
        }
#endif
  
    }
}