using UnityEngine;
using FMODUnity;

public class MonsterRandomSounds : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference screamEvent; // Evento de gritos
    [SerializeField] private EventReference growlEvent;  // Evento de gruñidos

    [Header("Timing Settings")]
    [SerializeField][Range(60, 120)] private float minDelay = 60f; // 2 minutos
    [SerializeField][Range(60, 120)] private float maxDelay = 120f; // 4 minutos

    [Header("Debug Settings")]
    [SerializeField] private KeyCode testKey = KeyCode.G; // Tecla para pruebas

    private float nextSoundTime;
    private float timer = 0f;

    void Start()
    {
        nextSoundTime = Random.Range(minDelay, maxDelay);
    }

    void Update()
    {
        // Reproducción automática cada 2-4 minutos
        timer += Time.deltaTime;
        if (timer >= nextSoundTime)
        {
            PlayRandomSound();
            ResetTimer();
        }

        // Reproducción manual con la tecla G (para pruebas)
        if (Input.GetKeyDown(testKey))
        {
            Debug.Log("Reproduciendo sonido manualmente...");
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        // Elige aleatoriamente entre gritar o gruñir (50% de probabilidad)
        bool shouldScream = Random.Range(0, 2) == 0;
        EventReference selectedEvent = shouldScream ? screamEvent : growlEvent;

        FMOD.Studio.EventInstance soundInstance = RuntimeManager.CreateInstance(selectedEvent);
        RuntimeManager.AttachInstanceToGameObject(soundInstance, transform, GetComponent<Rigidbody>());
        soundInstance.start();
        soundInstance.release();

        Debug.Log(shouldScream ? "¡Grito emitido!" : "¡Gruñido emitido!");
    }

    void ResetTimer()
    {
        timer = 0f;
        nextSoundTime = Random.Range(minDelay, maxDelay);
        Debug.Log($"Próximo sonido automático en {nextSoundTime} segundos");
    }
}