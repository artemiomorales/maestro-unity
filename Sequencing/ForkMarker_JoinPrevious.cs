namespace AltSalt.Maestro.Sequencing
{
    public class ForkMarker_JoinPrevious : ForkMarker
    {
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}