using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Diagnostics;

public class MusicController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform _audioSourceTransform; // Cámara o jugador
    [SerializeField] private string stateParameter = "opciones"; // Parámetro labeled

    private EventInstance musicInstance;
    private int currentState = 0;
    private int lastIntensity = -1;

    private MonsterMovement monster;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Music");
        Update3DAttributes();
        SetMusicState(1); // Estado de juego por defecto
        musicInstance.start();

        monster = FindObjectOfType<MonsterMovement>();
    }

    void Update()
    {
        if (currentState != 1 || monster == null) return;

        int targetIntensity = 0;

        if (monster.playerInNormalArea)
            targetIntensity = 2;
        else if (monster.playerInExtendedArea)
            targetIntensity = 1;

        if (targetIntensity != lastIntensity)
        {
            musicInstance.setParameterByName("distancia", targetIntensity);
            lastIntensity = targetIntensity;
        }
    }

    public void SetMusicState(int newState)
    {
        currentState = Mathf.Clamp(newState, 0, 2);
        musicInstance.setParameterByName(stateParameter, currentState);
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
