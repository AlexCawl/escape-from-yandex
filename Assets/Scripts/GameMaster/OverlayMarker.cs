namespace GameMaster
{
    public class OverlayMarker
    {
        private bool _state;

        public void Activate()
        {
            _state = true;
        }
        
        public void Deactivate()
        {
            _state = false;
        }

        public bool State => _state;
    }
}