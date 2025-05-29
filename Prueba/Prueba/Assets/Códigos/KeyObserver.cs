// KeyObserver.cs
using UnityEngine;

public class KeyObserver : MonoBehaviour
{
    public RecogerObjetos keyScript; // Referencia al script de la llave
    public GameObject finalDoorTrigger;

    private bool activated = false;

    void Update()
    {
        if (!activated && keyScript.isCollected)
        {
            finalDoorTrigger.SetActive(true);
            activated = true; // Evita múltiples activaciones
        }
    }
}
