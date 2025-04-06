using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Diagnostics;

public class MonsterRandomSounds : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference randomSoundEvent;

    [Header("Timing Settings")]
    [SerializeField][Range(60, 120)] private float minDelay = 60f;
    [SerializeField][Range(60, 120)] private float maxDelay = 120f;

    [Header("Debug Settings")]
    [SerializeField] private KeyCode testKey = KeyCode.G;

    private float nextSoundTime;
    private float timer = 0f;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSoundTime)
        {
            PlayRandomSound();
            ResetTimer();
        }

        if (Input.GetKeyDown(testKey))
        {
            UnityEngine.Debug.Log("Reproduciendo sonido manualmente...");
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        if (!randomSoundEvent.IsNull)
        {
            EventInstance instance = RuntimeManager.CreateInstance(randomSoundEvent);

            // Elegir aleatoriamente el tipo (0 = gruñido, 1 = grito)
            int tipo = UnityEngine.Random.Range(0, 2);
            instance.setParameterByName("tipo", tipo); // Asegúrate que el parámetro existe en FMOD

            RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
            instance.start();
            instance.release();
        }
    }

    void ResetTimer()
    {
        timer = 0f;
        nextSoundTime = UnityEngine.Random.Range(minDelay, maxDelay);
    }
}
