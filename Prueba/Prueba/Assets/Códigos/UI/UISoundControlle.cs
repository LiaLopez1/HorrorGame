using UnityEngine;
using UnityEngine.EventSystems;
using FMOD.Studio;
using FMODUnity;

[RequireComponent(typeof(EventTrigger))]
public class UISoundController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool _isExitButton = false;
    [SerializeField] [Range(0.1f, 1f)] private float _clickPriorityDelay = 0.1f; // Nuevo: Tiempo para dar prioridad al click

    private EventInstance _uiSoundInstance;
    private float _lastInteractionTime;
    private string _lastAction;

    void Start()
    {
        _uiSoundInstance = RuntimeManager.CreateInstance(FMODEventsUI.Instance.soundsui);
        SetupTriggers();
    }

    private void SetupTriggers()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        // Hover
        AddListener(trigger, EventTriggerType.PointerEnter, "hover");

        // Click
        AddListener(trigger, EventTriggerType.PointerUp, "click");

        // Exit (si es un botón de salida)
        if (_isExitButton)
            AddListener(trigger, EventTriggerType.PointerUp, "exit");
    }

    private void AddListener(EventTrigger trigger, EventTriggerType type, string action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = type,
            callback = new EventTrigger.TriggerEvent()
        };
        entry.callback.AddListener((data) => ProcessSound(action));
        trigger.triggers.Add(entry);
    }

   
    private void ProcessSound(string action)
    {
        // Si es un click, siempre tiene prioridad
        if (action == "click")
        {
            PlaySound(action);
            _lastInteractionTime = Time.time;
        }
        // Para hover, solo reproducir si no hubo un click reciente
        else if (action == "hover" && Time.time - _lastInteractionTime > _clickPriorityDelay)
        {
            PlaySound(action);
        }
        // Exit siempre se reproduce
        else if (action == "exit")
        {
            PlaySound(action);
        }
    }

    private void PlaySound(string action)
    {
        _uiSoundInstance.setParameterByNameWithLabel("Ui", action);
        _uiSoundInstance.start();
        Debug.Log($"Sonido reproducido: {action}"); // Para debug
    }

    void OnDestroy()
    {
        if (_uiSoundInstance.isValid())
        {
            _uiSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); 
            _uiSoundInstance.release();
        }
    }
}