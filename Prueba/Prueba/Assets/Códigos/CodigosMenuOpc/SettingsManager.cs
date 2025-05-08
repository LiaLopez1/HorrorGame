using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI")]
    public Slider brilloSlider;
    public TMP_Dropdown calidadDropdown;
    public Slider volumenAmbienteSlider;
    public Slider volumenEfectosSlider;
    [SerializeField] private Image brilloOverlay;

    private float brillo = 1f;
    private int calidad = 2;
    private float volumenAmbiente = 1f;
    private float volumenEfectos = 1f;

    // Buses de FMOD
    private Bus busAmbiente;
    private Bus busEfectos;

    // Instancias de eventos (si necesitas controlar algo directo)
    private EventInstance musicEvent;
    private EventInstance efectosEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cargar buses correctamente desde FMOD (ajusta según tu estructura de buses)
            busAmbiente = RuntimeManager.GetBus("bus:/Music");
            busEfectos = RuntimeManager.GetBus("bus:/ui");

            // Aplicar configuraciones guardadas
            CargarConfiguraciones();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarBrillo(float valor)
    {
        if (brilloOverlay != null)
        {
            Color color = brilloOverlay.color;
            color.a = Mathf.Clamp01(1f - valor); // 0 = sin oscuridad, 1 = oscuro
            brilloOverlay.color = color;
        }
    }

    public void CambiarCalidad(int nivel)
    {
        calidad = nivel;
        QualitySettings.SetQualityLevel(calidad);
    }

    public void CambiarVolumenAmbiente(float valor)
    {
        volumenAmbiente = valor;
        busAmbiente.setVolume(volumenAmbiente);
    }

    public void CambiarVolumenEfectos(float valor)
    {
        volumenEfectos = valor;
        busEfectos.setVolume(volumenEfectos);
    }

    public void GuardarConfiguraciones()
    {
        PlayerPrefs.SetFloat("Brillo", brillo);
        PlayerPrefs.SetInt("Calidad", calidad);
        PlayerPrefs.SetFloat("VolumenAmbiente", volumenAmbiente);
        PlayerPrefs.SetFloat("VolumenEfectos", volumenEfectos);
    }

    public void CargarConfiguraciones()
    {
        brillo = PlayerPrefs.GetFloat("Brillo", 1f);
        calidad = PlayerPrefs.GetInt("Calidad", 2);
        volumenAmbiente = PlayerPrefs.GetFloat("VolumenAmbiente", 1f);
        volumenEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 1f);

        CambiarBrillo(brillo);
        CambiarCalidad(calidad);
        CambiarVolumenAmbiente(volumenAmbiente);
        CambiarVolumenEfectos(volumenEfectos);
    }

    public void IniciarMusica()
    {
        musicEvent.start();
    }

    public void DetenerMusica()
    {
        musicEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
