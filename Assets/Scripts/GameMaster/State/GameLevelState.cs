using System;

namespace GameMaster.State
{
    public class GameLevelState : BaseState<GameLevel>
    {
        public GameLevelState()
        {
            Value = GameLevel.Top;
        }

        public string Name() =>
            Value switch
            {
                GameLevel.Top => "Scenes/LevelTop",
                GameLevel.Mid => "Scenes/LevelMid",
                GameLevel.Low => "Scenes/LevelLow",
                _ => throw new ArgumentOutOfRangeException()
            };

        public bool IsLast() => Value == GameLevel.Low;

        public string Next()
        {
            var next = (int)Value + 1;
            Value = (GameLevel)(next % 3);
            return Name();
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