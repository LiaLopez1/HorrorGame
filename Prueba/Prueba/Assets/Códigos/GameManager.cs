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

    [Header("UI")]
    public TMP_Text missionText;

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

        // Opcional: Registrar el cambio sin afectar misiones
        switch (newState)
        {
            case GameState.NotaRecepcionLeida:
                Debug.Log("El jugador leyó la nota de recepción");
                break;

            case GameState.NotaConsultorioLeida:
                Debug.Log("El jugador leyó la nota de consultorio");
                break;
        }
    }
    public void AdvanceDialogueSequence()
    {
        dialogueSequence++;
        Debug.Log("Secuencia de diálogo: " + dialogueSequence);
    }

    public void UpdateMission(string text)
    {
        if (missionText != null)
        {
            missionText.text = text;
            missionText.gameObject.SetActive(true);
        }
    }
}