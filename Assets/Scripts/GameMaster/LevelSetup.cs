using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class LevelSetup : MonoBehaviour
    {
        public Transform player;
        public Transform challengeItem;
        public Transform exitItem;
        
        private State _miniGameState;
        private IntentState _miniGameOverlayState;
        private IntentState _pauseOverlayState;
        private State _tooltipVisibilityState;

        private void Start()
        {
            _miniGameState = ServiceLocator.Get.Locate<State>("miniGamePassedState");
            _miniGameOverlayState = ServiceLocator.Get.Locate<IntentState>("miniGameOverlayState");
            _pauseOverlayState = ServiceLocator.Get.Locate<IntentState>("pauseOverlayState");
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            CharacterHealthHolder.GetInstance().Set(CharacterHealthHolder.GetMax);
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(OpenScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(CloseScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(OpenScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(CloseScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(CheckHealthStatus());
            StartCoroutine(CheckExitDistance());
        }

        private void Update()
        {
            HandlePauseMenuClick();
            HandleExitClick();
        }

        private void HandlePauseMenuClick()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            _pauseOverlayState.Toggle();
        }
        
        private void HandleExitClick()
        {
            var pressed = Input.GetKeyDown("e");
            if (!pressed) return;
            if (!ExitItemMarker.Controller.Get) return;
            if (!_miniGameState.Get) return;
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator OpenScene(IntentState sceneStateController, string sceneName)
        {
            while (true)
            {
                yield return new WaitUntil(sceneStateController.ShouldBeOpened);
                sceneStateController.SubmitOpen();
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator CloseScene(IntentState sceneStateController, string sceneName)
        {
            while (true)
            {
                yield return new WaitUntil(sceneStateController.ShouldBeClosed);
                sceneStateController.SubmitClose();
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator CheckHealthStatus()
        {
            while (true)
            {
                if (CharacterHealthHolder.GetInstance().Get <= 0)
                {
                    SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
                }

                yield return null;
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckExitDistance()
        {
            while (true)
            {
                if (Vector3.Distance(player.position, exitItem.position) < 5)
                {
                    if (_miniGameState.Get)
                    {
                        _tooltipVisibilityState.Activate();
                        ExitItemMarker.Controller.Activate();
                    }
                }
                else
                {
                    _tooltipVisibilityState.Deactivate();
                    ExitItemMarker.Controller.Deactivate();
                }
                yield return null;
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckExitStatus()
        {
            yield return null;
        }
    }
}