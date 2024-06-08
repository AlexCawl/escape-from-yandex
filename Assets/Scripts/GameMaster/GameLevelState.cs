namespace GameMaster
{
    public class GameLevelState
    {
        public GameLevel Level { get; private set; } = GameLevel.Top;

        public bool Next()
        {
            var next = (int)Level + 1;
            Level = (GameLevel)(next % 3);
            return next != 3;
        }

        public void Reset()
        {
            Level = GameLevel.Top;
        }
    }

    public enum GameLevel
    {
        Top,
        Mid,
        Low
    }
}