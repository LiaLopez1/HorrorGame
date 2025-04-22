using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AmbientSoundSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private EventReference[] ambientSounds;
    [SerializeField] private float minDelay = 120f; // 2 minutos
    [SerializeField] private float maxDelay = 180f; // 3 minutos

    [Header("Área alrededor del jugador")]
    [SerializeField] private float minRadius = 10f;
    [SerializeField] private float maxRadius = 30f;

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        StartCoroutine(SpawnAmbientSoundsLoop());
    }

    IEnumerator SpawnAmbientSoundsLoop()
    {
        // Espera inicial antes del primer sonido
        float initialDelay = UnityEngine.Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            yield return StartCoroutine(PlayRandomAmbientSound());

            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator PlayRandomAmbientSound()
    {
        if (ambientSounds.Length == 0 || playerTransform == null) yield break;

        // Elegir evento aleatorio
        EventReference chosenSound = ambientSounds[UnityEngine.Random.Range(0, ambientSounds.Length)];

        // Generar posición aleatoria dentro de un anillo (entre minRadius y maxRadius)
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float radius = UnityEngine.Random.Range(minRadius, maxRadius);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 spawnPosition = playerTransform.position + offset;

        // Crear y reproducir el sonido
        EventInstance instance = RuntimeManager.CreateInstance(chosenSound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(spawnPosition));
        instance.start();

        // Esperar a que termine el sonido
        bool isPlaying = true;
        while (isPlaying)
        {
            instance.getPlaybackState(out PLAYBACK_STATE state);
            isPlaying = state != PLAYBACK_STATE.STOPPED;
            yield return null;
        }

        instance.release();
    }
}
