using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster
{
    public class SplashScreenSetup : MonoBehaviour
    {
        public void SetupPlayButton()
        {
            Debug.Log("TODO - PLAY");
        }

        public void SetupTutorialButton()
        {
            Debug.Log("TODO - TUTORIAL");
        }

        public void SetupDebugButton()
        {
            SceneManager.LoadSceneAsync("Scenes/SampleScene");
        }
    }
}