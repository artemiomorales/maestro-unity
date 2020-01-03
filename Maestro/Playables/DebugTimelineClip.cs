﻿using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class DebugTimelineClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private DebugTimelineBehaviour _template = new DebugTimelineBehaviour ();

        private DebugTimelineBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        public override LerpToTargetBehaviour templateReference => template;
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<DebugTimelineBehaviour>.Create(graph, template);
            return playable;
        }
    }
}