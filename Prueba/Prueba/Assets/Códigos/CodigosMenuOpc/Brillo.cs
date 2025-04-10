using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicaBrillo : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image panelBrillo;

    // Start is called before the first frame update
    void Start()
    {
        slider.maxValue = 0.8f; // Limitar el valor máximo a 0.8
        slider.value = PlayerPrefs.GetFloat("brillo", 0.5f);

        // Limitar el valor del slider a 0.8 si es que se cargó un valor mayor
        if (slider.value > 0.8f)
        {
            slider.value = 0.8f;
        }

        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, slider.value);
    }

    public void ChangeSlider(float valor)
    {
        sliderValue = Mathf.Min(valor, 0.8f); // Asegurar que el valor nunca supere 0.8
        PlayerPrefs.SetFloat("brillo", sliderValue);
        panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, slider.value);
    }
}
