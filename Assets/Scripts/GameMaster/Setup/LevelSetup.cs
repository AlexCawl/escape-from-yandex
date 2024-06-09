using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private GameInput _gameInput;

        private void Awake()
        {
            _gameInput = new GameInput();
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

        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Pause.performed += HandlePauseMenuClick;
            _gameInput.Player.Flashlight.performed += HandleFlashLightToggleClick;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Pause.performed -= HandlePauseMenuClick;
            _gameInput.Player.Flashlight.performed -= HandleFlashLightToggleClick;
        }

        private void HandlePauseMenuClick(InputAction.CallbackContext value) => _pauseOverlayState.Toggle();

        private void HandleFlashLightToggleClick(InputAction.CallbackContext value) => _flashLightState.Set(!_flashLightState.Get);

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckPlayerDeath()
        {
            while (true)
            {
                if (_playerHealth.IsDead)
                {
                    _gameLevelState.Reset();
                    SceneManager.LoadSceneAsync("Scenes/Defeat", LoadSceneMode.Single);
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
                    SceneManager.LoadSceneAsync(_gameLevelState.Next() ? "Scenes/Selector" : "Scenes/Victory", LoadSceneMode.Single);
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