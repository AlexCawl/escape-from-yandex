using UnityEngine;

namespace GameCharacter
{
    public class ReloadHolder
    {
        private float _current;
        private readonly float _step;
        public readonly int Steps;

        public ReloadHolder(int steps)
        {
            _current = 1;
            _step = 1.0f / steps;
            Steps = steps;
        }

        public bool CanShoot => Mathf.Approximately(_current, 1.0f);

        public float Percent => _current;

        public void Shoot()
        {
            _current = 0;
        }

        public void Reload()
        {
            _current += _step;
            if (_current > 1.0f)
            {
                _current = 1.0f;
            }
        }
    }
}