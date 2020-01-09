﻿using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class TimelineTriggerBehaviour : PlayableBehaviour {

        [HideInInspector]
        private double _startTime;

        public double startTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        [HideInInspector]
        private double _endTime;

        public double endTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        [HideInInspector]
        private bool _triggered = false;

        public bool triggered
        {
            get => _triggered;
            set => _triggered = value;
        }

        [HideInInspector]
        public BoolReference _isReversing = new BoolReference();

        public bool isReversing
        {
            get => _isReversing.GetValue(this.directorObject);
            set => _isReversing.GetVariable(this.directorObject).SetValue(value);
        }

        [FormerlySerializedAs("disableOnReverse")]
        [SerializeField]
        private bool _disableOnReverse;

        public bool disableOnReverse
        {
            get => _disableOnReverse;
            set => _disableOnReverse = value;
        }
        
        [HideInInspector]
        private GameObject _directorObject;

        public GameObject directorObject
        {
            get => _directorObject;
            set => _directorObject = value;
        }

    }
}