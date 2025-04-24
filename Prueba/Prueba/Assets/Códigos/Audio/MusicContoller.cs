using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Diagnostics;

public class MusicController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform _audioSourceTransform; // Cámara o jugador
    [SerializeField] private string stateParameter = "opciones"; // Parámetro labeled

    private MonsterMovement[] monsters;

    private EventInstance musicInstance;
    private int currentState = 0;
    private int lastIntensity = -1;


    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance("event:/Music/Music");
        Update3DAttributes();
        SetMusicState(1);
        musicInstance.start();

        monsters = FindObjectsOfType<MonsterMovement>();
    }


    void Update()
    {
        if (currentState != 1 || monsters == null) return;

        int targetIntensity = 0;

        foreach (var m in monsters)
        {
            if (m.IsPlayerInNormalRange)
            {
                targetIntensity = 2;
                break;
            }
            if (m.IsExtendedZoneTriggered)
            {
                targetIntensity = 1;
            }

        }

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
