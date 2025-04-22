using UnityEngine;
using System;

public class RecogerObjetos: MonoBehaviour
{
    [Header("Configuración Visual")]
    public GameObject indicadorInteraccion; // El icono de "E"
    public float raycastDistance = 3f;

    [Header("Eventos")]
    public Action OnItemCollectedAction;

    private Transform playerCamera;
    private bool isNear = false;
    private bool isCollected = false;

    void Start()
    {
        playerCamera = Camera.main.transform;
        if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
    }

    void Update()
    {
        if (isCollected) return;

        CheckPlayerLook();
        HandleInput();
    }

    private void CheckPlayerLook()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            if (hit.collider.gameObject == gameObject && !isNear)
            {
                isNear = true;
                if (indicadorInteraccion != null)
                    indicadorInteraccion.SetActive(true);
            }
            else if (isNear && hit.collider.gameObject != gameObject)
            {
                isNear = false;
                if (indicadorInteraccion != null)
                    indicadorInteraccion.SetActive(false);
            }
        }
        else if (isNear)
        {
            isNear = false;
            if (indicadorInteraccion != null)
                indicadorInteraccion.SetActive(false);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && isNear)
        {
            RecogerObjeto();
        }
    }

    private void RecogerObjeto()
    {
        isCollected = true;

        // Desactivar el indicador
        if (indicadorInteraccion != null)
            indicadorInteraccion.SetActive(false);

        // Evento opcional para lógica externa
        OnItemCollectedAction?.Invoke();

        // Aquí puedes hacer efectos antes de destruir
        Destroy(gameObject); // Destruye el objeto completamente
    }
}
