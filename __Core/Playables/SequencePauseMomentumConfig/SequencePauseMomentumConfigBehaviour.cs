using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class SequencePauseMomentumConfigBehaviour : LerpToTargetBehaviour
    {
        public string description;
        public bool isVideoSequence;
    }
}