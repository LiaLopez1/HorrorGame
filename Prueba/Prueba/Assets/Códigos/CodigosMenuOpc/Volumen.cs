using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumen : MonoBehaviour
{
    public Slider sliderAmbiente;
    public Slider sliderEfectos;  // Nuevo slider para el volumen de efectos de sonido
    public float sliderAmbienteValue;
    public float sliderEfectosValue;  // Valor del slider de efectos de sonido

    // Imágenes para el volumen de ambiente
    public Image imagenMuteAmbiente;
    public Image imagenHighAmbiente;
    public Image imagenLowAmbiente;

    // Imágenes para el volumen de efectos de sonido
    public Image imagenMuteEfectos;
    public Image imagenHighEfectos;
    public Image imagenLowEfectos;

    void Start()
    {
        // Cargar el volumen del ambiente y los efectos de sonido desde PlayerPrefs
        sliderAmbienteValue = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        sliderEfectosValue = PlayerPrefs.GetFloat("volumenEfectos", 0.5f);

        // Asignar valores a los sliders
        sliderAmbiente.value = sliderAmbienteValue;
        sliderEfectos.value = sliderEfectosValue;

        // Asignar el volumen de ambiente
        AudioListener.volume = sliderAmbienteValue;

        // Actualizar imágenes y volúmenes al inicio
        ActualizarVolumenAmbiente();
        ActualizarVolumenEfectos();
    }

    public void CambiarVolumenAmbiente(float valor)
    {
        sliderAmbienteValue = sliderAmbiente.value;
        PlayerPrefs.SetFloat("volumenAudio", sliderAmbienteValue);
        AudioListener.volume = sliderAmbienteValue;
        ActualizarVolumenAmbiente();
    }

    public void CambiarVolumenEfectos(float valor)
    {
        sliderEfectosValue = sliderEfectos.value;
        PlayerPrefs.SetFloat("volumenEfectos", sliderEfectosValue);

        // Encontrar todos los objetos con un AudioSource de efectos y actualizar su volumen
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.gameObject.CompareTag("Efectos"))  // Solo actualizamos los AudioSources de efectos
            {
                audioSource.volume = sliderEfectosValue;
            }
        }

        ActualizarVolumenEfectos();
    }

    public void ActualizarVolumenAmbiente()
    {
        if (sliderAmbienteValue == 0)
        {
            imagenMuteAmbiente.enabled = true;
            imagenLowAmbiente.enabled = false;
            imagenHighAmbiente.enabled = false;
        }
        else if (sliderAmbienteValue > 0 && sliderAmbienteValue < 0.7)
        {
            imagenMuteAmbiente.enabled = false;
            imagenLowAmbiente.enabled = true;
            imagenHighAmbiente.enabled = false;
        }
        else
        {
            imagenMuteAmbiente.enabled = false;
            imagenLowAmbiente.enabled = false;
            imagenHighAmbiente.enabled = true;
        }
    }

    public void ActualizarVolumenEfectos()
    {
        if (sliderEfectosValue == 0)
        {
            imagenMuteEfectos.enabled = true;
            imagenLowEfectos.enabled = false;
            imagenHighEfectos.enabled = false;
        }
        else if (sliderEfectosValue > 0 && sliderEfectosValue < 0.7)
        {
            imagenMuteEfectos.enabled = false;
            imagenLowEfectos.enabled = true;
            imagenHighEfectos.enabled = false;
        }
        else
        {
            imagenMuteEfectos.enabled = false;
            imagenLowEfectos.enabled = false;
            imagenHighEfectos.enabled = true;
        }
    }
}
