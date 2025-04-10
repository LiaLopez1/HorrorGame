using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class Calidad : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int calidad;

    void Start()
    {
        // Asegurar que el dropdown solo tenga dos opciones: Baja y Alta
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string> { "Baja", "Alta" });

        // Cargar la calidad guardada, asegurando que solo sea 0 (Baja) o 1 (Alta)
        calidad = PlayerPrefs.GetInt("numeroDeCalidad", 1); 
        calidad = Mathf.Clamp(calidad, 0, 1); 

        dropdown.value = calidad;
        dropdown.onValueChanged.AddListener(delegate { AjustarCalidad(); });

        AjustarCalidad();
    }

    public void AjustarCalidad()
    {
        int calidadSeleccionada = dropdown.value;

        // Asignar la calidad de Unity (0 = Baja, 1 = Alta)
        if (calidadSeleccionada == 0)
        {
            QualitySettings.SetQualityLevel(0); // Baja
        }
        else
        {
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1); // Alta
        }

        // Guardar la calidad seleccionada
        PlayerPrefs.SetInt("numeroDeCalidad", calidadSeleccionada);
        calidad = calidadSeleccionada;
    }
}
