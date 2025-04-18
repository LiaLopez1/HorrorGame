using UnityEngine;
using UnityEngine.UI;
using System;

public class LeerNotas : MonoBehaviour
{
    [Header("Configuración Visual")]
    public GameObject indicadorInteraccion;
    public RawImage rawImagenNota;
    public float raycastDistance = 3f;

    [Header("Eventos")]
    public Action OnNoteOpenedAction;
    public Action OnNoteClosedAction;

    private Transform playerCamera;
    [HideInInspector] public bool isViewingNote = false;
    private bool isNear = false;

    void Start()
    {
        playerCamera = Camera.main.transform;
        if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
        if (rawImagenNota != null) rawImagenNota.gameObject.SetActive(false);
    }

    void Update()
    {
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
                if (indicadorInteraccion != null && !isViewingNote)
                    indicadorInteraccion.SetActive(true);
            }
            else if (isNear && hit.collider.gameObject != gameObject)
            {
                isNear = false;
                if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
            }
        }
        else if (isNear)
        {
            isNear = false;
            if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isNear && !isViewingNote)
            {
                MostrarNota();
            }
            else if (isViewingNote)
            {
                OcultarNota();
            }
        }
    }

    private void MostrarNota()
    {
        if (rawImagenNota != null)
        {
            rawImagenNota.gameObject.SetActive(true);
            isViewingNote = true;
            if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
            OnNoteOpenedAction?.Invoke();
        }
    }

    private void OcultarNota()
    {
        if (rawImagenNota != null)
        {
            rawImagenNota.gameObject.SetActive(false);
            isViewingNote = false;
            OnNoteClosedAction?.Invoke();
        }
    }
}