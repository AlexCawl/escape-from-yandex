using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class PauseMenuSetup : MonoBehaviour
    {
        private void Start()
        {
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        public void Resume()
        {
            PauseManager.Controller.Toggle();
        }

        public void BackToMainMenu()
        {
            Resume();
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
