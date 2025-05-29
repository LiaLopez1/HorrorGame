using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    // Paneles que se activarán/desactivarán
    public GameObject settingsPanel;
    public GameObject pantallaPanel;
    public GameObject sonidoPanel;
    public GameObject controlesPanel;
    public CanvasManager canvasManager;
    public string escenaDondeOcultarMouse = "SampleScene";

    public static bool isSettingsActive = false;
    private CanvasGroup settingsCanvasGroup;

    void Start()
    {
        settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        if (settingsCanvasGroup == null)
        {
            settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
        }

        settingsPanel.SetActive(false);
        DeactivateAllPanels();
        ConfigurarCursorInicial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOpciones();
        }
    }

    private void ConfigurarCursorInicial()
    {
        string nombreEscenaActual = SceneManager.GetActiveScene().name;
        bool debeOcultarMouse = DebeOcultarMouse(nombreEscenaActual);

        Cursor.lockState = debeOcultarMouse ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !debeOcultarMouse;
    }

    private bool DebeOcultarMouse(string nombreEscena)
    {
        return nombreEscena == escenaDondeOcultarMouse;
    }

    public void ToggleSettingsPanel(bool show)
    {
        if (canvasManager != null)
        {
            canvasManager.canvasSecundario.SetActive(show);
        }

        if (show)
        {
            settingsPanel.SetActive(true);
            settingsCanvasGroup.interactable = true;
            settingsCanvasGroup.blocksRaycasts = true;
            DeactivateAllPanels();
            pantallaPanel.SetActive(true);
            isSettingsActive = true;
            Time.timeScale = 0f;

            // Siempre mostrar cursor cuando el menú está abierto
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
            settingsPanel.SetActive(false);
            isSettingsActive = false;
            Time.timeScale = 1f;

            // Configurar cursor según la escena actual
            string nombreEscenaActual = SceneManager.GetActiveScene().name;
            bool debeOcultarMouse = DebeOcultarMouse(nombreEscenaActual);

            Cursor.lockState = debeOcultarMouse ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !debeOcultarMouse;
        }
    }

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
            default:
                Debug.LogWarning($"Panel no reconocido: {panelName}");
                pantallaPanel.SetActive(true);
                break;
        }
    }

    private void DeactivateAllPanels()
    {
        pantallaPanel.SetActive(false);
        sonidoPanel.SetActive(false);
        controlesPanel.SetActive(false);
        //salirpanel.SetActive(false);
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