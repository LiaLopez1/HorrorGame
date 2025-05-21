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
    private CanvasGroup settingsCanvasGroup;

    void Start()
    {
        // Asegurar que tenemos un CanvasGroup en el panel de settings
        settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        if (settingsCanvasGroup == null)
        {
            settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
        }

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
            settingsCanvasGroup.interactable = true;
            settingsCanvasGroup.blocksRaycasts = true;
            DeactivateAllPanels();
            pantallaPanel.SetActive(true);
            isSettingsActive = true;
            Time.timeScale = 0f;
        }
        else
        {
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
            settingsPanel.SetActive(false);
            isSettingsActive = false;
            Time.timeScale = 1f;
        }
    }

    // Método para activar un panel basado en el nombre
    public void ActivatePanel(string panelName)
    {
        DeactivateAllPanels();

        switch (panelName.ToLower()) // Hacer la comparación case-insensitive
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
            case "salir":
                salirpanel.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Panel no reconocido: {panelName}");
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
        ToggleSettingsPanel(false);
    }

    private void ToggleOpciones()
    {
        ToggleSettingsPanel(!isSettingsActive);
    }
}