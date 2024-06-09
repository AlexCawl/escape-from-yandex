using System.Threading;

namespace GameMaster.State
{
    public class NumberState : BaseState<int>
    {
        private readonly int _minValue;
        private readonly int _maxValue;

        public NumberState(int initialValue, int minValue, int maxValue)
        {
            Value = initialValue;
            _minValue = minValue;
            _maxValue = maxValue;
        }
        
        public override void Set(int newValue) => Interlocked.Exchange(ref Value, Coerce(newValue));
        
        public void Increase(int value) => Set(Value + value);

        public void Decrease(int value) => Set(Value - value);

        public float Percent => (float)(Value - _minValue) / (_maxValue - _minValue);
        
        private int Coerce(int value) => value > _maxValue ? _maxValue : value < _minValue ? _minValue : value;
    }
}