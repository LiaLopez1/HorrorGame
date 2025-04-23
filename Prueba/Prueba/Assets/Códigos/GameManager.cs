using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Inicio,
        NotaRecepcionLeida,
        NotaConsultorioLeida
    }

    [Header("Estado Actual")]
    public GameState currentState = GameState.Inicio;
    public int dialogueSequence = 0;

    [Header("UI Misiones")]
    public TMP_Text misionPrincipalText;
    public TMP_Text misionSecundariaText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"Estado cambiado a: {currentState}");
    }

    public void ActualizarMisionPrincipal(string texto)
    {
        if (misionPrincipalText != null)
        {
            misionPrincipalText.text = $"<color=yellow>PRINCIPAL:</color> {texto}";
            misionPrincipalText.gameObject.SetActive(true);
        }
    }

    public void ActualizarMisionSecundaria(string texto)
    {
        if (misionSecundariaText != null)
        {
            misionSecundariaText.text = $"<color=white>SECUNDARIA:</color> {texto}";
            misionSecundariaText.gameObject.SetActive(true);
        }
    }

    public void LimpiarMisionSecundaria()
    {
        if (misionSecundariaText != null)
            misionSecundariaText.text = "";
    }

    public void AdvanceDialogueSequence()
    {
        dialogueSequence++;
        Debug.Log("Secuencia de diálogo: " + dialogueSequence);
    }
}