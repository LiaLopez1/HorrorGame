using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject canvasSecundario;
    public PanelManager panelManager;   
    public Canvas gameCanvas; 
    public Canvas menuCanvas; 

    private CanvasGroup gameCanvasGroup; 

    void Start()
    {
        gameCanvasGroup = gameCanvas.GetComponent<CanvasGroup>();
        menuCanvas.sortingOrder = 2;  
        gameCanvas.sortingOrder = 1;  
    }

    public void MostrarCanvasSecundario()
    {
        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(true);
            if (panelManager != null)
            {
                panelManager.ToggleSettingsPanel(true); 
            }

            Time.timeScale = 0f;
            gameCanvasGroup.blocksRaycasts = false; 
        }
    }

    public void OcultarCanvasSecundario()
    {
        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(false);
            if (panelManager != null)
            {
                panelManager.ToggleSettingsPanel(false); 
            }

            Time.timeScale = 1f;
            gameCanvasGroup.blocksRaycasts = true;
        }
    }
}
