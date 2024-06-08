using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameMaster.Setup
{
    public class SelectorSetup : MonoBehaviour
    {
        public Button topLevelButton;
        public Button midLevelButton;
        public Button lowLevelButton;
        public Text progressLabel;
        public Button backButton;

        private GameLevelState _gameLevelState;

        private void Awake()
        {
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
        }

        public void Start()
        {
            backButton.onClick.AddListener(NavigateToSplash);
            progressLabel.text += $"{(int)_gameLevelState.Level} / 3";
            switch (_gameLevelState.Level)
            {
                case GameLevel.Top:
                    topLevelButton.interactable = true;
                    topLevelButton.onClick.AddListener(NavigateToLevel);
                    break;
                case GameLevel.Mid:
                    midLevelButton.interactable = true;
                    midLevelButton.onClick.AddListener(NavigateToLevel);
                    break;
                case GameLevel.Low:
                    lowLevelButton.interactable = true;
                    lowLevelButton.onClick.AddListener(NavigateToLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NavigateToLevel()
        {
            SceneManager.LoadSceneAsync("Scenes/LevelScene", LoadSceneMode.Single);
            _gameLevelState.Next();
        }
        
        private static void NavigateToSplash() => SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
    }
}
