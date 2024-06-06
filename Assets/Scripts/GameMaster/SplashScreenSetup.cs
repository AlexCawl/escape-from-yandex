using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class SplashScreenSetup : MonoBehaviour
    {
        public void SetupPlayButton()
        {
            SceneManager.LoadSceneAsync("Scenes/LevelScene");
        }

        public void SetupTutorialButton()
        {
            SceneManager.LoadSceneAsync("Scenes/SampleScene");
        }
    }
}