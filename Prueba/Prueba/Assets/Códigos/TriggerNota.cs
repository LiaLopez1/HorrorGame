using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerNota : MonoBehaviour
{
    [Header("Configuración Diálogo")]
    public DialogueData[] dialogos;
    public float dialogueDuration = 3f;
    public float delayAfterClose = 0.5f;

    [Header("Referencias UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text missionText;

    [Header("Configuración Progresión")]
    public GameManager gameManager; // Referencia asignable desde el Inspector
    public GameManager.GameState nextState;
    public bool disableInsteadOfDestroy = false;

    private LeerNotas noteReader;
    private bool isRunningDialogue = false;

    void Start()
    {
        // 1. Obtener referencia al componente LeerNotas
        noteReader = GetComponent<LeerNotas>();

        // 2. Conexión automática de eventos
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction += OnNoteOpened;
            noteReader.OnNoteClosedAction += HandleNoteClosed;
        }

        // 3. Buscar GameManager si no está asignado
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("No se encontró GameManager en la escena!");
            }
        }

        // 4. Asegurar que el panel de diálogo está oculto al inicio
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Limpieza de eventos para evitar memory leaks
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction -= OnNoteOpened;
            noteReader.OnNoteClosedAction -= HandleNoteClosed;
        }
    }

    private void OnNoteOpened()
    {
        Debug.Log("Nota abierta: " + gameObject.name);
    }

    private void HandleNoteClosed()
    {
        if (!isRunningDialogue)
        {
            StartCoroutine(DialogueSequence());
        }
    }

    private IEnumerator DialogueSequence()
    {
        isRunningDialogue = true;

        // Esperar un frame para asegurar estabilidad
        yield return null;

        // Mostrar panel de diálogo
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Mostrar todos los diálogos
        foreach (var dialogo in dialogos)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogo.texto;
            }

            if (dialogo.mostrarMision && missionText != null)
            {
                GameManager.Instance.UpdateMission(dialogo.textoMision);
            }

            yield return new WaitForSeconds(dialogueDuration);
        }

        // Ocultar panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Cambiar estado del juego (con verificación de null)
        if (gameManager != null)
        {
            gameManager.ChangeState(nextState);
        }
        else
        {
            Debug.LogWarning("GameManager no asignado - No se cambió el estado");
        }

        // Desactivar/Destruir la nota
        if (disableInsteadOfDestroy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

        isRunningDialogue = false;
    }
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 5)] public string texto;
    public bool mostrarMision;
    [TextArea(2, 3)] public string textoMision;
}