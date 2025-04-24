using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject canvasSecundario; // Asigna el CanvasSecundario en el Inspector
    public PanelManager panelManager;   // Asigna el PanelManager en el Inspector

    // Método para activar el CanvasSecundario
    public void MostrarCanvasSecundario()
    {
        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(true);
            if (panelManager != null)
            {
                panelManager.ToggleSettingsPanel(true);
            }
        }
    }

    // Método para ocultar el CanvasSecundario
    public void OcultarCanvasSecundario()
    {
        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(false);
            if (panelManager != null)
            {
                panelManager.ToggleSettingsPanel(false);
            }
        }
    }
}