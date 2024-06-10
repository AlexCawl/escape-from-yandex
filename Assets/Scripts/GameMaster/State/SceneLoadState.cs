namespace GameMaster.State
{
    public class SceneLoadState : BaseState<bool>
    {
        private bool _nextValue;

        public SceneLoadState(bool loadType = false)
        {
            Value = loadType;
            _nextValue = loadType;
        }

        public void Open() => _nextValue = true;

        public void Close() => _nextValue = false;

        public void Toggle() => _nextValue = !_nextValue;

        public bool OpenRequest() => Value == false && _nextValue;

        public bool CloseRequest() => Value && _nextValue == false;

        public void Sync() => Value = _nextValue;
    }
}