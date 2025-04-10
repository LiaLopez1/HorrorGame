using UnityEngine;

public class NotaInteractiva : MonoBehaviour
{
    public string textoNota = "Las llaves están en un consultorio. Para salir necesito encontrarlas.";
    public float tiempoEnPantalla = 5f;
    public GameManager.GameState estadoADisparar = GameManager.GameState.LeyoNota1;

    public GameObject panelUIInteraccion;

    private bool jugadorCerca = false;
    private bool yaLeida = false;

    void Update()
    {
        if (jugadorCerca && !yaLeida && Input.GetKeyDown(KeyCode.E))
        {
            yaLeida = true;

            if (panelUIInteraccion != null)
                panelUIInteraccion.SetActive(false);

            GameManager gm = FindObjectOfType<GameManager>();
            gm.MostrarTexto(textoNota);
            gm.ChangeState(estadoADisparar);
            gm.Invoke("OcultarTexto", tiempoEnPantalla);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (panelUIInteraccion != null)
                panelUIInteraccion.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (panelUIInteraccion != null)
                panelUIInteraccion.SetActive(false);
        }
    }
}
