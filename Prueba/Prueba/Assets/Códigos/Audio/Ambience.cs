using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Diagnostics;

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
        while (true)
        {
            yield return StartCoroutine(PlayRandomAmbientSound());

            // Espera aleatoria entre sonidos
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator PlayRandomAmbientSound()
    {
        if (ambientSounds.Length == 0 || playerTransform == null) yield break;

        // Elegir evento aleatorio
        EventReference chosenSound = ambientSounds[UnityEngine.Random.Range(0, ambientSounds.Length)];

        // Posición aleatoria en anillo
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minRadius, maxRadius);
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Crear instancia
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
