using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class PauseMenuSetup : MonoBehaviour
    {
        private IntentState _pauseOverlayState;
        
        private void Start()
        {
            Time.timeScale = 0f;
            _pauseOverlayState = ServiceLocator.Get.Locate<IntentState>("pauseOverlayState");
        }

        private void OnDestroy() => Time.timeScale = 1f;

        public void Resume() => _pauseOverlayState.Toggle();

        public void BackToMainMenu()
        {
            Resume();
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
