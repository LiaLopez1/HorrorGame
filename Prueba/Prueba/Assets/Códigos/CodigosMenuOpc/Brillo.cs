using UnityEngine;
using UnityEngine.UI;

public class LogicaBrillo : MonoBehaviour
{
    public Slider slider;
    public Image panelBrillo;

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat("brillo", 0.5f);

        if (slider != null)
        {
            slider.maxValue = 0.8f;
            slider.value = Mathf.Min(savedValue, 0.8f);
            slider.onValueChanged.AddListener(ChangeSlider); // ← conecta el slider dinámicamente
        }

        ApplyBrightness(savedValue);
    }

    public void ChangeSlider(float valor)
    {
        float clampedValue = Mathf.Min(valor, 0.8f);
        PlayerPrefs.SetFloat("brillo", clampedValue);
        ApplyBrightness(clampedValue);
    }

    private void ApplyBrightness(float value)
    {
        if (panelBrillo != null)
        {
            Color color = panelBrillo.color;
            color.a = value;
            panelBrillo.color = color;
        }
    }
}
