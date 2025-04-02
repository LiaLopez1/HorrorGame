using UnityEngine;
using FMODUnity;

public class FMODEventsUI : MonoBehaviour
{
    // UI Sounds
    [field: Header("ui")]
    [field: SerializeField] public EventReference soundsui { get; private set; }

    // Music
    [field: Header("Music")]
    [field: SerializeField] public EventReference musicui { get; private set; }

    public static FMODEventsUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}