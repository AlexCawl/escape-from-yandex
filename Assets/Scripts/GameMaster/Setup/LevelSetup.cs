using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class LevelSetup : MonoBehaviour
    {
        private State _exitOpenState;
        private GameInput _gameInput;

        private NumberState _playerHealth;
        private GameLevelState _gameLevelState;
        private SceneLoadState _pauseState;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
            _exitOpenState = ServiceLocator.Get.Create(new State(), "exitOpenState");
            _pauseState = ServiceLocator.Get.Create(new SceneLoadState(), "pauseState");
            _miniGameState = ServiceLocator.Get.Create(new SceneLoadState(), "miniGameState");
            ServiceLocator.Get.Create(new State(), "tooltipVisibilityState");
            ServiceLocator.Get.Create(new State(), "miniGamePassedState");
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
            _playerHealth = ServiceLocator.Get.Locate<NumberState>("playerHealth");
            new SceneLoadObserver(_pauseState, "Scenes/PauseMenu").Observe(this);
            new SceneLoadObserver(_miniGameState, "Scenes/NumbersChallenge").Observe(this);
            StartCoroutine(CheckPlayerDeath());
            StartCoroutine(CheckLevelPassed());
        }

        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Pause.performed += HandlePauseMenuClick;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Pause.performed -= HandlePauseMenuClick;
        }

        private void HandlePauseMenuClick(InputAction.CallbackContext value) => _pauseState.Toggle();

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckPlayerDeath()
        {
            while (true)
            {
                if (_playerHealth.Get == 0)
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