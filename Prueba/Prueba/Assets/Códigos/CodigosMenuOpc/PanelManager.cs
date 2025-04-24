using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // Paneles que se activarán/desactivarán
    public GameObject settingsPanel;
    public GameObject pantallaPanel;
    public GameObject sonidoPanel;
    public GameObject controlesPanel;
    public GameObject cuentaPanel;

    public static bool isSettingsActive = false;

    void Start()
    {
        settingsPanel.SetActive(false);
        DeactivateAllPanels();
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
        }
        else
        {
            settingsPanel.SetActive(false);
            isSettingsActive = false;
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
            case "cuenta":
                cuentaPanel.SetActive(true);
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
        cuentaPanel.SetActive(false);
    }
}