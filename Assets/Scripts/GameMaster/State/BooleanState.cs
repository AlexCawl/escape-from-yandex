namespace GameMaster.State
{
    public class BooleanState : BaseState<bool>
    {
        public BooleanState(bool initialValue = false) => Value = initialValue;

        public void Activate() => Value = true;

        public void Deactivate() => Value = false;

        public void Toggle() => Value = !Value;
    }
}