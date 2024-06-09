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
        private HealthHolder _playerHealth;
        private State _flashLightState;
        private GameLevelState _gameLevelState;
        private GameInput _gameInput;

        private SceneLoadState _pauseState;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
            _exitOpenState = ServiceLocator.Get.Create(new State(), "exitOpenState");
            _playerHealth = ServiceLocator.Get.Create(new HealthHolder(), "playerHealth");
            _flashLightState = ServiceLocator.Get.Locate<State>("flashLightState");
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
            _pauseState = ServiceLocator.Get.Create(new SceneLoadState(), "pauseState");
            _miniGameState = ServiceLocator.Get.Create(new SceneLoadState(), "miniGameState");
            ServiceLocator.Get.Create(new State(), "tooltipVisibilityState");
            ServiceLocator.Get.Create(new State(), "miniGamePassedState");
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            new SceneLoadObserver(_pauseState, "Scenes/PauseMenu").Observe(this);
            new SceneLoadObserver(_miniGameState, "Scenes/NumbersChallenge").Observe(this);
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

        private void HandlePauseMenuClick(InputAction.CallbackContext value) => _pauseState.Toggle();

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
}