using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameMaster.Setup
{
    public class DefeatSetup : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
