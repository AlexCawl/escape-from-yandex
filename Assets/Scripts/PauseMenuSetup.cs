using UnityEngine;

public class PauseMenuSetup : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    public void UnPause()
    {
        PauseManager.Toggle();
    }
}
