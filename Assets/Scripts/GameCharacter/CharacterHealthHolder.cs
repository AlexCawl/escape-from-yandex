using System.Threading;

namespace GameCharacter
{
    public class CharacterHealthHolder
    {
        private static CharacterHealthHolder _instance;

        private CharacterHealthHolder()
        {
        }

        public static CharacterHealthHolder GetInstance() => _instance ??= new CharacterHealthHolder();

        private const int MaxHealth = 100;
        private int _health = MaxHealth;

        public int Get => _health;
        
        public static int GetMax => MaxHealth;

        public void Set(int value) => Interlocked.Exchange(ref _health, value);

        public void Increase(int value) => Interlocked.Add(ref _health, value);

        public void Decrease(int value) => Interlocked.Add(ref _health, -1 * value);
    }
}