using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RelAspect : MonoBehaviour
{
    public TMP_Dropdown aspectRatioDropdown; 
    // Resoluciones recomendadas para cada relación de aspecto
    private readonly Dictionary<string, Vector2Int> aspectRatios = new Dictionary<string, Vector2Int>
    {
        {"16:9", new Vector2Int(1920, 1080)},
        {"16:10", new Vector2Int(1920, 1200)}, //Widescreen
        //{"21:9", new Vector2Int(2560, 1080)}, //Ultrawide
        {"3:2", new Vector2Int(2160, 1440)}//Relación usada en algunos portátiles y tablets modernos
    };

    void Start()
    {

        // Inicializar el dropdown de relación de aspecto
        aspectRatioDropdown.ClearOptions();
        List<string> options = new List<string>(aspectRatios.Keys); // Añadir las opciones
        aspectRatioDropdown.AddOptions(options);

        // Escuchar cambios en el Dropdown
        aspectRatioDropdown.onValueChanged.AddListener(delegate { CambiarRelacionDeAspecto(aspectRatioDropdown); });
    }

    public void ActivarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    // Método para cambiar la relación de aspecto según la selección del Dropdown
    public void CambiarRelacionDeAspecto(TMP_Dropdown dropdown)
    {
        string selectedAspectRatio = dropdown.options[dropdown.value].text;

        if (aspectRatios.ContainsKey(selectedAspectRatio))
        {
            Vector2Int resolution = aspectRatios[selectedAspectRatio];
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        }
    }
}
