using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FMODUnity;

public class DesbloquearObj : MonoBehaviour
{
    [Header("Estado del objeto")]
    public bool estaBloqueado = true;
    public bool desaparecerAlDesbloquear = false;

    [Header("Distancia de interacción")]
    public float distanciaInteraccion = 3f;

    [Header("UI de interacción")]
    public GameObject panelBloqueado;
    public GameObject panelDesbloqueado;

    [Header("Diálogo Bloqueado")]
    public Text textoDialogo;
    public string mensajeBloqueado = "Este objeto está bloqueado.";
    public float tiempoOcultarDialogo = 3f;

    [Header("FMOD")]
    public EventReference sonidoDesbloqueo;

    [Header("Eventos")]
    public UnityEvent onDesbloqueado;

    private bool jugadorCerca = false;
    private Transform camaraJugador;
    private bool estaMirandoObjeto = false;

    void Start()
    {
        camaraJugador = Camera.main.transform;

        if (panelBloqueado != null) panelBloqueado.SetActive(false);
        if (panelDesbloqueado != null) panelDesbloqueado.SetActive(false);
        if (textoDialogo != null) textoDialogo.text = "";
    }

    void Update()
    {
        RevisarDistanciaJugador();
        VerificarRaycast();

        if (jugadorCerca && estaMirandoObjeto && Input.GetKeyDown(KeyCode.E))
        {
            if (estaBloqueado)
            {
                MostrarPanelBloqueado();
                MostrarDialogoBloqueado();
            }
            else
            {
                MostrarPanelDesbloqueado();
                onDesbloqueado.Invoke();
            }
        }
    }

    void VerificarRaycast()
    {
        estaMirandoObjeto = false;

        if (!jugadorCerca) return;

        Ray ray = new Ray(camaraJugador.position, camaraJugador.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == gameObject)
            {
                estaMirandoObjeto = true;
            }
        }

        ActualizarPaneles();
    }

    void RevisarDistanciaJugador()
    {
        float distancia = Vector3.Distance(camaraJugador.position, transform.position);
        bool cerca = distancia <= distanciaInteraccion;

        if (cerca != jugadorCerca)
        {
            jugadorCerca = cerca;
            if (!jugadorCerca) estaMirandoObjeto = false;
            ActualizarPaneles();
        }
    }

    void ActualizarPaneles()
    {
        if (jugadorCerca && estaMirandoObjeto)
        {
            if (estaBloqueado)
            {
                if (panelBloqueado != null) panelBloqueado.SetActive(true);
                if (panelDesbloqueado != null) panelDesbloqueado.SetActive(false);
            }
            else
            {
                if (panelBloqueado != null) panelBloqueado.SetActive(false);
                if (panelDesbloqueado != null) panelDesbloqueado.SetActive(true);
            }
        }
        else
        {
            if (panelBloqueado != null) panelBloqueado.SetActive(false);
            if (panelDesbloqueado != null) panelDesbloqueado.SetActive(false);
        }
    }

    // Resto de los métodos permanecen igual...
    public void Desbloquear()
    {
        estaBloqueado = false;
        ActualizarPaneles();

        if (!sonidoDesbloqueo.IsNull)
        {
            RuntimeManager.PlayOneShot(sonidoDesbloqueo, transform.position);
        }

        if (desaparecerAlDesbloquear)
        {
            Destroy(gameObject, 0.1f);
        }
    }

    void MostrarPanelBloqueado()
    {
        if (panelBloqueado != null) panelBloqueado.SetActive(true);
        if (panelDesbloqueado != null) panelDesbloqueado.SetActive(false);
    }

    void MostrarPanelDesbloqueado()
    {
        if (panelBloqueado != null) panelBloqueado.SetActive(false);
        if (panelDesbloqueado != null) panelDesbloqueado.SetActive(true);
    }

    void MostrarDialogoBloqueado()
    {
        if (textoDialogo != null)
        {
            textoDialogo.text = mensajeBloqueado;
            CancelInvoke(nameof(OcultarDialogo));
            Invoke(nameof(OcultarDialogo), tiempoOcultarDialogo);
        }
    }

    void OcultarDialogo()
    {
        if (textoDialogo != null)
        {
            textoDialogo.text = "";
        }
    }
}