using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class LevelSetup : MonoBehaviour
    {
        private bool _isPauseAlreadyActive;

        private void Start()
        {
            CharacterHealthHolder.GetInstance().Set(CharacterHealthHolder.GetMax);
            SceneManager.LoadSceneAsync("Scenes/Ui", LoadSceneMode.Additive);
            StartCoroutine(OpenPauseMenu());
            StartCoroutine(ClosePauseMenu());
            StartCoroutine(CheckHealthStatus());
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator OpenPauseMenu()
        {
            while (true)
            {
                yield return new WaitUntil(PauseManager.Controller.ShouldBeOpened);
                PauseManager.Controller.Open();
                SceneManager.LoadSceneAsync("Scenes/PauseMenu", LoadSceneMode.Additive);
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private static IEnumerator ClosePauseMenu()
        {
            while (true)
            {
                yield return new WaitUntil(PauseManager.Controller.ShouldBeClosed);
                PauseManager.Controller.Close();
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

        private void Update()
        {
            var pressed = Input.GetKeyDown("p");
            if (!pressed) return;
            PauseManager.Controller.Toggle();
        }
    }
}