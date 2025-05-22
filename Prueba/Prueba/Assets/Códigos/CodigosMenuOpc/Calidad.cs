using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaCalidad : MonoBehaviour
{
    public TMP_Dropdown dropdownCalidad;
    
    void Start()
    {
        int maxQualityLevel = QualitySettings.names.Length - 1;
        

        int savedQuality = PlayerPrefs.GetInt("calidad", maxQualityLevel);
        
        savedQuality = Mathf.Clamp(savedQuality, 0, maxQualityLevel);
        
        dropdownCalidad.value = savedQuality;
        ApplyQuality(savedQuality);

        dropdownCalidad.onValueChanged.AddListener(ChangeQuality);
    }

    public void ChangeQuality(int index)
    {
        // Validar que el índice esté dentro del rango
        if (index >= 0 && index < QualitySettings.names.Length)
        {
            PlayerPrefs.SetInt("calidad", index);
            ApplyQuality(index);
        }
        else
        {
            Debug.LogWarning("Índice de calidad no válido: " + index);
        }
    }

    private void ApplyQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        
        if (qualityIndex == 0) 
        {
            // Alta calidad
            QualitySettings.pixelLightCount = 8;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            QualitySettings.shadowDistance = 200f;
            QualitySettings.shadowCascades = 4;

            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            QualitySettings.realtimeReflectionProbes = true; 
        }
        else 
        {
            // Baja calidad
            QualitySettings.pixelLightCount = 1;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowDistance = 50;
        }
    }
}