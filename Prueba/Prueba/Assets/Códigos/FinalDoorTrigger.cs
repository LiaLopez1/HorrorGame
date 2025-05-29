// FinalDoorTrigger.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoorTrigger : MonoBehaviour
{
    public GameObject interactionPanel;
    public string nextSceneName = "NombreDeLaEscena";

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPanel.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionPanel.SetActive(false);
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
