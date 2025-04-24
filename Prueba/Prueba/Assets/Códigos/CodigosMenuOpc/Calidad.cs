using UnityEngine;
using TMPro;
using System.Collections;

public class Calidad : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public RectTransform panelConfiguracion; // Cambiado a RectTransform

    IEnumerator Start()
    {
        // Espera un frame para evitar conflictos de inicialización
        yield return null;

        if (panelConfiguracion != null)
        {
            panelConfiguracion.gameObject.SetActive(true);
            panelConfiguracion.ForceUpdateRectTransforms();
            panelConfiguracion.SetAsLastSibling();
        }

        // Configuración inicial del dropdown
        ConfigurarDropdown();
    }

    void ConfigurarDropdown()
    {
        dropdown.onValueChanged.AddListener((valor) => {
            StartCoroutine(ForzarVisibilidadPanel());
            AplicarCalidad(valor);
        });
    }

    IEnumerator ForzarVisibilidadPanel()
    {
        yield return null; // Espera un frame
        if (panelConfiguracion != null)
        {
            panelConfiguracion.gameObject.SetActive(true);
            panelConfiguracion.SetAsLastSibling();
        }
    }

    void AplicarCalidad(int nivel)
    {
        QualitySettings.SetQualityLevel(nivel == 0 ? 0 : QualitySettings.names.Length - 1);
        PlayerPrefs.SetInt("numeroDeCalidad", nivel);
    }
}