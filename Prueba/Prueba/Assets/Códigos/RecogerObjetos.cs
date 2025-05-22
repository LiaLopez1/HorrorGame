using UnityEngine;
using TMPro;
using System.Collections;
using FMODUnity; // Asegúrate de incluir esto
using FMOD.Studio;

public class RecogerObjetos : MonoBehaviour
{
    [Header("Configuración Visual")]
    public GameObject indicadorInteraccion;
    public float raycastDistance = 3f;

    [Header("Brillo (Contorno)")]
    public float distanciaBrillo = 6f;

    [Header("Configuración Diálogo")]
    [Tooltip("Mensajes que se mostrarán al recoger el objeto")]
    [TextArea(3, 5)] public string[] mensajesDialogo;
    [Tooltip("Tiempo que se muestra cada mensaje")]
    public float duracionDialogo = 3f;
    [Tooltip("Panel UI que contiene el texto del diálogo")]
    public GameObject panelDialogo;
    [Tooltip("Componente de texto donde se mostrarán los mensajes")]
    public TMP_Text textoDialogo;

    [Header("Configuración Misión")]
    public bool esMisionPrincipal = false;
    [TextArea(2, 3)] public string textoMision;
    [Tooltip("Tiempo que se muestra la notificación de misión")]
    public float duracionMision = 5f;

    [Header("Eventos")]
    public UnityEngine.Events.UnityEvent onRecoger;

    [Header("FMOD")]
    [Tooltip("Evento de sonido que se reproduce al recoger el objeto")]
    public EventReference sonidoRecoger;

    private Transform playerCamera;
    private bool isNear = false;
    private bool isCollected = false;
    private bool mostrandoDialogo = false;

    private OutlineController outlineController;

    void Start()
    {
        playerCamera = Camera.main.transform;

        // Buscar automáticamente el OutlineController
        outlineController = GetComponent<OutlineController>();

        if (indicadorInteraccion != null)
            indicadorInteraccion.SetActive(false);

        // Desactivar contorno al inicio
        outlineController?.HideOutline();

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    void Update()
    {

        if (isCollected || mostrandoDialogo) return;

        CheckPlayerLook();

        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            RecogerObjeto();
        }

        UpdateBrillo();
    }

    private void UpdateBrillo()
    {
        if (isCollected || outlineController == null) return;

        float distancia = Vector3.Distance(transform.position, playerCamera.position);

        if (distancia <= distanciaBrillo)
        {
            outlineController.ShowOutline();
        }
        else
        {
            outlineController.HideOutline();
        }
    }

    private void CheckPlayerLook()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        bool mirandoAhora = Physics.Raycast(ray, out RaycastHit hit, raycastDistance) &&
                            hit.collider.gameObject == gameObject;

        if (mirandoAhora != isNear)
        {
            isNear = mirandoAhora;

            if (indicadorInteraccion != null)
                indicadorInteraccion.SetActive(isNear);

            if (outlineController != null)
            {
                if (isNear) outlineController.ShowOutline();
                else outlineController.HideOutline();
            }
        }
    }

    private void RecogerObjeto()
    {
        isCollected = true;

        // 1️⃣ Ocultar el objeto visualmente (sin desactivarlo aún)
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        foreach (var collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;

        if (indicadorInteraccion != null)
            indicadorInteraccion.SetActive(false);

        outlineController?.HideOutline();

        // 2️⃣ Reproducir sonido
        if (!sonidoRecoger.IsNull)
            RuntimeManager.PlayOneShot(sonidoRecoger, transform.position);

        // 3️⃣ Mostrar misión (si existe)
        if (!string.IsNullOrEmpty(textoMision))
        {
            GameManager.Instance.ActualizarMision(textoMision, esMisionPrincipal, duracionMision);
        }

        onRecoger.Invoke();

        // 4️⃣ Mostrar diálogos (si existen)
        if (mensajesDialogo != null && mensajesDialogo.Length > 0)
            StartCoroutine(MostrarDialogos());
        else
            Destroy(gameObject); // Si no hay diálogos, destruir inmediatamente
    }

    private IEnumerator MostrarDialogos()
    {
        mostrandoDialogo = true;

        if (panelDialogo != null)
            panelDialogo.SetActive(true);

        foreach (string mensaje in mensajesDialogo)
        {
            if (textoDialogo != null)
                textoDialogo.text = mensaje;

            yield return new WaitForSeconds(duracionDialogo);
        }

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        mostrandoDialogo = false;

        // 5️⃣ Ahora sí destruir el objeto (después de los diálogos)
        Destroy(gameObject);
    }

    private IEnumerator MostrarMisionTemporal(TMP_Text textoMisionUI)
    {
        if (textoMisionUI != null)
        {
            textoMisionUI.gameObject.SetActive(true);
            yield return new WaitForSeconds(duracionMision);

            if (!esMisionPrincipal)
            {
                textoMisionUI.gameObject.SetActive(false);
            }
        }
    }
}
