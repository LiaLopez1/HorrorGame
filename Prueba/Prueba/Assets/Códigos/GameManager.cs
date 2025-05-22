using UnityEngine;
using TMPro;
using System.Collections;
using FMODUnity; // Asegúrate de incluir esto
using FMOD.Studio;
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

    public void ActualizarMision(string texto, bool esPrincipal, float duracion = 0f)
    {
        TMP_Text textoMision = esPrincipal ? misionPrincipalText : misionSecundariaText;

        if (textoMision != null)
        {
            textoMision.text = texto;
            textoMision.gameObject.SetActive(true);

            // Solo ocultar si es misión secundaria Y se especificó duración > 0
            if (!esPrincipal && duracion > 0)
            {
                StartCoroutine(OcultarMisionTemporal(textoMision, duracion));
            }
        }
    }
    private IEnumerator OcultarMisionTemporal(TMP_Text textoUI, float duracion)
    {
        yield return new WaitForSeconds(duracion);
        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(false);
        }
    }


    public void ActualizarMisionSecundaria(string texto)
    {
        if (misionSecundariaText != null)
        {
            misionSecundariaText.text = $"{texto}";
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