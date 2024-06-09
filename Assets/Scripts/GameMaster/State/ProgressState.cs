using System.Threading;
using UnityEngine;

namespace GameMaster.State
{
    public class ProgressState : BaseState<float>
    {
        private const float MinValue = 0f;
        private const float MaxValue = 1.0f;
        private readonly float _stepSize;

        public readonly int StepCount;

        public ProgressState(int stepCount, float initialValue = MaxValue)
        {
            Value = initialValue;
            StepCount = stepCount;
            _stepSize = MaxValue / StepCount;
        }
        
        public override void Set(float newValue) => Interlocked.Exchange(ref Value, Coerce(newValue));

        public void Load() => Set(Value + _stepSize);
        
        public void Reset() => Set(MinValue);

        public float Percent => (Value - MinValue) / (MaxValue - MinValue);
        
        public bool IsReady() => Mathf.Approximately(Value, MaxValue);
        
        private static float Coerce(float value) => value > MaxValue ? MaxValue : value < MinValue ? MinValue : value;
    }
}