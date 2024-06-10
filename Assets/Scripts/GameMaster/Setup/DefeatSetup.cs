using System.Collections;
using GameMaster.State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class DefeatSetup : MonoBehaviour
    {
        private IEnumerator Start()
        {
            ServiceLocator.Get.Locate<GameLevelState>().Reset();
            ServiceLocator.Get.Locate<NumberState>("playerHealth").Reset();
            yield return new WaitForSeconds(5f);
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
