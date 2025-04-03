using UnityEngine;
using FMOD.Studio;
using FMODUnity; 

public class MusicController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform _audioSourceTransform; // Referencia 3D (jugador/cámara)
    [SerializeField] private string stateParameter = "opciones"; // Parámetro labeled

    [Header("Distancias")]
    [SerializeField] private Transform audioSourceTransform;
    [SerializeField] private string enemyTag = "Enemigo";
    [SerializeField] private float mediumDistance = 10f;
    [SerializeField] private float dangerDistance = 2f;

    private EventInstance musicInstance;
    private int currentState = 0;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Music");
        Update3DAttributes();
        SetMusicState(1); // Estado inicial (menú)
         musicInstance.start();
    }

    void Update()
    {
        if (currentState == 1) // Solo actualiza distancia si está en música de juego
        {
            float distance = GetDistanceToClosestEnemy();
            int newSubState = CalculateSubState(distance);
            musicInstance.setParameterByName("distancia", newSubState); // Parámetro de distancia
        }
    }

    public void SetMusicState(int newState)
    {
        currentState = Mathf.Clamp(newState, 0, 2);
        musicInstance.setParameterByName(stateParameter, currentState);

        // Valores posibles del parámetro labeled:
        // 0 = Menú
        // 1 = Main (juego)
        // 2 = Game Over
    }

    private int CalculateSubState(float distance)
    {
        if (distance <= dangerDistance) return 2;
        if (distance <= mediumDistance) return 1;
        return 0;
    }

    private float GetDistanceToClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        if (enemies.Length == 0) return Mathf.Infinity;

        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(_audioSourceTransform.position, enemy.transform.position);
            closestDistance = Mathf.Min(distance, closestDistance);
        }
        return closestDistance;
    }

    private void Update3DAttributes()
    {
        if (_audioSourceTransform != null)
        {
            musicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_audioSourceTransform));
        }
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}

