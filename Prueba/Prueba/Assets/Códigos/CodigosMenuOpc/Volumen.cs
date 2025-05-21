using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class VolumenFMOD : MonoBehaviour
{
    public Slider sliderAmbiente;
    public Slider sliderMaster;

    public Image imagenMuteAmbiente;
    public Image imagenLowAmbiente;
    public Image imagenHighAmbiente;

    public Image imagenMuteMaster;
    public Image imagenLowMaster;
    public Image imagenHighMaster;

    private VCA vcaAmbiente;
    private VCA vcaefectos;

    void Start()
    {
        float volumenAmbiente = PlayerPrefs.GetFloat("volumenAmbiente", 1f);
        float volumenEfectos = PlayerPrefs.GetFloat("volumenEfectos", 0.5f);

        sliderAmbiente.value = volumenAmbiente;
        sliderMaster.value = volumenEfectos;

        // Obtener las rutas de los VCA
        vcaAmbiente = RuntimeManager.GetVCA("vca:/AmbienteVCA");
        vcaefectos = RuntimeManager.GetVCA("vca:/EfectosVCA");

        // Aplicar valores iniciales
        vcaAmbiente.setVolume(volumenAmbiente);
        vcaefectos.setVolume(volumenEfectos);

        ActualizarImagenAmbiente(volumenAmbiente);
        ActualizarImagenMaster(volumenEfectos);

        // Añadir listeners
        sliderAmbiente.onValueChanged.AddListener(CambiarVolumenAmbiente);
        sliderMaster.onValueChanged.AddListener(CambiarVolumenMaster);
    }

    public void CambiarVolumenAmbiente(float valor)
    {
        //Debug.Log("CambiarVolumenAmbiente a " + valor);
        vcaAmbiente.setVolume(valor);
        PlayerPrefs.SetFloat("volumenAmbiente", valor);
        ActualizarImagenAmbiente(valor);
    }

    public void CambiarVolumenMaster(float valor)
    {
        vcaefectos.setVolume(valor);
        PlayerPrefs.SetFloat("volumenEfectos", valor);
        ActualizarImagenMaster(valor);
    }

    private void ActualizarImagenAmbiente(float valor)
    {
        imagenMuteAmbiente.enabled = valor == 0;
        imagenLowAmbiente.enabled = valor > 0 && valor < 0.7f;
        imagenHighAmbiente.enabled = valor >= 0.7f;
    }

    private void ActualizarImagenMaster(float valor)
    {
        imagenMuteMaster.enabled = valor == 0;
        imagenLowMaster.enabled = valor > 0 && valor < 0.7f;
        imagenHighMaster.enabled = valor >= 0.7f;
    }
}
