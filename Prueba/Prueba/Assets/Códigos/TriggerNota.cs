using UnityEngine;
using UnityEngine.Events; // Añade esto para usar UnityEvent
using TMPro;
using System.Collections;
using FMODUnity;


public class TriggerNota : MonoBehaviour
{
    [Header("Configuración Diálogo")]
    public DialogueData[] dialogos;
    public float dialogueDuration = 3f;

    [Header("Referencias UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Configuración Progresión")]
    public bool esMisionPrincipal = false;
    public GameManager.GameState nextState;
    public bool disableInsteadOfDestroy = false;
    public GameManager gameManager;

    [Header("Eventos al Recoger")]
    public UnityEvent onNoteCollected; // Evento que se dispara al recoger la nota

    private LeerNotas noteReader;
    private bool isRunningDialogue = false;

    [Header("FMOD")]
    [Tooltip("Sonido que se reproduce al abrir la nota")]
    public EventReference sonidoNota;


    void Start()
    {
        noteReader = GetComponent<LeerNotas>();
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction += OnNoteOpened;
            noteReader.OnNoteClosedAction += HandleNoteClosed;
        }
        else
        {
            Debug.LogError("Componente LeerNotas no encontrado", this);
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private IEnumerator DialogueSequence()
    {
        isRunningDialogue = true;

        // Disparar evento al recoger la nota
        onNoteCollected.Invoke(); // ¡Aquí se ejecutarán las acciones vinculadas en el Inspector!

        // Mostrar diálogos
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        foreach (var dialogo in dialogos)
        {
            if (dialogueText != null) dialogueText.text = dialogo.texto;

            if (dialogo.mostrarMision && GameManager.Instance != null)
            {
                if (esMisionPrincipal)
                    GameManager.Instance.ActualizarMisionPrincipal(dialogo.textoMision);
                else
                    GameManager.Instance.ActualizarMisionSecundaria(dialogo.textoMision);
            }

            yield return new WaitForSeconds(dialogueDuration);
        }

        // Finalización
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (gameManager != null)
            gameManager.ChangeState(nextState);

        if (disableInsteadOfDestroy)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);

        isRunningDialogue = false;
    }

    void OnDestroy()
    {
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction -= OnNoteOpened;
            noteReader.OnNoteClosedAction -= HandleNoteClosed;
        }
    }

    private void OnNoteOpened()
    {
        Debug.Log("Nota abierta: " + name);

        if (!sonidoNota.IsNull)
        {
            RuntimeManager.PlayOneShot(sonidoNota, transform.position);
        }
    }

    private void HandleNoteClosed() { if (!isRunningDialogue) StartCoroutine(DialogueSequence()); }
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 5)] public string texto;
    public bool mostrarMision;
    [TextArea(2, 3)] public string textoMision;
}