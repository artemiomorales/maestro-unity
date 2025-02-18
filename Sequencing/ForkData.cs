using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    public class ForkData : Input_Data
    {
        [SerializeField]
        private Fork _fork;

        public Fork fork
        {
            get => _fork;
            private set => _fork = value;
        }

        protected override string dataTitle
        {
            get => sequence.name + " - " + fork.name;
        }

        [SerializeField]
        private string _description;

        public string description
        {
            get => _description;
            private set => _description = value;
        }

        [SerializeField]
        private JoinMarker _joinMarker;
        
        public JoinMarker joinMarker
        {
            get => _joinMarker;
            private set => _joinMarker = value;
        }

        [TitleGroup("$"+nameof(dataTitle))]
        private MarkerPlacement _markerPlacement;

        public MarkerPlacement markerPlacement
        {
            get => _markerPlacement;
            private set => _markerPlacement = value;
        }

        public ForkData(Sequence sequence, JoinMarker joinMarker, Fork fork)
        {
            this.sequence = sequence;
            this.joinMarker = joinMarker;
            this.markerPlacement = joinMarker.markerPlacement;
            this.description = joinMarker.description;

            this.fork = fork;
        }
    }
}