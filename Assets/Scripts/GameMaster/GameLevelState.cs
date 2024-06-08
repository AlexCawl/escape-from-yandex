namespace GameMaster
{
    public class GameLevelState
    {
        public GameLevel Level { get; private set; } = GameLevel.Top;

        public void Next()
        {
            Level = (GameLevel)(((int)Level + 1) % 3);
        }
    }

    public enum GameLevel
    {
        Top,
        Mid,
        Low
    }
}