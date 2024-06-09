using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class PauseMenuSetup : MonoBehaviour
    {
        private SceneLoadState _pauseState;
        
        private void Start()
        {
            _pauseState = ServiceLocator.Get.Locate<SceneLoadState>("pauseState");
            Time.timeScale = 0f;
        }

        private void OnDestroy() => Time.timeScale = 1f;

        public void Resume() => _pauseState.Toggle();

        public void BackToMainMenu()
        {
            Resume();
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
