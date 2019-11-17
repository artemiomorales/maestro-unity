using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(AudioForwardReverseClip))]
    [TrackBindingType(typeof(AudioSource))]
    public class AudioForwardReverseTrack : LerpToTargetTrack
    {

#if UNITY_EDITOR
        public void StoreUtilVars(GameObject go)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as AudioForwardReverseClip;
                if (myAsset) {
                    myAsset.isReversing.Variable = go.GetComponent<TrackAssetConfig>().isReversing;
                    myAsset.frameStepValue.Variable = go.GetComponent<TrackAssetConfig>().frameStepValue;
                    myAsset.swipeModifierOutput.Variable = go.GetComponent<TrackAssetConfig>().swipeModifierOutput;;
                }
            }
        }
#endif

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
#if UNITY_EDITOR
            StoreUtilVars(go);
#endif
            StoreClipProperties(go);
            return ScriptPlayable<AudioForwardReverseMixerBehaviour>.Create(graph, inputCount);
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {

//#if UNITY_EDITOR
//            var comp = director.GetGenericBinding(this) as AudioSource;
//            if (comp == null)
//                return;
//            var so = new UnityEditor.SerializedObject(comp);
//            var iter = so.GetIterator();
//            while (iter.NextVisible(true)) {
//                if (iter.hasVisibleChildren)
//                    continue;
//                driver.AddFromName<AudioSource>(comp.gameObject, iter.propertyPath);
//            }
//#endif

            base.GatherProperties(director, driver);
        }
    }
    
}