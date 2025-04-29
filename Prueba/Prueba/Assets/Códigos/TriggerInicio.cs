using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerInicio : MonoBehaviour
{
    [Header("Configuración Diálogo")]
    [TextArea(3, 5)] public string mensajeDialogo;
    public float duracionDialogo = 3f;
    public GameObject panelDialogo;
    public TMP_Text textoDialogo;

    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered && other.CompareTag("Player"))
        {
            alreadyTriggered = true;
            StartCoroutine(MostrarDialogo());
        }
    }

    private IEnumerator MostrarDialogo()
    {
        if (panelDialogo != null && textoDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = mensajeDialogo;

            yield return new WaitForSeconds(duracionDialogo);
            panelDialogo.SetActive(false);
        }
        else
        {
            Debug.LogError("Panel o texto de diálogo no asignado en el Inspector", this);
        }
    }
}
