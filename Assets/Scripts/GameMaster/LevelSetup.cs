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
        
        private State _exitOpenState;
        private IntentState _miniGameOverlayState;
        private IntentState _pauseOverlayState;

        private void Start()
        {
            _miniGameOverlayState = ServiceLocator.Get.Locate<IntentState>("miniGameOverlayState");
            _pauseOverlayState = ServiceLocator.Get.Locate<IntentState>("pauseOverlayState");
            _exitOpenState = ServiceLocator.Get.Locate<State>("exitOpenState");
            CharacterHealthHolder.GetInstance().Set(CharacterHealthHolder.GetMax);
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(OpenScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(CloseScene(_pauseOverlayState, "Scenes/PauseMenu"));
            StartCoroutine(OpenScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(CloseScene(_miniGameOverlayState, "Scenes/NumbersChallenge"));
            StartCoroutine(CheckPlayerHealth());
            StartCoroutine(CheckLevelPassed());
        }

        private void Update()
        {
            HandlePauseMenuClick();
        }

        private void HandlePauseMenuClick()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            _pauseOverlayState.Toggle();
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
        private static IEnumerator CheckPlayerHealth()
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
        private IEnumerator CheckLevelPassed()
        {
            while (true)
            {
                if (_exitOpenState.Get)
                {
                    SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
                    yield break;
                }
                yield return null;
            }
        }
    }
}