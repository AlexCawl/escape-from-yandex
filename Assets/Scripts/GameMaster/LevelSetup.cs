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

        private void Start()
        {
            CharacterHealthHolder.GetInstance().Set(CharacterHealthHolder.GetMax);
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(OpenScene(PauseManager.Controller, "Scenes/PauseMenu"));
            StartCoroutine(CloseScene(PauseManager.Controller, "Scenes/PauseMenu"));
            StartCoroutine(OpenScene(ChallengeManager.Controller, "Scenes/NumbersChallenge"));
            StartCoroutine(CloseScene(ChallengeManager.Controller, "Scenes/NumbersChallenge"));
            StartCoroutine(CheckHealthStatus());
            StartCoroutine(CheckChallengeItemDistance());
        }

        private void Update()
        {
            HandlePauseMenuClick();
            HandleChallengeMenuClick();
        }

        private static void HandlePauseMenuClick()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            PauseManager.Controller.Toggle();
        }
        
        private static void HandleChallengeMenuClick()
        {
            var pressed = Input.GetKeyDown("e");
            if (!pressed) return;
            if (!ChallengeItemMarker.Controller.State) return;
            ChallengeManager.Controller.Toggle();
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator OpenScene(OverlayManager sceneStateController, string sceneName)
        {
            while (true)
            {
                yield return new WaitUntil(sceneStateController.ShouldBeOpened);
                sceneStateController.SubmitOpen();
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator CloseScene(OverlayManager sceneStateController, string sceneName)
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
        private IEnumerator CheckChallengeItemDistance()
        {
            while (true)
            {
                if (Vector3.Distance(player.position, challengeItem.position) < 5)
                {
                    TooltipMarker.Controller.Activate();
                    ChallengeItemMarker.Controller.Activate();
                }
                else if (Vector3.Distance(player.position, exitItem.position) < 5)
                {
                    TooltipMarker.Controller.Activate();
                }
                else
                {
                    TooltipMarker.Controller.Deactivate();
                    ChallengeItemMarker.Controller.Deactivate();
                }
                yield return null;
            }
        }
    }
}