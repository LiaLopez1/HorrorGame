using UnityEngine;
using UnityEngine.Events;
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
    public UnityEvent onNoteCollected;

    [Header("FMOD")]
    public EventReference sonidoNota;

    private LeerNotas noteReader;
    private bool hasDialogueBeenTriggered = false;

    void Start()
    {
        noteReader = GetComponent<LeerNotas>();
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction += OnNoteOpened;
            noteReader.OnNoteClosedAction += OnNoteClosed;
        }
        else
        {
            Debug.LogError("Componente LeerNotas no encontrado", this);
        }

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (noteReader != null)
        {
            noteReader.OnNoteOpenedAction -= OnNoteOpened;
            noteReader.OnNoteClosedAction -= OnNoteClosed;
        }
    }

    private void OnNoteOpened()
    {
        if (!sonidoNota.IsNull)
            RuntimeManager.PlayOneShot(sonidoNota, transform.position);
    }

    private void OnNoteClosed()
    {
        if (!hasDialogueBeenTriggered)
        {
            hasDialogueBeenTriggered = true;
            StartCoroutine(MostrarDialogoYFinalizar());
        }
    }

    private IEnumerator MostrarDialogoYFinalizar()
    {
        onNoteCollected.Invoke();

        // Mostrar panel de diálogo
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null && dialogos.Length > 0)
        {
            var dialogo = dialogos[0];
            dialogueText.text = dialogo.texto;

            // Actualizar misión usando textoMision
            if (dialogo.mostrarMision && GameManager.Instance != null)
            {
                GameManager.Instance.ActualizarMision(
                    dialogo.textoMision,
                    esMisionPrincipal
                );
            }
        }

        // Esperar duración configurada
        yield return new WaitForSeconds(dialogueDuration);

        // FORZAR cerrar el panel aunque esté activo por cualquier motivo
        if (dialoguePanel != null)
        {
            Debug.Log("Diálogo activo antes de forzar cierre: " + dialoguePanel.activeSelf);
            dialoguePanel.SetActive(false);
            Debug.Log("Diálogo activo después de forzar cierre: " + dialoguePanel.activeSelf);
        }

        // Cambiar estado del juego
        if (gameManager != null)
            gameManager.ChangeState(nextState);

        // Desactivar o destruir el objeto
        if (disableInsteadOfDestroy)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 5)] public string texto;          // Texto que se ve en el panel de diálogo
    public bool mostrarMision;
    [TextArea(2, 3)] public string textoMision;    // Texto que se asigna como misión
}
