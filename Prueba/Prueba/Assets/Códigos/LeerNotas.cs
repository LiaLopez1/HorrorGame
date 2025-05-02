using UnityEngine;
using UnityEngine.UI;
using System;

public class LeerNotas : MonoBehaviour
{
    [Header("Indicador en 3D")]
    public GameObject exclamacion3D;  // Objeto del signo de exclamación en 3D
    public float rangoActivacion = 5f;  // Rango de proximidad para mostrar el signo
    private Transform player;
    private bool notaLeida = false;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
        if (rawImagenNota != null) rawImagenNota.gameObject.SetActive(false);
        if (exclamacion3D != null) exclamacion3D.SetActive(false);
    }

    void Update()
    {
        CheckPlayerLook();
        HandleInput();
        if (!notaLeida)
            VerificarRangoProximidad();

    }
    private void VerificarRangoProximidad()
    {
        float distancia = Vector3.Distance(player.position, transform.position);

        if (distancia < rangoActivacion && !isNear)
        {
            if (exclamacion3D != null) exclamacion3D.SetActive(true);
        }
        else
        {
            if (exclamacion3D != null) exclamacion3D.SetActive(false);
        }
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
                if (exclamacion3D != null) exclamacion3D.SetActive(false); // Oculta el signo al mirar
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
            notaLeida = true; // <- Ya fue leída
            if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);
            if (exclamacion3D != null) exclamacion3D.SetActive(false);
            OnNoteOpenedAction?.Invoke();
        }
    }

    private void OcultarNota()
    {
        if (rawImagenNota != null)
        {
            rawImagenNota.gameObject.SetActive(false);
            isViewingNote = false;
            notaLeida = true;

            if (exclamacion3D != null) exclamacion3D.SetActive(false);
            if (indicadorInteraccion != null) indicadorInteraccion.SetActive(false);

            OnNoteClosedAction?.Invoke();

            // 👉 Desactiva el GameObject completo de la nota
            gameObject.SetActive(false);
        }
    }
}