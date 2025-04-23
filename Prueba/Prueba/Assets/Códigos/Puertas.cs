using UnityEngine;
using UnityEngine.UI;
using System.Collections; // ¡Asegúrate de incluir esto para las corrutinas!

public class Puerta : MonoBehaviour
{
    [Header("Estado")]
    public bool isLocked = true;
    private bool isNear = false;
    private bool isOpen = false;
    private Coroutine rotationCoroutine; // Controla la corrutina de rotación

    [Header("Configuración Visual")]
    public GameObject interactMessagePanel;
    public GameObject lockedMessagePanel;
    public float interactionDistance = 5f;

    [Header("Diálogo Bloqueada")]
    public Text dialogueText;
    public string blockedMessage = "Necesitas la Llave Antigua para abrirme.";

    [Header("Rotación Suave")]
    public float rotationDuration = 0.5f; // Duración de la animación

    private Camera playerCamera;

    void Start()
    {
        if (interactMessagePanel != null) interactMessagePanel.SetActive(false);
        if (lockedMessagePanel != null) lockedMessagePanel.SetActive(false);

        playerCamera = Camera.main;
    }

    void Update()
    {
        CheckPlayerInteraction();

        if (!isLocked && Input.GetKeyDown(KeyCode.E) && isNear)
        {
            ToggleDoor();
        }
        else if (isLocked && Input.GetKeyDown(KeyCode.E) && isNear)
        {
            ShowBlockedDialogue();
        }
    }

    private void CheckPlayerInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider != null && hit.transform == transform)
            {
                if (!isNear)
                {
                    isNear = true;
                    UpdateMessagePanel();
                }
                return;
            }
        }

        if (isNear)
        {
            isNear = false;
            UpdateMessagePanel();
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
        UpdateMessagePanel();
    }

    private void ToggleDoor()
    {

       //MODUnity.RuntimeManager.PlayOneShot("event:/puerta_abrir"); 
                                                                    
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine); // Detiene la animación si ya está en curso
        }

        rotationCoroutine = StartCoroutine(RotateDoorSmoothly(isOpen ? 90f : -90f));
        isOpen = !isOpen;
    }

    private IEnumerator RotateDoorSmoothly(float targetAngle)
    {
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null; // Espera un frame
        }

        transform.rotation = endRotation; // Asegura la rotación final exacta
    }

    private void ShowBlockedDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.text = blockedMessage;
            Invoke("HideDialogue", 3f);
        }
    }

    private void HideDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    void UpdateMessagePanel()
    {
        if (isNear)
        {
            if (isLocked)
            {
                if (lockedMessagePanel != null) lockedMessagePanel.SetActive(true);
                if (interactMessagePanel != null) interactMessagePanel.SetActive(false);
            }
            else
            {
                if (lockedMessagePanel != null) lockedMessagePanel.SetActive(false);
                if (interactMessagePanel != null) interactMessagePanel.SetActive(true);
            }
        }
        else
        {
            if (lockedMessagePanel != null) lockedMessagePanel.SetActive(false);
            if (interactMessagePanel != null) interactMessagePanel.SetActive(false);
        }
    }
}