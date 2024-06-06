using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker Instance { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (PlayerTransform == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
    }
}