using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // Paneles que se activarán/desactivarán
    public GameObject settingsPanel;
    public GameObject pantallaPanel;
    public GameObject sonidoPanel;
    public GameObject controlesPanel;
    public GameObject salirpanel;

    public static bool isSettingsActive = false;

    void Start()
    {
        settingsPanel.SetActive(false);
        DeactivateAllPanels();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOpciones();
        }
    }

    // Método público para alternar la visibilidad del panel de configuración
    public void ToggleSettingsPanel(bool show)
    {
        if (show)
        {
            settingsPanel.SetActive(true);
            DeactivateAllPanels();
            pantallaPanel.SetActive(true);
            isSettingsActive = true;
            Time.timeScale = 0f;
        }
        else
        {
            settingsPanel.SetActive(false);
            isSettingsActive = false;
            Time.timeScale = 1f;
        }
    }

    // Método para activar un panel basado en el nombre usando un switch
    public void ActivatePanel(string panelName)
    {
        DeactivateAllPanels();

        switch (panelName)
        {
            case "pantalla":
                pantallaPanel.SetActive(true);
                break;
            case "sonido":
                sonidoPanel.SetActive(true);
                break;
            case "controles":
                controlesPanel.SetActive(true);
                break;
            case "Salir":
                salirpanel.SetActive(true);
                break;
            default:
                pantallaPanel.SetActive(true);
                break;
        }
    }

    // Método para desactivar todos los paneles
    private void DeactivateAllPanels()
    {
        pantallaPanel.SetActive(false);
        sonidoPanel.SetActive(false);
        controlesPanel.SetActive(false);
        salirpanel.SetActive(false);
    }

    public void CerrarOpciones()
    {
        settingsPanel.SetActive(false);
        isSettingsActive = false;
        Time.timeScale = 0f;
    }

    private void ToggleOpciones()
{
    if (isSettingsActive)
    {
        CerrarOpciones();
    }
    else
    {
        ToggleSettingsPanel(true);
    }
}
}