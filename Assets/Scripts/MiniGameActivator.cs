using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameActivator : MonoBehaviour
{
    private bool _isOverlayed;

    private void Awake()
    {
        _isOverlayed = false;
    }

    private void Update()
    {
        var pressed = Input.GetKeyDown("g");
        if (!pressed) return;
        _isOverlayed = !_isOverlayed;
        StartCoroutine(ToggleMiniGame(_isOverlayed));
    }

    private static IEnumerator ToggleMiniGame(bool activated)
    {
        if (activated)
        {
            SceneManager.LoadSceneAsync("Scenes/NumbersChallenge", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync("Scenes/NumbersChallenge");
        }
        yield return null;
    }
}
