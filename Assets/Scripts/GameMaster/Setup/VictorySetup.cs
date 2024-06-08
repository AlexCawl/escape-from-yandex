using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameMaster.Setup
{
    public class VictorySetup : MonoBehaviour
    {
        public Text title;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(5f);
            title.text = "Congratulations";
            yield return new WaitForSeconds(3f);
            title.text = "You have completed the game :)";
            yield return new WaitForSeconds(5f);
            SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
        }
    }
}
