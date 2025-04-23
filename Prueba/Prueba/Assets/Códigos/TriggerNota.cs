using UnityEngine;
using TMPro;
using System.Collections;

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
    public GameManager gameManager; // Referencia manual opcional

    private LeerNotas noteReader;
    private bool isRunningDialogue = false;

    void Start()
    {
        // 1. Conexión con LeerNotas (totalmente restaurada)
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

        // 2. Búsqueda segura de GameManager
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager no encontrado en la escena", this);
            }
        }

        // 3. Inicialización de UI
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel de diálogo no asignado", this);
        }
    }

    private IEnumerator DialogueSequence()
    {
        isRunningDialogue = true;

        // Mostrar diálogos
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        foreach (var dialogo in dialogos)
        {
            if (dialogueText != null) dialogueText.text = dialogo.texto;

            if (dialogo.mostrarMision)
            {
                if (GameManager.Instance != null)
                {
                    if (esMisionPrincipal)
                        GameManager.Instance.ActualizarMisionPrincipal(dialogo.textoMision);
                    else
                        GameManager.Instance.ActualizarMisionSecundaria(dialogo.textoMision);
                }
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

    private void OnNoteOpened() => Debug.Log("Nota abierta: " + name);
    private void HandleNoteClosed() { if (!isRunningDialogue) StartCoroutine(DialogueSequence()); }
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 5)] public string texto;
    public bool mostrarMision;
    [TextArea(2, 3)] public string textoMision;
}