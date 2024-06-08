using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class LevelSetup : MonoBehaviour
    {
        private State _exitOpenState;
        private IntentState _miniGameOverlayState;
        private IntentState _pauseOverlayState;
        private HealthHolder _playerHealth;
        private State _flashLightState;
        private GameLevelState _gameLevelState;

        private void Awake()
        {
            _miniGameOverlayState = ServiceLocator.Get.Create(new IntentState(), "miniGameOverlayState");
            _pauseOverlayState = ServiceLocator.Get.Create(new IntentState(), "pauseOverlayState");
            _exitOpenState = ServiceLocator.Get.Create(new State(), "exitOpenState");
            _playerHealth = ServiceLocator.Get.Create(new HealthHolder(), "playerHealth");
            _flashLightState = ServiceLocator.Get.Locate<State>("flashLightState");
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
            ServiceLocator.Get.Create(new State(), "tooltipVisibilityState");
            ServiceLocator.Get.Create(new State(), "miniGamePassedState");
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(SceneManagerUtils.OpenScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(SceneManagerUtils.CloseScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(SceneManagerUtils.OpenScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(SceneManagerUtils.CloseScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(CheckPlayerDeath());
            StartCoroutine(CheckLevelPassed());
        }

        private void Update()
        {
            HandlePauseMenuClick();
            HandleFlashLightToggleClick();
        }

        private void HandlePauseMenuClick()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            _pauseOverlayState.Toggle();
        }

        private void HandleFlashLightToggleClick()
        {
            var pressed = Input.GetKeyDown("f");
            if (!pressed) return;
            _flashLightState.Set(!_flashLightState.Get);
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckPlayerDeath()
        {
            while (true)
            {
                if (_playerHealth.IsDead)
                {
                    _gameLevelState.Reset();
                    SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
                }

                yield return null;
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckLevelPassed()
        {
            while (true)
            {
                if (_exitOpenState.Get)
                {
                    _gameLevelState.Next();
                    SceneManager.LoadSceneAsync("Scenes/Selector", LoadSceneMode.Single);
                    yield break;
                }

                yield return null;
            }
        }
    }

    internal static class SceneManagerUtils
    {
        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        public static IEnumerator OpenScene(IntentState sceneStateController, string sceneName)
        {
            while (true)
            {
                yield return new WaitUntil(sceneStateController.ShouldBeOpened);
                sceneStateController.SubmitOpen();
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        public static IEnumerator CloseScene(IntentState sceneStateController, string sceneName)
        {
            while (true)
            {
                yield return new WaitUntil(sceneStateController.ShouldBeClosed);
                sceneStateController.SubmitClose();
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
}