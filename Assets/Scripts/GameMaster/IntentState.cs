namespace GameMaster
{
    public class IntentState
    {
        private bool _state;
        private bool _nextState;

        internal void Toggle()
        {
            _nextState = !_nextState;
        }
        
        public void RequestOpen()
        {
            _nextState = true;
        }

        public void SubmitOpen()
        {
            _state = true;
        }
        
        public void RequestClose()
        {
            _nextState = false;
        }

        public void SubmitClose()
        {
            _state = false;
        }

        public bool ShouldBeOpened() => _state == false && _nextState;

        public bool ShouldBeClosed() => _state && _nextState == false;
    }
}