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
        private bool _isChallengeAccepted;

        private void Start()
        {
            CharacterHealthHolder.GetInstance().Set(CharacterHealthHolder.GetMax);
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(OpenPauseMenu());
            StartCoroutine(ClosePauseMenu());
            StartCoroutine(CheckHealthStatus());
            StartCoroutine(CheckChallengeItemDistance());
        }

        private void Update()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            PauseManager.Controller.Toggle();
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator OpenPauseMenu()
        {
            while (true)
            {
                yield return new WaitUntil(PauseManager.Controller.ShouldBeOpened);
                PauseManager.Controller.SubmitOpen();
                SceneManager.LoadSceneAsync("Scenes/PauseMenu", LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator ClosePauseMenu()
        {
            while (true)
            {
                yield return new WaitUntil(PauseManager.Controller.ShouldBeClosed);
                PauseManager.Controller.SubmitClose();
                SceneManager.UnloadSceneAsync("Scenes/PauseMenu");
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
                }
                else if (Vector3.Distance(player.position, exitItem.position) < 5)
                {
                    TooltipMarker.Controller.Activate();
                }
                else
                {
                    TooltipMarker.Controller.Deactivate();
                }
                yield return null;
            }
        }
    }
}