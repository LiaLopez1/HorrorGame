using UnityEngine;
using FMOD.Studio;
using FMODUnity; 

public class MusicController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform _audioSourceTransform; // Referencia 3D (jugador/cámara)
    [SerializeField] private string _enemyTag = "Enemigo";
    [SerializeField] private float _mediumDistance = 5f; // Estado 1
    [SerializeField] private float _dangerDistance = 3f; // Estado 2

    private EventInstance _musicInstance;
    private int _currentState = 0;

    void Start()
    {
        // Usa AudioManager para crear la instancia del evento de música
        _musicInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.MusicEvent); // Necesitarás agregar "MusicEvent" en FMODEvents
        Update3DAttributes();
        _musicInstance.start();
    }

    void Update()
    {
        float distance = GetDistanceToClosestEnemy();
        int newState = CalculateState(distance);
        
        if (newState != _currentState)
        {
            _currentState = newState;
            _musicInstance.setParameterByName("musicgame", _currentState);
        }

        Update3DAttributes(); // Actualiza posición 3D (opcional: puedes hacerlo en intervalos)
    }

    private float GetDistanceToClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(_enemyTag);
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(_audioSourceTransform.position, enemy.transform.position);
            if (distance < closestDistance) closestDistance = distance;
        }
        return closestDistance;
    }

    private int CalculateState(float distance)
    {
        if (distance <= _dangerDistance) return 2;
        if (distance <= _mediumDistance) return 1;
        return 0;
    }

    private void Update3DAttributes()
    {
        if (_audioSourceTransform != null)
        {
            _musicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_audioSourceTransform));
        }
    }

    void OnDestroy()
    {
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicInstance.release();
    }
}