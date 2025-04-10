using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Inicio,
        LeyoNota1
    }

    public GameState currentState = GameState.Inicio;

    public TextMeshProUGUI textoCentro;
    public TextMeshProUGUI textoMision;

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Estado cambiado a: " + newState.ToString());

        if (newState == GameState.LeyoNota1)
        {
            ActualizarMision("Buscar el consultorio mencionado en la nota.");
        }
    }

    public void MostrarTexto(string texto)
    {
        if (textoCentro != null)
        {
            textoCentro.text = texto;
            textoCentro.gameObject.SetActive(true);
        }
    }

    public void OcultarTexto()
    {
        if (textoCentro != null)
        {
            textoCentro.gameObject.SetActive(false);
        }
    }

    public void ActualizarMision(string nuevoTexto)
    {
        if (textoMision != null)
        {
            textoMision.text = "Misión: " + nuevoTexto;
        }
    }
}
