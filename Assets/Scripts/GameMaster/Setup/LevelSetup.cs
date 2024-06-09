using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class LevelSetup : MonoBehaviour
    {
        private GameInput _gameInput;

        private NumberState _playerHealth;
        private GameLevelState _gameLevelState;
        private SceneLoadState _pauseState;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
            _pauseState = ServiceLocator.Get.Create(new SceneLoadState(), "pauseState");
            _miniGameState = ServiceLocator.Get.Create(new SceneLoadState(), "miniGameState");
            ServiceLocator.Get.Create(new BooleanState(), "miniGameCompleteState");
            ServiceLocator.Get.Create(new State(), "tooltipVisibilityState");
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
            _playerHealth = ServiceLocator.Get.Locate<NumberState>("playerHealth");
            new SceneLoadObserver(_pauseState, "Scenes/PauseMenu").Observe(this);
            new SceneLoadObserver(_miniGameState, "Scenes/NumbersChallenge").Observe(this);
            StartCoroutine(CheckPlayerDeath());
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
    }
}