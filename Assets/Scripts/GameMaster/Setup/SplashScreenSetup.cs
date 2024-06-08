using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class SplashScreenSetup : MonoBehaviour
    {
        public void SetupPlayButton() => SceneManager.LoadSceneAsync("Scenes/Selector");

        public void SetupTutorialButton() => SceneManager.LoadSceneAsync("Scenes/SampleScene");

        public void SetupQuitButton() => Application.Quit();
    }
}