namespace GameMaster
{
    public class State
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
        
        public void Set(bool state)
        {
            _state = state;
        }

        public bool Get => _state;
    }
}