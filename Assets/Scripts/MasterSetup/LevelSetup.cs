using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSetup : MonoBehaviour
{
    private bool _isPauseAlreadyActive;

    private void Start()
    {
        StartCoroutine(OpenPauseMenu());
        StartCoroutine(ClosePauseMenu());
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

    private void Update()
    {
        var pressed = Input.GetKeyDown("p");
        if (!pressed) return;
        PauseManager.Controller.Toggle();
    }
}