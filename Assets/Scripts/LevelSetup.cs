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
            yield return new WaitUntil(PauseManager.ShouldBeOpened);
            PauseManager.Open();
            SceneManager.LoadSceneAsync("Scenes/PauseMenu", LoadSceneMode.Additive);
        }
    }

    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    private static IEnumerator ClosePauseMenu()
    {
        while (true)
        {
            yield return new WaitUntil(PauseManager.ShouldBeClosed);
            PauseManager.Close();
            SceneManager.UnloadSceneAsync("Scenes/PauseMenu");
        }
    }

    private void Update()
    {
        var pressed = Input.GetKeyDown("p");
        if (!pressed) return;
        PauseManager.Toggle();
    }
}

public static class PauseManager
{
    private static bool _pauseState;
    private static bool _pauseNextState;

    internal static void Toggle()
    {
        _pauseNextState = !_pauseNextState;
    }

    public static void Open()
    {
        _pauseState = true;
    }

    public static void Close()
    {
        _pauseState = false;
    }

    public static bool ShouldBeOpened() => _pauseState == false && _pauseNextState;

    public static bool ShouldBeClosed() => _pauseState && _pauseNextState == false;
}