namespace GameMaster.State
{
    public class GameLevelState : BaseState<GameLevel>
    {
        public GameLevelState()
        {
            Value = GameLevel.Top;
        }

        public bool Next()
        {
            var next = (int)Value + 1;
            Value = (GameLevel)(next % 3);
            return next != 3;
        }

        public void Reset()
        {
            Value = GameLevel.Top;
        }
    }

    public enum GameLevel
    {
        Top,
        Mid,
        Low
    }
}