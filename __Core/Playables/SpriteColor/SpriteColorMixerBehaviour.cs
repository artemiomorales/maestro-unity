using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt
{
    public class SpriteColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        SpriteRenderer trackBinding;
        ScriptPlayable<SpriteColorBehaviour> inputPlayable;
        SpriteColorBehaviour input;
        SpriteRenderer trackBindingComponent;
        Color originalColor;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as SpriteRenderer;
            
            if (!trackBinding)
                return;

            if(trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<SpriteRenderer>();
                originalColor = trackBindingComponent.color;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<SpriteColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.color = Color.Lerp(input.initialColor, input.targetColor, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.color = input.targetColor;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.color = input.initialColor;
                    }
                }
            }
        }

        //public override void OnGraphStart(Playable playable)
        //{
        //    base.OnGraphStart(playable);
        //    if(trackBindingComponent != null) {
        //        trackBindingComponent.color = originalColor;
        //    }
        //}
    }   
}