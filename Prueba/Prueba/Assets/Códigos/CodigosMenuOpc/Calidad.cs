using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaCalidad : MonoBehaviour
{
    public TMP_Dropdown dropdownCalidad;

    void Start()
    {
        int savedQuality = PlayerPrefs.GetInt("calidad", 2); // 2 es la calidad por defecto (Medium)
        dropdownCalidad.value = savedQuality;
        ApplyQuality(savedQuality);
        
        // Conectar el evento del Dropdown
        dropdownCalidad.onValueChanged.AddListener(ChangeQuality);
    }

    public void ChangeQuality(int index)
    {
        PlayerPrefs.SetInt("calidad", index); // Guardamos la opción seleccionada
        ApplyQuality(index);
    }

    private void ApplyQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
