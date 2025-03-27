using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using FMOD;
using FMODUnity;

using Debug = UnityEngine.Debug; // Fuerza a usar siempre UnityEngine.Debug
public class MicrophoneCapture : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text dbText;
    [SerializeField] private Slider volumeSlider;

    [Header("Audio Settings")]
    [SerializeField] private float minDB = -80f; // Silencio total
    [SerializeField] private float maxDB = -15f; // Máximo esperado
    [SerializeField] private float smoothingSpeed = 5f; // Suavizado del movimiento

    private FMOD.System fmodSystem;
    private Sound sound;
    private CREATESOUNDEXINFO exinfo;
    private bool isInitialized = false;
    private float currentSmoothedValue = 0f;

    void Start()
    {
        // Inicialización FMOD (igual que antes)
        fmodSystem = RuntimeManager.CoreSystem;
        exinfo = new CREATESOUNDEXINFO();
        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
        exinfo.numchannels = 1;
        exinfo.format = SOUND_FORMAT.PCM16;
        exinfo.defaultfrequency = 44100;
        exinfo.length = (uint)(exinfo.defaultfrequency * sizeof(short) * exinfo.numchannels);

        RESULT result = fmodSystem.createSound("", MODE.LOOP_NORMAL | MODE.OPENUSER, ref exinfo, out sound);
        if (result != RESULT.OK) UnityEngine.Debug.LogError("Error FMOD: " + result);
        
        fmodSystem.recordStart(0, sound, true);
        isInitialized = true;

        // Configuración inicial del Slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = 0;
        }
    }

    void Update()
    {
        if (!isInitialized) return;

        float volumeDB = GetVolumeInDecibels();
        UpdateUI(volumeDB);
    }


    float GetVolumeInDecibels()
    {
        IntPtr ptr1, ptr2;
        uint len1, len2;
        sound.@lock(0, exinfo.length, out ptr1, out ptr2, out len1, out len2);

        // Versión segura con Marshal.Copy (alternativa al unsafe)
        byte[] buffer = new byte[len1];
        System.Runtime.InteropServices.Marshal.Copy(ptr1, buffer, 0, (int)len1);

        float sum = 0f;
        int sampleCount = (int)len1 / sizeof(short);

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(buffer, i * sizeof(short));
            float normalizedSample = sample / 32768f;
            sum += normalizedSample * normalizedSample;
        }

        sound.unlock(ptr1, ptr2, len1, len2);
        float rms = Mathf.Sqrt(sum / sampleCount);
        return rms > 0.0001f ? 20f * Mathf.Log10(rms) : minDB;
    }

    void UpdateUI(float currentDB)
    {
        // 1. Normalizar dB al rango [0, 1]
        float normalizedValue = Mathf.InverseLerp(minDB, maxDB, currentDB);

        // 2. Suavizar el movimiento (opcional pero recomendado)
        currentSmoothedValue = Mathf.Lerp(
            currentSmoothedValue,
            normalizedValue,
            Time.deltaTime * smoothingSpeed
        );

        // 3. Actualizar Slider y Texto
        if (volumeSlider != null)
        {
            volumeSlider.value = currentSmoothedValue;
        }

        if (dbText != null)
        {
            dbText.text = $"{currentDB:F1}";
        }
    }

    void OnDestroy()
    {
        if (isInitialized)
        {
            fmodSystem.recordStop(0);
            sound.release();
        }
    }
}