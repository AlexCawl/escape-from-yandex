namespace GameMaster
{
    public class OverlayManager
    {
        private bool _overlayState;
        private bool _overlayNextState;

        internal void Toggle()
        {
            _overlayNextState = !_overlayNextState;
        }

        public void SubmitOpen()
        {
            _overlayState = true;
        }

        public void SubmitClose()
        {
            _overlayState = false;
        }

        public bool ShouldBeOpened() => _overlayState == false && _overlayNextState;

        public bool ShouldBeClosed() => _overlayState && _overlayNextState == false;
    }
}