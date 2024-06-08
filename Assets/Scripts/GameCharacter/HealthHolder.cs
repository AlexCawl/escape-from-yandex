using System.Threading;

namespace GameCharacter
{
    public class HealthHolder
    {
        private const int DefaultHealth = 100;
        private const int MinHealth = 0;

        private int _health;
        private readonly int _maxHealth;

        public HealthHolder(int initialHealth = DefaultHealth)
        {
            _health = initialHealth;
            _maxHealth = initialHealth;
        }

        public float Percent => (float)_health / _maxHealth;

        public bool IsDead => _health == 0;

        private void Set(int value) => Interlocked.Exchange(ref _health, Validate(value, MinHealth, _maxHealth));

        public void Increase(int value) => Set(_health + value);

        public void Decrease(int value) => Set(_health - value);

        private static int Validate(int value, int min, int max) => value > max ? max : value < min ? min : value;
    }
}