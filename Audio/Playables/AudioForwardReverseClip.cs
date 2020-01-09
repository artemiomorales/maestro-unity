using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioForwardReverseClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private AudioForwardReverseBehaviour _template = new AudioForwardReverseBehaviour ();

        private AudioForwardReverseBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        public override LerpToTargetBehaviour templateReference => template;
        
        private BoolReference _isReversing = new BoolReference();

        public BoolReference isReversingReference
        {
            get => _isReversing;
            set => _isReversing = value;
        }
        
        private FloatReference _frameStepValue = new FloatReference();

        public FloatReference frameStepValue
        {
            get => _frameStepValue;
            set => _frameStepValue = value;
        }
        
        private FloatReference _swipeModifierOutput = new FloatReference();

        public FloatReference swipeModifierOutput
        {
            get => _swipeModifierOutput;
            set => _swipeModifierOutput = value;
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template._isReversing.SetVariable(isReversingReference.GetVariable(this.directorObject));
            template.frameStepValue.variable = frameStepValue.variable;
            template.swipeModifier.variable = swipeModifierOutput.variable;

            var playable = ScriptPlayable<AudioForwardReverseBehaviour>.Create(graph, template);
            return playable;
        }
    }
}