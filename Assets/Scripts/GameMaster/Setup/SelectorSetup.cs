using System;
using GameMaster.State;
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
            progressLabel.text += $"{(int)_gameLevelState.Get} / 3";
            switch (_gameLevelState.Get)
            {
                case GameLevel.Top:
                    topLevelButton.interactable = true;
                    topLevelButton.onClick.AddListener(() => NavigateToLevel(_gameLevelState.Name()));
                    break;
                case GameLevel.Mid:
                    midLevelButton.interactable = true;
                    midLevelButton.onClick.AddListener(() => NavigateToLevel(_gameLevelState.Name()));
                    break;
                case GameLevel.Low:
                    lowLevelButton.interactable = true;
                    lowLevelButton.onClick.AddListener(() => NavigateToLevel(_gameLevelState.Name()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void NavigateToLevel(string name) => SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);

        private static void NavigateToSplash() => SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
    }
}
